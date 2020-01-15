using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace w9wen.OPC.UA.Client.ConsoleApp
{
    public class EntryClient
    {
        private bool autoAccept = true;
        private string endpointUrl;
        private Session session;

        public void Run()
        {
            Instance().Wait();
        }

        private async Task Instance()
        {
            endpointUrl = "opc.tcp://127.0.0.1:49320";

            //// 1 - Create an Application Configuration.

            ApplicationInstance applicationInstance = new ApplicationInstance
            {
                ApplicationName = "w9wen OPC UA Client",
                ApplicationType = ApplicationType.Client,
                ConfigSectionName = Utils.IsRunningOnMono() ? "Opc.Ua.MonoSampleClient" : "w9wen.OPC.UA"
            };

            //// Load the application configuration.
            ApplicationConfiguration appConfig = await applicationInstance.LoadApplicationConfiguration(false);

            //// Check the application certificate
            var hasAppCertificate = await applicationInstance.CheckApplicationInstanceCertificate(false, 0);

            if (!hasAppCertificate)
            {
                throw new Exception("Application instance certificate invalid!");
            }
            else
            {
                appConfig.ApplicationUri = Utils.GetApplicationUriFromCertificate
                    (appConfig.SecurityConfiguration.ApplicationCertificate.Certificate);
                if (appConfig.SecurityConfiguration.AutoAcceptUntrustedCertificates)
                {
                    autoAccept = true;
                }
                appConfig.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);
            }

            //// 2 - Discover endpoints of Server
            var selectedEndpoint = CoreClientUtils.SelectEndpoint(endpointUrl, hasAppCertificate, 15000);
            Console.WriteLine("    Selected endpoint uses: {0}",
                selectedEndpoint.SecurityPolicyUri.Substring(selectedEndpoint.SecurityPolicyUri.LastIndexOf('#') + 1));

            //// 3 - Create a session with OPC UA server
            var endpointConfig = EndpointConfiguration.Create(appConfig);
            var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfig);
            this.session = await Session.Create(appConfig, endpoint, false, "w9wen OPC UA Client", 60000,
                new UserIdentity(new AnonymousIdentityToken()), null);

            //// 4 - Browse the OPC UA server
            ReferenceDescriptionCollection references;
            Byte[] continuationPoint;

            references = session.FetchReferences(ObjectIds.ObjectsFolder);

            session.Browse(
                null,
                null,
                ObjectIds.ObjectsFolder,
                0u,
                BrowseDirection.Forward,
                ReferenceTypeIds.HierarchicalReferences,
                true,
                (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
                out continuationPoint,
                out references);

            //Console.WriteLine(" DisplayName, BrowseName, NodeClass");

            foreach (var rd in references)
            {
                //Console.WriteLine(" {0}, {1}, {2}", rd.DisplayName, rd.BrowseName, rd.NodeClass);
                ReferenceDescriptionCollection nextRefs;
                byte[] nextCp;
                session.Browse(
                    null,
                    null,
                    ExpandedNodeId.ToNodeId(rd.NodeId, session.NamespaceUris),
                    0u,
                    BrowseDirection.Forward,
                    ReferenceTypeIds.HierarchicalReferences,
                    true,
                    (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
                    out nextCp,
                    out nextRefs);

                foreach (var nextRd in nextRefs)
                {
                    //Console.WriteLine("   + {0}, {1}, {2}", nextRd.DisplayName, nextRd.BrowseName, nextRd.NodeClass);

                    ReferenceDescriptionCollection nextRefs1;
                    byte[] nextCp1;
                    session.Browse(
                       null,
                       null,
                       ExpandedNodeId.ToNodeId(nextRd.NodeId, session.NamespaceUris),
                       0u,
                       BrowseDirection.Forward,
                       ReferenceTypeIds.HierarchicalReferences,
                       true,
                       (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
                       out nextCp1,
                       out nextRefs1);

                    foreach (var nextRd1 in nextRefs1)
                    {
                        //Console.WriteLine("   + + {0}, {1}, {2}", nextRd1.DisplayName, nextRd1.BrowseName, nextRd1.NodeClass);

                        if (rd.DisplayName.Text.StartsWith("Channel1") &&
                            nextRd.DisplayName.Text.StartsWith("Device1") &&
                            !nextRd1.DisplayName.Text.StartsWith("_"))
                        {
                            try
                            {
                                var _node = ExpandedNodeId.ToNodeId(nextRd1.NodeId, session.NamespaceUris);
                                DataValue dv2 = session.ReadValue(_node);
                                Console.WriteLine("   + + {0}, DataValue = [{1}]", nextRd1.DisplayName, dv2.Value);
                            }
                            catch (Exception ex)
                            {
                            }
                        }

                        ReferenceDescriptionCollection nextRefs2;
                        byte[] nextCp2;
                        session.Browse(
                           null,
                           null,
                           ExpandedNodeId.ToNodeId(nextRd1.NodeId, session.NamespaceUris),
                           0u,
                           BrowseDirection.Forward,
                           ReferenceTypeIds.HierarchicalReferences,
                           true,
                           (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
                           out nextCp2,
                           out nextRefs2);

                        foreach (var nextRd2 in nextRefs2)
                        {
                            //Console.WriteLine("   + + + {0}, {1}, {2}", nextRd2.DisplayName, nextRd2.BrowseName, nextRd2.NodeClass);
                        }
                    }
                }
            }
        }

        #region Methods

        private void CertificateValidator_CertificateValidation(CertificateValidator sender, CertificateValidationEventArgs e)
        {
            if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
            {
                e.Accept = autoAccept;
                if (autoAccept)
                {
                    Console.WriteLine("Accepted Certificate: {0}", e.Certificate.Subject);
                }
                else
                {
                    Console.WriteLine("Rejected Certificate: {0}", e.Certificate.Subject);
                }
            }
        }

        #endregion Methods
    }
}