using System;

namespace InventoryManagement.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var inventoryManagementService = new InventoryManagementService();
            inventoryManagementService.Start();

            Console.CancelKeyPress += (sender, eventArgs) => { inventoryManagementService.Stop(); };

            inventoryManagementService.WhenTerminated.Wait();
        }
    }
}
