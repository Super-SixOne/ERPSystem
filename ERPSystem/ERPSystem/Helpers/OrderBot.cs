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
            if (!OrderBotConfig.Refresh())
            {
                return;
            }
            while (OrderBotConfig.isActive)
            {
                Random rand = new Random();

                // Create random amount of order items.
                int numItems = rand.Next(OrderBotConfig.minItems, OrderBotConfig.maxItems + 1);
                
                var orders = SqlHelper.GetAllOrders();
                var materials = SqlHelper.GetMaterials();
                var customers = SqlHelper.GetCustomers();

                if(orders.Count < OrderBotConfig.maxOrderNumber)
                {
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

                        item.TargetQuantity = rand.Next(1, OrderBotConfig.maxTargetQuantity + 1);

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
                    order.Sequence = rand.Next(0, OrderBotConfig.maxSequence + 1);
                    order.Items = items;

                    SqlHelper.AddOrder(order);
                    
                }

                Thread.Sleep(OrderBotConfig.orderDelay);
                if (OrderBotConfig.Refresh())
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
