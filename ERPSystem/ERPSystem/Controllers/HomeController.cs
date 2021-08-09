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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Orders()
        {
            var orders = await SqlHelper.GetOrdersAsync(CancellationToken.None);

            return View(orders);
        }
        public async Task<IActionResult> Customers()
        {
            var customers = await SqlHelper.GetCustomersAsync(CancellationToken.None);
            return View(customers);
        }

        public IActionResult AddCustomer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(Customer customer)
        {
            var existingCustomers = await SqlHelper.GetCustomersAsync(CancellationToken.None);

            int nextCustomerNumber = 1;

            while(nextCustomerNumber < 10000000)
            {
                if(!existingCustomers.Any(c => c.CustomerNo == nextCustomerNumber.ToString()))
                {
                    customer.CustomerNo = nextCustomerNumber.ToString();
                    break;
                }
            }

            await SqlHelper.AddCustomerAsync(customer, CancellationToken.None);

            return View();
        }

        public IActionResult AddOrder()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddOrder(OrderHeader order)
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}