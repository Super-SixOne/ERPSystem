using ERPSystem.Helpers;
using ERPSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace ERPSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<CustomerCollection> GetAllCustomers()
        {
            var existingCustomers = await SqlHelper.GetCustomersAsync(CancellationToken.None);
            return existingCustomers;
        }
        public async Task<IActionResult> Index()
        {
            var lastOrders = await SqlHelper.GetLastOrdersAsync(5,CancellationToken.None);
            ViewData["ExistingCustomers"] = await GetAllCustomers();
            
            return View(lastOrders);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
