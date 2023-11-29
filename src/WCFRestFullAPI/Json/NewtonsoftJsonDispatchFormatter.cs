using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Xml;

namespace WCFRestFullAPI.Json
{
    class NewtonsoftJsonDispatchFormatter : IDispatchMessageFormatter
    {
        OperationDescription operation;
        Dictionary<string, int> parameterNames;
        ServiceEndpoint endpoint;
        WebMessageBodyStyle bodyStyle;
        public NewtonsoftJsonDispatchFormatter(OperationDescription operation, ServiceEndpoint endpoint, WebMessageBodyStyle bodyStyle, bool isRequest)
        {
            this.operation = operation;
            this.endpoint = endpoint;
            this.bodyStyle = bodyStyle;
            if (isRequest)
            {
                int operationParameterCount = operation.Messages[0].Body.Parts.Count;
                if (operationParameterCount > 1)
                {
                    this.parameterNames = new Dictionary<string, int>();
                    for (int i = 0; i < operationParameterCount; i++)
                    {
                        this.parameterNames.Add(operation.Messages[0].Body.Parts[i].Name, i);
                    }
                }
            }
        }

        /****************************************
         * 2023/11/12
         * 1、添加UriTemple参数判断逻辑
         * 2、添加二进制文件传输判断逻辑
         ***************************************/
        public void DeserializeRequest(Message message, object[] parameters)
        {
            //如果是上传的content-type,则不作json处理
            var headers = ((HttpRequestMessageProperty)(message.Properties[HttpRequestMessageProperty.Name])).Headers;
            string contenttype = headers["Content-type"];
            string contentLength = headers["Content-Length"];

            //获取URI中携带的参数
            var paramts = operation.SyncMethod.GetParameters();
            var uriParamers = GetUriParamers(operation, message);
            List<int> paramersIndexNotInUri = new List<int>();
            for (int i = 0; i < paramts.Length; i++)
            {
                if (uriParamers.ContainsKey(paramts[i].Name.ToUpper()))
                {
                    parameters[i] = uriParamers[paramts[i].Name.ToUpper()];
                }
                else
                {
                    paramersIndexNotInUri.Add(i);
                }
            }

            //大文件分包传输
            if (contenttype.StartsWith("multipart/form-data"))
            {
                //获得附件分割边界字符串
                string boundary = contenttype.Substring(contenttype.IndexOf("boundary=") + "boundary=".Length);
                int len = int.Parse(contentLength);
                //获得方法的参数
                int streamtypeIndx = -1;
                int bodyheaderlen = 0;
                //找到Stream类型的参数
                for (streamtypeIndx = 0; streamtypeIndx < paramts.Length && streamtypeIndx < parameters.Length; streamtypeIndx++)
                {
                    if(paramts[streamtypeIndx].ParameterType==typeof(Stream))
                    {

                        var stream = message.GetBody<Stream>();
                        //定位到第一个0D0A0D0A)
                        //sb= new StringBuilder(512);
                        int datimes = 0;//回车换行次数
                        int c = 0;
                        while (datimes != 4)
                        {
                            c=stream.ReadByte();
                            if (c == -1)
                                break;
                            if (c == 0x0d && (datimes == 0 || datimes == 2))
                            {
                                datimes++;
                            }
                            else if (c == 0x0a && (datimes == 1 || datimes == 3))
                            {
                                datimes++;
                            }
                            else
                                datimes = 0;

                            bodyheaderlen++;
                        }
                        if (c == -1)
                            continue;
                        //计算实际附件大小
                        int fileLength = len - bodyheaderlen - boundary.Length - 6;
                        int remain = fileLength;
                        MemoryStream filestream = new MemoryStream(fileLength);
                        byte[] buffer=new byte[8192];
                        int readed = 0;
                        while (remain>0)
                        {
                            readed = stream.Read(buffer, 0, remain > 8192 ? 8192 : remain);
                            remain -= readed;
                            filestream.Write(buffer, 0, readed);
                        }
                        stream.Close();
                        filestream.Seek(0, SeekOrigin.Begin);
                        //MemoryStream stream = new MemoryStream(message.GetReaderAtBodyContents().ReadElementContentAsBinHex());
                        parameters[streamtypeIndx] = filestream;
                    }
                    else
                    {
                        if (parameters[streamtypeIndx] == null)
                        {
                            parameters[streamtypeIndx] = headers[paramts[streamtypeIndx].Name];
                        }
                    }
                }

                return;
            }

            ///二进制文件传输（Stream）
            if (contenttype.StartsWith("application/x-msdownload"))
            {
                int streamtypeIndx ;
                for (streamtypeIndx = 0; streamtypeIndx < paramts.Length && streamtypeIndx < parameters.Length; streamtypeIndx++)
                {
                    if (paramts[streamtypeIndx].ParameterType == typeof(Stream))
                    {

                        var stream = message.GetBody<Stream>();
                        parameters[streamtypeIndx] = stream;
                    }
                    else
                    {
                        if (parameters[streamtypeIndx] == null)
                        {
                            parameters[streamtypeIndx] = headers[paramts[streamtypeIndx].Name];
                        }
                    }
                }
                return;
            }

            object bodyFormatProperty;
            if (!message.Properties.TryGetValue(WebBodyFormatMessageProperty.Name, out bodyFormatProperty) ||
                (bodyFormatProperty as WebBodyFormatMessageProperty).Format != WebContentFormat.Raw)
            {
                throw new InvalidOperationException("Incoming messages must have a body format of Raw. Is a ContentTypeMapper set on the WebHttpBinding?");
            }

            XmlDictionaryReader bodyReader = message.GetReaderAtBodyContents();
            bodyReader.ReadStartElement("Binary");
            byte[] rawBody = bodyReader.ReadContentAsBase64();
            MemoryStream ms = new MemoryStream(rawBody);

            StreamReader sr = new StreamReader(ms);
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer() {
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            };
            if (paramersIndexNotInUri.Count==1)
            {
                // single parameter, assuming bare
                int indexNotInUri = paramersIndexNotInUri[0];
                parameters[indexNotInUri] = serializer.Deserialize(sr, operation.Messages[0].Body.Parts[indexNotInUri].Type);
            }
            else if(bodyStyle== WebMessageBodyStyle.Wrapped||bodyStyle==WebMessageBodyStyle.WrappedRequest)
            {
                // multiple parameter, needs to be wrapped
                Newtonsoft.Json.JsonReader reader = new Newtonsoft.Json.JsonTextReader(sr);
                reader.Read();
                if (reader.TokenType != Newtonsoft.Json.JsonToken.StartObject)
                {
                    throw new InvalidOperationException("Input needs to be wrapped in an object");
                }

                reader.Read();
                while (reader.TokenType == Newtonsoft.Json.JsonToken.PropertyName)
                {
                    string parameterName = reader.Value as string;
                    reader.Read();
                    if (this.parameterNames.ContainsKey(parameterName))
                    {
                        int parameterIndex = this.parameterNames[parameterName];
                        parameters[parameterIndex] = serializer.Deserialize(reader, this.operation.Messages[0].Body.Parts[parameterIndex].Type);
                    }
                    else
                    {
                        reader.Skip();
                    }

                    reader.Read();
                }

                reader.Close();
            }
            else
            {
                throw new InvalidOperationException("When paramers more than one, WebMessageBodyStyle must set to Wrapped or WrappedRequest!");
            }

            sr.Close();
            ms.Close();
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            byte[] body;
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
                {
                    using (Newtonsoft.Json.JsonWriter writer = new Newtonsoft.Json.JsonTextWriter(sw) { DateFormatString ="yyyy-MM-dd HH:mm:ss"})
                    {
                        //writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                        serializer.Serialize(writer, result);
                        sw.Flush();
                        body = ms.ToArray();
                    }
                }
            }

            Message replyMessage = Message.CreateMessage(messageVersion, operation.Messages[1].Action, new RawBodyWriter(body));
            replyMessage.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));
            HttpResponseMessageProperty respProp = new HttpResponseMessageProperty();
            respProp.Headers[HttpResponseHeader.ContentType] = "application/json";
            replyMessage.Properties.Add(HttpResponseMessageProperty.Name, respProp);
            return replyMessage;
        }
        private UriTemplate GetUriTemplate(OperationDescription operation)
        {
            WebGetAttribute wga = operation.Behaviors.Find<WebGetAttribute>();
            if (wga != null)
            {
                return  new UriTemplate( wga.UriTemplate,true);
            }
            WebInvokeAttribute wia = operation.Behaviors.Find<WebInvokeAttribute>();
            if (wia != null)
            {
                return new UriTemplate(wia.UriTemplate, true);
            }

            return null;
        }

        private Uri GetUri(Message message)
        {
        return message.Properties.Via;
        }

        private Dictionary<string,string> GetUriParamers(OperationDescription operation, Message message)
        {
            Dictionary<string, string> paramers = new Dictionary<string, string>();
            var uriTemplate = GetUriTemplate(operation);
            var uri = GetUri(message);
            var baseUri = endpoint.ListenUri;

            UriTemplateMatch match = uriTemplate.Match(baseUri, uri);

            foreach (var key in match.BoundVariables.AllKeys)
            {
                paramers.Add(key, match.BoundVariables[key]);
            }
            return paramers;
        }

    }
}
