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
        public async Task<IActionResult> Orders(OrderHeader order)
        {
            var orders = await SqlHelper.GetOrdersAsync(CancellationToken.None);

            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder(OrderHeader order)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditOrder(OrderHeader order)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrder(OrderHeader order)
        {
            return View();
        }
    }
}
