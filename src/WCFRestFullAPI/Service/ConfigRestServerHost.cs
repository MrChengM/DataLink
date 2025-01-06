using System.ServiceModel;


namespace WCFRestFullAPI.Service
{
  public class ConfigRestServerHost
    {
        private ServiceHost _restHost;
        private ConfigRestService _restService;

        public ConfigRestService RestService => _restService;

        public void Open()
        {
            _restService = new ConfigRestService();
            //restServer.Config = _config;

            //_restHost = new ServiceHost(restServer, new Uri(_restServerAddress));
            _restHost = new ServiceHost(_restService);
            //_restHost = new ServiceHost(typeof(ConfigRestService));
            //_restHost.AddServiceEndpoint(typeof(IConfigRestService), new WebHttpBinding() { MaxReceivedMessageSize= 2147483647 ,MaxBufferSize= 2147483647 }, "").Behaviors.Add(new WebHttpBehavior() {});
            //_restHost.Open();
        }

        public void close()
        {
            _restHost.Close();
        }
    }
}
