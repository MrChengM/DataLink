using GuiBase.Common;
using GuiBase.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Ioc;
using DataServer.Points;
using DataServer;
using DataServer.Log;

namespace GuiBase.Helper
{
    public static class SignalMangementHelper
    {
        //private static  readonly ISignalMangement mangementDefault = new SignalMangement(null,null);
        private static ISignalMangement GetMangement()
        {
            ISignalMangement signalMangement;
            if (ContainerLocator.Container != null)
            {
                signalMangement = ContainerLocator.Container.Resolve<ISignalMangement>();
            }
            else
            {
                signalMangement = new SignalMangement(new Log4netWrapper(), new SignalService(new Log4netWrapper()));
            }
            return signalMangement;
        }
        public static void Register(ISignalChanged host, GSignal signal)
        {
            var signalMangement = GetMangement();
            signalMangement.Register(host, signal);
        }
        public static void Register(ISignalChanged host, string signalName)
        {
            var signalMangement = GetMangement();
            signalMangement.Register(host, signalName);
        }
        public static void Register(ISignalChanged host, GSignalSet signalSet)
        {

            var signalMangement = GetMangement();
            signalMangement.Register(host, signalSet);
        }
        public static void UnRegister(ISignalChanged host, string signalName)
        {
            var signalMangement = GetMangement();
            signalMangement.UnRegister(host, signalName);
        }
        public static void UnRegister(ISignalChanged host, GSignal signal)
        {
            var signalMangement = GetMangement();
            signalMangement.UnRegister(host, signal);
        }
        public static void UnRegister(ISignalChanged host, GSignalSet signalSet)
        {
            var signalMangement = GetMangement();
            signalMangement.UnRegister(host, signalSet);
        }

        public static ITag GetTag(string name)
        {
            var signalMangement = GetMangement();
          return  signalMangement.GetTag(name);
        }

        public static WriteResult Write(SignalDTO signalDTO)
        {
            var signalMangement = GetMangement();
            return signalMangement.Write(signalDTO);
        }
        public static WriteResult Write(SignalBitDTO signalbitDTO)
        {
            var signalMangement = GetMangement();
            return signalMangement.Write(signalbitDTO);
        }
        public static void WriteAsync(SignalBitDTO signalbitDTO,Action<WriteResult> callBack)
        {
            var signalMangement = GetMangement();
             signalMangement.WriteAsync(signalbitDTO,callBack);
        }
    }
}
