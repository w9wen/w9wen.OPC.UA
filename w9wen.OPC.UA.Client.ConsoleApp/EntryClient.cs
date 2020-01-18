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

            Console.WriteLine(" DisplayName, BrowseName, NodeClass");

            foreach (var rd in references)
            {
                Console.WriteLine(" {0}, {1}, {2}", rd.DisplayName, rd.BrowseName, rd.NodeClass);

                if (rd.DisplayName.Text == "Channel2")
                {
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
                        Console.WriteLine("   + {0}, {1}, {2}", nextRd.DisplayName, nextRd.BrowseName, nextRd.NodeClass);

                        if (nextRd.DisplayName.Text == "Device1")
                        {
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
                                Console.WriteLine("   + + {0}, {1}, {2}", nextRd1.DisplayName, nextRd1.BrowseName, nextRd1.NodeClass);

                                if (nextRd1.DisplayName.Text == "ICONICS_DataWorX")
                                {
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
                                        Console.WriteLine("   + + + {0}, {1}, {2}", nextRd2.DisplayName, nextRd2.BrowseName, nextRd2.NodeClass);

                                        if (nextRd2.DisplayName.Text == "DataWorX")
                                        {
                                            ReferenceDescriptionCollection nextRefs3;
                                            byte[] nextCp3;
                                            session.Browse(
                                               null,
                                               null,
                                               ExpandedNodeId.ToNodeId(nextRd2.NodeId, session.NamespaceUris),
                                               0u,
                                               BrowseDirection.Forward,
                                               ReferenceTypeIds.HierarchicalReferences,
                                               true,
                                               (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
                                               out nextCp3,
                                               out nextRefs3);

                                            foreach (var nextRd3 in nextRefs3)
                                            {
                                                Console.WriteLine("   + + + + {0}, {1}, {2}", nextRd3.DisplayName, nextRd3.BrowseName, nextRd3.NodeClass);

                                                if (nextRd3.DisplayName.Text == "Registers")
                                                {
                                                    ReferenceDescriptionCollection nextRefs4;
                                                    byte[] nextCp4;
                                                    session.Browse(
                                                       null,
                                                       null,
                                                       ExpandedNodeId.ToNodeId(nextRd3.NodeId, session.NamespaceUris),
                                                       0u,
                                                       BrowseDirection.Forward,
                                                       ReferenceTypeIds.HierarchicalReferences,
                                                       true,
                                                       (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
                                                       out nextCp4,
                                                       out nextRefs4);

                                                    foreach (var nextRd4 in nextRefs4)
                                                    {
                                                        Console.WriteLine("   + + + + + {0}, {1}, {2}", nextRd4.DisplayName, nextRd4.BrowseName, nextRd4.NodeClass);
                                                        if (nextRd4.DisplayName.Text == "ARCH")
                                                        {
                                                            ReferenceDescriptionCollection nextRefs5;
                                                            byte[] nextCp5;
                                                            session.Browse(
                                                               null,
                                                               null,
                                                               ExpandedNodeId.ToNodeId(nextRd4.NodeId, session.NamespaceUris),
                                                               0u,
                                                               BrowseDirection.Forward,
                                                               ReferenceTypeIds.HierarchicalReferences,
                                                               true,
                                                               (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
                                                               out nextCp5,
                                                               out nextRefs5);

                                                            foreach (var nextRd5 in nextRefs5)
                                                            {
                                                                Console.WriteLine("   + + + + + + {0}, {1}, {2}", nextRd5.DisplayName, nextRd5.BrowseName, nextRd5.NodeClass);

                                                                if (nextRd5.DisplayName.Text == "PA3000")
                                                                {
                                                                    ReferenceDescriptionCollection nextRefs6;
                                                                    byte[] nextCp6;
                                                                    session.Browse(
                                                                       null,
                                                                       null,
                                                                       ExpandedNodeId.ToNodeId(nextRd5.NodeId, session.NamespaceUris),
                                                                       0u,
                                                                       BrowseDirection.Forward,
                                                                       ReferenceTypeIds.HierarchicalReferences,
                                                                       true,
                                                                       (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
                                                                       out nextCp6,
                                                                       out nextRefs6);

                                                                    foreach (var nextRd6 in nextRefs6)
                                                                    {
                                                                        Console.WriteLine("   + + + + + + + {0}, {1}, {2}", nextRd6.DisplayName, nextRd6.BrowseName, nextRd6.NodeClass);

                                                                        try
                                                                        {
                                                                            var _node = ExpandedNodeId.ToNodeId(nextRd6.NodeId, session.NamespaceUris);

                                                                            DataValue dv2 = session.ReadValue(_node);
                                                                            Console.WriteLine("   + + {0}, DataValue = [{1}]", nextRd5.DisplayName, dv2.Value);

                                                                            ////var node = client.FindNode("SomeTag.SomeChildTag");

                                                                            ////// Find out what the type is before you try to get the value
                                                                            ////Type type = client.GetDataType(node.Tag);
                                                                            ////// If you find out it's a UInt32 then you use it.
                                                                            ////var value = client.Read<UInt32>(node.Tag).Value; }
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            Console.WriteLine("Exception: {0}，{1}", ex.Message, ex.StackTrace);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (rd.DisplayName.Text == "Channel1")
                {
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
                        Console.WriteLine("   + {0}, {1}, {2}", nextRd.DisplayName, nextRd.BrowseName, nextRd.NodeClass);

                        if (nextRd.DisplayName.Text == "Device1")
                        {
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
                                Console.WriteLine("   + + {0}, {1}, {2}", nextRd1.DisplayName, nextRd1.BrowseName, nextRd1.NodeClass);

                                try
                                {
                                    var _node = ExpandedNodeId.ToNodeId(nextRd1.NodeId, session.NamespaceUris);
                                    DataValue dv2 = session.ReadValue(_node);
                                    Console.WriteLine("   + + {0}, DataValue = [{1}]", nextRd1.DisplayName, dv2.Value);

                                    //var node = client.FindNode("SomeTag.SomeChildTag");

                                    //// Find out what the type is before you try to get the value
                                    //Type type = client.GetDataType(node.Tag);
                                    //// If you find out it's a UInt32 then you use it.
                                    //var value = client.Read<UInt32>(node.Tag).Value;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Exception: " + ex.Message);
                                }
                            }
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