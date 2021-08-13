using ERPSystem.Models;
using System;
using System.Linq;
using System.Threading;

namespace ERPSystem.Helpers
{
    public static class OrderBot
    {
        public static void Run()
        {
            Random rand = new Random();

            // ----- CONFIG -----

            bool isActive = true;

            int orderDelay = 120000; // in ms 

            int minItems = 1; // Minimum amount of items per order
            int maxItems = 5; // Maximuim amount of items per order

            int maxTargetQuantity = 10;

            int maxSequence = 150;

            // ------------------

            while (isActive)
            {
                // Create random amount of order items.
                int numItems = rand.Next(minItems, maxItems + 1);

                var orders = SqlHelper.GetAllOrders();
                var materials = SqlHelper.GetMaterials();
                var customers = SqlHelper.GetCustomers();

                int orderNo = 0;

                // Determine order number
                for (int i = 1; i < 1000000; i++)
                {
                    if (orders.Any(o => Convert.ToInt32(o.OrderNo) == i))
                    {
                        continue;
                    }

                    orderNo = i;
                    break;
                }

                OrderItemCollection items = new OrderItemCollection();

                for (int i = 0; i < numItems; i++)
                {
                    OrderItem item = new OrderItem();

                    // Choose free orderNo
                    item.OrderNo = orderNo.ToString("D" + 8);
                    item.OrderPos = (i + 1).ToString("D" + 4);

                    // select random material number from exisiting ones
                    int matIndex = rand.Next(0, materials.Count);
                    item.MaterialNo = materials[matIndex].MaterialNo;

                    item.TargetQuantity = rand.Next(1, maxTargetQuantity + 1);

                    items.Add(item);
                }

                OrderHeader order = new OrderHeader();
                order.OrderNo = orderNo.ToString("D" + 8);
                int custIndex = rand.Next(0, customers.Count);
                order.CustomerNo = customers[custIndex].CustomerNo;
                order.CreationDate = DateTime.Now;
                order.Status = "created";
                int carrierDecider = rand.Next(0, 2);
                order.Carrier = carrierDecider == 0 ? "DHL" : "Schenker";
                order.Sequence = rand.Next(0, maxSequence + 1);
                order.Items = items;

                SqlHelper.AddOrder(order);

                Thread.Sleep(orderDelay);
            }
        }
    }
}
