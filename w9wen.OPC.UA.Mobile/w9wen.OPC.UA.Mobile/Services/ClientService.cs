using Opc.Ua;
using Opc.Ua.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace w9wen.OPC.UA.Mobile.Services
{
    /// <summary>
    ///
    /// </summary>
    public class ClientService
    {
        public async Task CreateCertificate()
        {
            ApplicationInstance applicationInstance = new ApplicationInstance
            {
                ApplicationName = "w9wen OPC UA Client",
                ApplicationType = ApplicationType.Client,
            };
        }
    }
}