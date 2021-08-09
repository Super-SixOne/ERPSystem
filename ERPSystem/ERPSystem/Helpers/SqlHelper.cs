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

        public static async Task<OrderHeaderCollection> GetLastOrdersAsync(int number, CancellationToken cancellationToken)
        {
            var orders = new OrderHeaderCollection();

            var table = await GetDataAsync($"SELECT TOP {(number < 1 ? 3 : number)} * FROM OrderHeader ORDER BY CreationDate DESC", cancellationToken);

            foreach (DataRow row in table.Rows)
            {
                var order = await MapToOrderAsync(row, true, cancellationToken);

                orders.Add(order);
            }

            return orders;
        }

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
            return await ExecuteNonQueryAsync($"DELETE FROM Customer WHERE CustomerNo='{customerNo}'", cancellationToken);
        }

        public static async Task<CustomerCollection> GetCustomersAsync(CancellationToken cancellationToken)
        {
            var customers = new CustomerCollection();

            var table = await GetDataAsync("SELECT * FROM Customer", cancellationToken);

            foreach (DataRow row in table.Rows)
            {
                customers.Add(MapToCustomer(row));
            }

            return customers;
        }

        public static async Task<Customer> GetCustomerAsync(string customerNo, CancellationToken cancellationToken)
        {
            var table = await GetDataAsync($"SELECT * FROM Customer WHERE CustomerNo='{customerNo}'", cancellationToken);

            foreach (DataRow row in table.Rows)
            {
                return MapToCustomer(row);
            }

            return null;
        }

        #endregion

        #region CRUD Order / Items

        public static async Task<int> AddOrderAsync(OrderHeader order, CancellationToken cancellationToken)
        {
            if (order != null)
            {
                var sql = new StringBuilder();

                sql.Append($"INSERT INTO OrderHeader (OrderNo,CustomerNo,CreationDate,Status,Carrier,Sequence) VALUES (");
                sql.Append($"'{order.OrderNo}',");
                sql.Append($"'{order.CustomerNo}',");
                sql.Append($"'{order.CreationDate}',");
                sql.Append($"'{order.Status}',");
                sql.Append($"'{order.Carrier}',");
                sql.Append($"{order.Sequence}");
                sql.Append(")");

                var updates = await ExecuteNonQueryAsync(sql.ToString(), cancellationToken);

                if (order.Items != null)
                {
                    foreach (var item in order.Items)
                    {
                        await AddOrderItemAsync(item, cancellationToken);
                    }
                }

                return updates;
            }

            return 0;
        }

        public static async Task<int> UpdateOrderAsync(OrderHeader order, CancellationToken cancellationToken)
        {
            if (order != null)
            {
                var sql = new StringBuilder();

                sql.Append($"UPDATE OrderHeader ");
                sql.Append($"SET CustomerNo='{order.CustomerNo}',");
                sql.Append($"Status='{order.Status}',");
                sql.Append($"Carrier='{order.Carrier}',");
                sql.Append($"Sequence={order.Sequence} ");
                sql.Append($"WHERE OrderNo='{order.OrderNo}'");

                var updates = await ExecuteNonQueryAsync(sql.ToString(), cancellationToken);

                if (order.Items != null)
                {
                    foreach (var item in order.Items)
                    {
                        await UpdateOrderItemAsync(item, cancellationToken);
                    }
                }

                return updates;
            }

            return 0;
        }
        public static async Task<int> DeleteOrderAsync(string orderNo, CancellationToken cancellationToken)
        {
            await ExecuteNonQueryAsync($"DELETE FROM OrderItem WHERE OrderNo='{orderNo}'", cancellationToken);
            return await ExecuteNonQueryAsync($"DELETE FROM OrderHeader WHERE OrderNo='{orderNo}'", cancellationToken);
        }

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
                var order = await MapToOrderAsync(row, true, cancellationToken);

                orders.Add(order);
            }

            return orders;
        }

        public static async Task<int> AddOrderItemAsync(OrderItem item, CancellationToken cancellationToken)
        {
            if (item != null)
            {
                var sql = new StringBuilder();

                sql.Append($"INSERT INTO OrderItem (OrderNo,OrderPos,Material,Status,TargetQuantity,CurrentQuantity,NOKQuantity) VALUES (");
                sql.Append($"'{item.OrderNo}',");
                sql.Append($"'{item.OrderPos}',");
                sql.Append($"'{item.Material}',");
                sql.Append($"'{item.Status}',");
                sql.Append($"{item.TargetQuantity},");
                sql.Append($"{item.CurrentQuantity},");
                sql.Append($"{item.NOKQuantity}");
                sql.Append(")");

                return await ExecuteNonQueryAsync(sql.ToString(), cancellationToken);
            }

            return 0;
        }

        public static async Task<int> UpdateOrderItemAsync(OrderItem item, CancellationToken cancellationToken)
        {
            if (item != null)
            {
                var sql = new StringBuilder();

                sql.Append($"UPDATE OrderItem ");
                sql.Append($"SET Material='{item.Material}',");
                sql.Append($"Status='{item.Status}',");
                sql.Append($"TargetQuantity={item.TargetQuantity},");
                sql.Append($"CurrentQuantity={item.CurrentQuantity},");
                sql.Append($"NOKQuantity={item.NOKQuantity} ");
                sql.Append($"WHERE OrderNo='{item.OrderNo}' AND OrderPos='{item.OrderPos}'");

                return await ExecuteNonQueryAsync(sql.ToString(), cancellationToken);
            }

            return 0;
        }

        public static async Task<int> DeleteOrderItemAsync(string orderNo, string orderPos, CancellationToken cancellationToken)
        {
            return await ExecuteNonQueryAsync($"DELETE FROM OrderItem WHERE OrderNo='{orderNo}' AND OrderPos='{orderPos}'", cancellationToken);
        }

        public static async Task<OrderItemCollection> GetOrderItemsAsync(string orderNo, CancellationToken cancellationToken)
        {
            var items = new OrderItemCollection();

            var table = await GetDataAsync($"SELECT * FROM OrderItem WHERE OrderNo='{orderNo}'", cancellationToken);

            foreach (DataRow row in table.Rows)
            {
                items.Add(MapToOrderItem(row));
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

        public static Customer MapToCustomer(DataRow row)
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

        public static OrderHeader MapToOrder(DataRow row)
        {
            var order = new OrderHeader();

            order.OrderNo = (string)row["OrderNo"];
            order.CustomerNo = (string)row["CustomerNo"];
            order.CreationDate = (DateTime)row["CreationDate"];
            order.Status = (string)row["Status"];
            order.Carrier = (string)row["Carrier"];
            order.Sequence = (int)row["Sequence"];

            return order;
        }

        public static async Task<OrderHeader> MapToOrderAsync(DataRow row, bool loadItems, CancellationToken cancellationToken)
        {
            var order = new OrderHeader();

            order.OrderNo = (string)row["OrderNo"];
            order.CustomerNo = (string)row["CustomerNo"];
            order.CreationDate = (DateTime)row["CreationDate"];
            order.Status = (string)row["Status"];
            order.Carrier = (string)row["Carrier"];
            order.Sequence = (int)row["Sequence"];

            if (loadItems)
            {
                order.Items = await GetOrderItemsAsync(order.OrderNo, cancellationToken);
            }

            return order;
        }

        public static OrderItem MapToOrderItem(DataRow row)
        {
            var item = new OrderItem();

            item.OrderNo = (string)row["OrderNo"];
            item.OrderPos = (string)row["OrderPos"];
            item.Material = (string)row["Material"];
            item.Status = (string)row["Status"];
            item.TargetQuantity = (int)row["TargetQuantity"];
            item.CurrentQuantity = (int)row["CurrentQuantity"];
            item.NOKQuantity = (int)row["NOKQuantity"];

            return item;
        }

        #endregion
    }
}
