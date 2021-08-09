using ERPSystem.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace ERPSystem.Helpers
{
    static class SqlHelper
    {
        public const string ConnectionString = "Data Source=demo.peakboard.rocks;Initial Catalog=AMZDB;User ID=AMZAdmin;Password=Gengenbach2021";

        public static async Task<CustomerCollection> GetCustomersAsync(CancellationToken cancellationToken)
        {
            var customers = new CustomerCollection();

            var table = await GetDataAsync("SELECT * FROM Customer", cancellationToken);

            foreach (DataRow row in table.Rows)
            {
                var customer = new Customer();

                customer.CustomerNo = (string)row["CustomerNo"];
                customer.CustomerName = (string)row["CustomerName"];
                customer.Streetaddress = (string)row["Streetadress"];
                customer.City = (string)row["City"];
                customer.Country = (string)row["Country"];
                customer.VIP = (bool)row["VIP"];

                customers.Add(customer);
            }

            return customers;
        }

        public static async Task<Customer> GetCustomerAsync(string customerNo, CancellationToken cancellationToken)
        {
            var table = await GetDataAsync($"SELECT * FROM Customer WHERE CustomerNo='{customerNo}'", cancellationToken);

            foreach (DataRow row in table.Rows)
            {
                var customer = new Customer();

                customer.CustomerNo = (string)row["CustomerNo"];
                customer.CustomerName = (string)row["CustomerName"];
                customer.Streetaddress = (string)row["Streetadress"];
                customer.City = (string)row["City"];
                customer.Country = (string)row["Country"];
                customer.VIP = (bool)row["VIP"];

                return customer;
            }

            return null;
        }

        public static async Task<OrderHeaderCollection> GetOrdersAsync(string customerNo, CancellationToken cancellationToken)
        {
            var orders = new OrderHeaderCollection();

            var table = await GetDataAsync($"SELECT * FROM OrderHeader WHERE CustomerNo='{customerNo}'", cancellationToken);

            foreach (DataRow row in table.Rows)
            {
                var order = new OrderHeader();

                order.OrderNo = (string)row["OrderNo"];
                order.CustomerNo = (string)row["CustomerNo"];
                order.CreationDate = (DateTime)row["CreationDate"];
                order.Status = (string)row["Status"];
                order.Carrier = (string)row["Carrier"];
                order.Sequence = (int)row["Sequence"];
                order.Items = await GetOrderItemsAsync(order.OrderNo, cancellationToken);

                orders.Add(order);
            }

            return orders;
        }

        public static async Task<OrderItemCollection> GetOrderItemsAsync(string orderNo, CancellationToken cancellationToken)
        {
            var items = new OrderItemCollection();

            var table = await GetDataAsync($"SELECT * FROM OrderItem WHERE OrderNo='{orderNo}'", cancellationToken);

            foreach (DataRow row in table.Rows)
            {
                var item = new OrderItem();

                item.OrderNo = (string)row["OrderNo"];
                item.OrderPos = (string)row["OrderPos"];
                item.Material = (string)row["Material"];
                item.Description = (string)row["Description"];
                item.Status = (string)row["Status"];
                item.TargetQuantity = (int)row["TargetQuantity"];
                item.CurrentQuantity = (int)row["CurrentQuantity"];

                items.Add(item);
            }

            return items;
        }

        public static async Task<DataTable> GetDataAsync(string statement, CancellationToken cancellationToken)
        {
            var table = new DataTable();

            using (var connection = new SqlConnection(ConnectionString))
            {
                using var adapter = new SqlDataAdapter(statement, connection);
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                adapter.Fill(table);
            }

            return table;
        }


        public static Task<int> ExecuteNonQueryAsync(string statement, CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = new SqlCommand(statement, connection))
                {
                    connection.Open();
                    return command.ExecuteNonQueryAsync(cancellationToken);
                }
            }
        }
    }
}
