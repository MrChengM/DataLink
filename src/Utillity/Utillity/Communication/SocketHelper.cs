using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utillity.Communication
{
  public class SocketHelper
    {
        private bool isConnect;
        private ManualResetEvent timeOutObject;
        private Exception socektException;
        public SocketHelper()
        {
            timeOutObject = new ManualResetEvent(false);
            isConnect = false;
        }
        public bool ConnectWithTimeOut(Socket socket,IPAddress address,int port,int timeOut)
        {
            timeOutObject.Reset();
            socket.BeginConnect(address, port, new AsyncCallback(callBackMethod), socket);
            if (timeOutObject.WaitOne(timeOut,false))
            {
                if (isConnect)
                {
                    return true;
                }
                else
                {
                    throw socektException;
                }

            }
            else
            {
                socket.Close();
                throw socektException = new Exception("Connect TimeOut");
            }
        }

        private void callBackMethod(IAsyncResult asyncResult)
        {
            try
            {
                isConnect = false;
                var socket = asyncResult.AsyncState as Socket;
                socket.EndConnect(asyncResult);
                isConnect = true;

            }
            catch(Exception e)
            {
                socektException = e;
            }
            finally
            {
                timeOutObject.Set();
            }
        }
    }
}
