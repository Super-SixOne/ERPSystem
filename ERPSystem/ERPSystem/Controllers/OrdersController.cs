using ERPSystem.Helpers;
using ERPSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

                await SqlHelper.AddOrderAsync(order, CancellationToken.None);
            }
            else
            {
                await SqlHelper.UpdateOrderAsync(order, CancellationToken.None);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddEditOrder(string orderNo)
        {
            var existingOrders = await SqlHelper.GetOrdersAsync(CancellationToken.None);

            var model = existingOrders.FirstOrDefault(c => c.OrderNo == orderNo) ?? new OrderHeader();

            return PartialView("OrderDetails", model);
        }
        

        [HttpPost]
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
    }
}
