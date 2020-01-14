using System;

namespace w9wen.OPC.UA.Client.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var entryClient = new EntryClient();
            entryClient.Run();
        }
    }
}