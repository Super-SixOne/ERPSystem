using ERPSystem.Helpers;
using ERPSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ERPSystem.Controllers
{
    public class OrdersController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders = await SqlHelper.GetOrdersAsync(CancellationToken.None);

            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> AddUpdateOrder(OrderHeader order)
        {
            if (order.OrderNo == null)
            {
                var existingOrders = await SqlHelper.GetOrdersAsync(CancellationToken.None);

                int nextOrderNumber = 1;

                while (nextOrderNumber < 10000000)
                {
                    if (!existingOrders.Any(o => o.OrderNo == nextOrderNumber.ToString()))
                    {
                        order.OrderNo = nextOrderNumber.ToString();
                        break;
                    }

                    nextOrderNumber++;
                }

                if (order.Items != null)
                {
                    foreach (var orderItem in order.Items)
                    {
                        orderItem.OrderNo = order.OrderNo;
                    }
                }

                await SqlHelper.AddOrderAsync(order, CancellationToken.None);
            }
            else
            {
                // Note: I know that it is duplication. But I have to idea for now how to improve it, sorry
                if (order.Items != null)
                {
                    foreach (var orderItem in order.Items)
                    {
                        orderItem.OrderNo = order.OrderNo;
                    }
                }
                
                await SqlHelper.UpdateOrderAsync(order, CancellationToken.None);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddEditOrder(string orderNo)
        {
            var existingOrders = await SqlHelper.GetOrdersAsync(CancellationToken.None);
            var model = existingOrders.FirstOrDefault(c => c.OrderNo == orderNo) ?? new OrderHeader();

            ViewData["ExistingCustomers"] = await GetAllCustomers();

            ViewData["ExistingMaterials"] = await GetAllMaterials();
            
            return View("OrderEditingView", model);
        }
        

        public async Task<IActionResult> DeleteOrder(string orderNo)
        {
            await SqlHelper.DeleteOrderAsync(orderNo, CancellationToken.None);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ViewOrder(string orderNo)
        {
            var order = await SqlHelper.GetOrderAsync(orderNo, CancellationToken.None);
            return View("OrderDetailedView", order);
        }

        public async Task<CustomerCollection> GetAllCustomers()
        {
            var existingCustomers = await SqlHelper.GetCustomersAsync(CancellationToken.None);
            return existingCustomers;
        }
        public async Task<MaterialCollection> GetAllMaterials()
        {
            var existingMaterials = await SqlHelper.GetMaterialsAsync(CancellationToken.None);
            return existingMaterials;
        }
        
        public async Task<PartialViewResult> BlankEditorRow()
        {
            ViewData["ExistingMaterials"] = await GetAllMaterials();
            return PartialView("OrderItemEditorRow", new OrderItem());
        }
    }
}
