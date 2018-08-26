using System;

namespace Storage.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var inventoryManagementService = new StorageService();
            inventoryManagementService.Start();

            Console.CancelKeyPress += (sender, eventArgs) => { inventoryManagementService.Stop(); };

            inventoryManagementService.WhenTerminated.Wait();
        }
    }
}
