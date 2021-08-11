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
            order.CreationDate = DateTime.Now;
            if (order.OrderNo == null)
            {
                var existingOrders = await SqlHelper.GetOrdersAsync(CancellationToken.None);

                int nextOrderNumber = 1;

                while (nextOrderNumber < 10000000)
                {
                    if (!existingOrders.Any(o => Convert.ToInt32(o.OrderNo) == nextOrderNumber))
                    {
                        string newOrderNumber = nextOrderNumber.ToString();
                        int length = newOrderNumber.Length;

                        for (int i = 8; i > length; i--)
                        {
                            newOrderNumber = newOrderNumber.Insert(0, "0");
                        }

                        order.OrderNo = newOrderNumber.ToString();
                        break;
                    }

                    nextOrderNumber++;
                }

                for (var i = 0; i < order.Items.Count; i++)
                {
                    order.Items[i].OrderNo = order.OrderNo;
                    order.Items[i].OrderPos = (i+1).ToString("D" + 4);
                }

                await SqlHelper.AddOrderAsync(order, CancellationToken.None);
            }
            else
            {
                // Note: I know that it is duplication. But I have to idea for now how to improve it, sorry
                
                for (var i = 0; i < order.Items.Count; i++)
                {
                    order.Items[i].OrderNo = order.OrderNo;
                    order.Items[i].OrderPos = (i+1).ToString("D" + 4);
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
            
            ViewData["ExistingMaterials"] = await GetAllMaterials();
            
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
