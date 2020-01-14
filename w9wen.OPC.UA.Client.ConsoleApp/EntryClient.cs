using Opc.Ua;
using Opc.Ua.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace w9wen.OPC.UA.Client.ConsoleApp
{
    public class EntryClient
    {
        public void Run()
        {
            Instance().Wait();
        }

        private async Task Instance()
        {
            Console.WriteLine("aaa");

            ApplicationInstance application = new ApplicationInstance
            {
                ApplicationName = "OPC UA Client",
                ApplicationType = ApplicationType.Client,
                ConfigSectionName = Utils.IsRunningOnMono() ? "Opc.Ua.MonoSampleClient" : "Opc.Ua.SampleClient"
            };

            // load the application configuration.
            ApplicationConfiguration config = await application.LoadApplicationConfiguration(false);
        }
    }
}