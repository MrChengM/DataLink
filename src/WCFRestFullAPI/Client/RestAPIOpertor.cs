using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace WCFRestFullAPI.Client
{
    public class RestAPIOpertor
    {
        private static string CommomRequestFunc(string url, string type, string body = null, string authorization = null)
        {
            string content = null;

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = type;
            request.ContentType = "application/json";
            
            if (authorization != null)
            {
                request.Headers.Add(HttpRequestHeader.Authorization, authorization);

            }

            if (body != null)
            {
                using (Stream stream = request.GetRequestStream())
                {
                    var postData = Encoding.ASCII.GetBytes(body);
                    stream.Write(postData, 0, postData.Length);
                };

            }
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }
            }

            return content;
        }
        private static string FilePostFunc(string url, byte[] body, string authorization = null)
        {
            string content = null;
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-msdownload";
          
            if (authorization != null)
            {
                request.Headers.Add(HttpRequestHeader.Authorization, authorization);

            }

            if (body != null)
            {
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(body, 0, body.Length);
                };

            }
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }
            }

            return content;
        }
        public static T1 GetFuncJson<T1>(string url, string authorization = null)
        {
            var content = CommomRequestFunc(url, "GET", null, authorization);
            return JsonConvert.DeserializeObject<T1>(content);
        }
        public static T2 PostFuncJson<T1, T2>(string url, T1 body = null, string authorization = null) where T1 : class
        {
            string bodyStr = JsonConvert.SerializeObject(body);
            var content = CommomRequestFunc(url, "POST", bodyStr, authorization);
            return JsonConvert.DeserializeObject<T2>(content);
        }
        /// <summary>
        /// 文件上传功能，未做续传处理
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
        public static T1 PostFileFunc<T1>(string url, byte[] body = null, string authorization = null) where T1 : class
        {
            var content = FilePostFunc(url,body, authorization);
            return JsonConvert.DeserializeObject<T1>(content);
        }
        public static T2 PutFuncJson<T1, T2>(string url, T1 body = null, string authorization = null) where T1 : class
        {
            string bodyStr = JsonConvert.SerializeObject(body);
            var content = CommomRequestFunc(url, "PUT", bodyStr, authorization);
            return JsonConvert.DeserializeObject<T2>(content);
        }
        public static T DeleteFuncJson<T>(string url, string authorization = null)
        {
            var content = CommomRequestFunc(url, "DELETE", null, authorization);
            return JsonConvert.DeserializeObject<T>(content);
        }
        public static string UserNameToBase64Str(string userName, string password)
        {
            byte[] bytes = Encoding.Default.GetBytes(userName + ":" + password);
            string Authorization = Convert.ToBase64String(bytes);

            return "Basic " + Authorization;
        }
    }
}
