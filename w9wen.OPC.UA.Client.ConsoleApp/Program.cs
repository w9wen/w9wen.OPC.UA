using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace w9wen.OPC.UA.Client.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var entryClient = new EntryClient();
            entryClient.Run();

            //var signalRConnection = new HubConnectionBuilder()
            //             .WithUrl("https://localhost:5001/OPCUAHub")
            //             .Build();

            //signalRConnection.Closed += async (error) =>
            //{
            //    await Task.Delay(new Random().Next(0, 5) * 1000);
            //    await signalRConnection.StartAsync();
            //};

            //signalRConnection.StartAsync();
        }
    }
}