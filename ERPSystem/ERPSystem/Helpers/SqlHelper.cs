using ERPSystem.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERPSystem.Helpers
{
    static class SqlHelper
    {
        public const string ConnectionString = "Data Source=demo.peakboard.rocks;Initial Catalog=AMZDB;User ID=AMZAdmin;Password=Gengenbach2021";

        #region CRUD Customer

        public static async Task<int> AddCustomerAsync(Customer customer, CancellationToken cancellationToken)
        {
            if (customer != null)
            {
                var sql = new StringBuilder();

                sql.Append($"INSERT INTO Customer (CustomerNo,CustomerName,Streetaddress,City,Country,VIP) VALUES (");
                sql.Append($"'{customer.CustomerNo}',");
                sql.Append($"'{customer.CustomerName}',");
                sql.Append($"'{customer.Streetaddress}',");
                sql.Append($"'{customer.City}',");
                sql.Append($"'{customer.Country}',");
                sql.Append($"{(customer.VIP ? "1" : "0")}");
                sql.Append(")");

                return await ExecuteNonQueryAsync(sql.ToString(), cancellationToken);
            }

            return 0;
        }

        public static async Task<int> UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken)
        {
            if (customer != null)
            {
                var sql = new StringBuilder();

                sql.Append($"UPDATE Customer ");
                sql.Append($"SET CustomerName='{customer.CustomerName}',");
                sql.Append($"Streetaddress='{customer.Streetaddress}',");
                sql.Append($"City='{customer.City}',");
                sql.Append($"Country='{customer.Country}',");
                sql.Append($"VIP={(customer.VIP ? "1" : "0")} ");
                sql.Append($"WHERE CustomerNo = '{customer.CustomerNo}'");

                return await ExecuteNonQueryAsync(sql.ToString(), cancellationToken);
            }

            return 0;
        }

        public static async Task<int> DeleteCustomerAsync(string customerNo, CancellationToken cancellationToken)
        {
            return await ExecuteNonQueryAsync($"DELETE FROM Customer WHERE CustomerNo = '{customerNo}'", cancellationToken);
        }

        public static async Task<CustomerCollection> GetCustomersAsync(CancellationToken cancellationToken)
        {
            var customers = new CustomerCollection();

            var table = await GetDataAsync("SELECT * FROM Customer", cancellationToken);

            foreach (DataRow row in table.Rows)
            {
                var customer = new Customer();

                customer.CustomerNo = (string)row["CustomerNo"];
                customer.CustomerName = (string)row["CustomerName"];
                customer.Streetaddress = (string)row["Streetaddress"];
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
                customer.Streetaddress = (string)row["Streetaddress"];
                customer.City = (string)row["City"];
                customer.Country = (string)row["Country"];
                customer.VIP = (bool)row["VIP"];

                return customer;
            }

            return null;
        }

        #endregion

        #region CRUD Order / Items

        public static Task<OrderHeaderCollection> GetOrdersAsync(CancellationToken cancellationToken)
        {
            return GetOrdersAsync(null, cancellationToken);
        }

        public static async Task<OrderHeaderCollection> GetOrdersAsync(string customerNo, CancellationToken cancellationToken)
        {
            var orders = new OrderHeaderCollection();

            var where = customerNo != null ? $" WHERE CustomerNo='{customerNo}'" : string.Empty;
            var table = await GetDataAsync($"SELECT * FROM OrderHeader{where}", cancellationToken);

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
                item.Status = (string)row["Status"];
                item.TargetQuantity = (int)row["TargetQuantity"];
                item.CurrentQuantity = (int)row["CurrentQuantity"];
                item.NOKQuantity = (int)row["NOKQuantity"];

                items.Add(item);
            }

            return items;
        }

        #endregion

        #region Helper Methods

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


        public static async Task<int> ExecuteNonQueryAsync(string statement, CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = new SqlCommand(statement, connection))
                {
                    await connection.OpenAsync();
                    return await command.ExecuteNonQueryAsync(cancellationToken);
                }
            }
        }

        #endregion
    }
}
