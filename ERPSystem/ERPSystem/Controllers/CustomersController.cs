using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERPSystem.Helpers;
using ERPSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.Controllers
{
    public class CustomersController : Controller
    {
        
        public async Task<IActionResult> Index()
        {
            var customers = await SqlHelper.GetCustomersAsync(CancellationToken.None);
            return View(customers);
        }

        [HttpPost]
        public async Task<IActionResult> AddUpdateCustomer(Customer customer)
        {
            if (customer.CustomerNo == null)
            {
                var existingCustomers = await SqlHelper.GetCustomersAsync(CancellationToken.None);

                int nextCustomerNumber = 1;

                while(nextCustomerNumber < 10000000)
                {
                    if(!existingCustomers.Any(c => Convert.ToInt32(c.CustomerNo) == nextCustomerNumber))
                    {
                        string newCustomerNumber = nextCustomerNumber.ToString();
                        int length = newCustomerNumber.Length;

                        for (int i = 8; i > length; i--)
                        {
                            newCustomerNumber = newCustomerNumber.Insert(0, "0");
                        }

                        customer.CustomerNo = newCustomerNumber.ToString();
                        break;
                    }

                    nextCustomerNumber++;
                }
            
                await SqlHelper.AddCustomerAsync(customer, CancellationToken.None);
            }
            else
            {
                await SqlHelper.UpdateCustomerAsync(customer, CancellationToken.None);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddEditCustomer(string customerNo)
        {
            var existingCustomers = await SqlHelper.GetCustomersAsync(CancellationToken.None);

            var model = existingCustomers.FirstOrDefault(c => c.CustomerNo == customerNo) ?? new Customer();

            return PartialView("CustomerDetails", model);
        }
        
        public async Task<IActionResult> DeleteCustomer(string customerNo)
        {
            await SqlHelper.DeleteCustomerAsync(customerNo, CancellationToken.None);

            return RedirectToAction("Index");
        }

    }
}