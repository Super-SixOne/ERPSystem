using ERPSystem.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
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
                List<SqlParameter> parameters = new List<SqlParameter>()
                {
                    new SqlParameter("customerNo", customer.CustomerNo),
                    new SqlParameter("customerName", customer.CustomerName),
                    new SqlParameter("streetaddress", customer.Streetaddress),
                    new SqlParameter("city", customer.City),
                    new SqlParameter("country", customer.Country),
                    new SqlParameter("vip", customer.VIP ? "1" : "0")

                };

                var sql = new StringBuilder();

                sql.Append($"INSERT INTO Customer (CustomerNo,CustomerName,Streetaddress,City,Country,VIP) VALUES (");
                sql.Append($"@customerNo,");
                sql.Append($"@customerName,");
                sql.Append($"@streetaddress,");
                sql.Append($"@city,");
                sql.Append($"@country,");
                sql.Append($"@vip");
                sql.Append(")");

                return await ExecuteNonQueryAsync(sql.ToString(), cancellationToken , parameters);
            }

            return 0;
        }

        public static async Task<int> UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken)
        {
            if (customer != null)
            {
                List<SqlParameter> parameters = new List<SqlParameter>()
                {
                    new SqlParameter("customerName", customer.CustomerName),
                    new SqlParameter("streetaddress", customer.Streetaddress),
                    new SqlParameter("city", customer.City),
                    new SqlParameter("country", customer.Country),
                    new SqlParameter("vip", customer.VIP ? "1" : "0"),
                    new SqlParameter("customerNo", customer.CustomerNo)
                };

                var sql = new StringBuilder();

                sql.Append($"UPDATE Customer ");
                sql.Append($"SET CustomerName=@customerName,");
                sql.Append($"Streetaddress=@streetaddress,");
                sql.Append($"City=@city,");
                sql.Append($"Country=@country,");
                sql.Append($"VIP=@vip ");
                sql.Append($"WHERE CustomerNo = @customerNo");

                return await ExecuteNonQueryAsync(sql.ToString(), cancellationToken, parameters);
            }

            return 0;
        }

        public static async Task<int> DeleteCustomerAsync(string customerNo, CancellationToken cancellationToken)
        {
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter("customerNo", customerNo)
            };

            return await ExecuteNonQueryAsync($"DELETE FROM Customer WHERE CustomerNo=@customerNo", cancellationToken, parameters);
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
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter("customerNo", customerNo)

            };

            var table = await GetDataAsync($"SELECT * FROM Customer WHERE CustomerNo=@customerNo", cancellationToken, parameters);

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
                List<SqlParameter> parameters = new List<SqlParameter>()
                {
                    new SqlParameter("orderNo", order.OrderNo),
                    new SqlParameter("customerNo", order.CustomerNo),
                    new SqlParameter("creationDate", order.CreationDate),
                    new SqlParameter("status", order.Status),
                    new SqlParameter("carrier", order.Carrier),
                    new SqlParameter("sequence", order.Sequence)

                };

                var sql = new StringBuilder();

                sql.Append($"INSERT INTO OrderHeader (OrderNo,CustomerNo,CreationDate,Status,Carrier,Sequence) VALUES (");
                sql.Append($"@orderNo,");
                sql.Append($"@customerNo,");
                sql.Append($"@creationDate,");
                sql.Append($"@status,");
                sql.Append($"@carrier,");
                sql.Append($"@sequence");
                sql.Append(")");

                var updates = await ExecuteNonQueryAsync(sql.ToString(), cancellationToken, parameters);

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
                List<SqlParameter> parameters = new List<SqlParameter>()
                {
                    new SqlParameter("customerNo", order.CustomerNo),
                    new SqlParameter("status", order.Status),
                    new SqlParameter("carrier", order.Carrier),
                    new SqlParameter("sequence", order.Sequence),
                    new SqlParameter("orderNo", order.OrderNo)
                };

                var sql = new StringBuilder();

                sql.Append($"UPDATE OrderHeader ");
                sql.Append($"SET CustomerNo=@customerNo,");
                sql.Append($"Status=@status,");
                sql.Append($"Carrier=@carrier,");
                sql.Append($"Sequence=@sequence ");
                sql.Append($"WHERE OrderNo=@orderNo");

                var updates = await ExecuteNonQueryAsync(sql.ToString(), cancellationToken, parameters);

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
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter("orderNo", orderNo)
            };

            await ExecuteNonQueryAsync($"DELETE FROM OrderItem WHERE OrderNo=@orderNo", cancellationToken, parameters);
            return await ExecuteNonQueryAsync($"DELETE FROM OrderHeader WHERE OrderNo=@orderNo", cancellationToken, parameters);
        }

        public static Task<OrderHeaderCollection> GetOrdersAsync(CancellationToken cancellationToken)
        {
            return GetOrdersAsync(null, cancellationToken);
        }

        public static async Task<OrderHeaderCollection> GetOrdersAsync(string customerNo, CancellationToken cancellationToken)
        {
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter("customerNo", customerNo)
            };

            var orders = new OrderHeaderCollection();

            var where = customerNo != null ? $" WHERE CustomerNo=@customerNo" : string.Empty;
            var table = await GetDataAsync($"SELECT * FROM OrderHeader{where}", cancellationToken, parameters);

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
                List<SqlParameter> parameters = new List<SqlParameter>()
                {
                    new SqlParameter("orderNo", item.OrderNo),
                    new SqlParameter("orderPos", item.OrderPos),
                    new SqlParameter("material", item.Material),
                    new SqlParameter("status", item.Status),
                    new SqlParameter("targetQuantity", item.TargetQuantity),
                    new SqlParameter("currentQuantity", item.CurrentQuantity),
                    new SqlParameter("nokQuantity", item.NOKQuantity),
                };

                var sql = new StringBuilder();

                sql.Append($"INSERT INTO OrderItem (OrderNo,OrderPos,Material,Status,TargetQuantity,CurrentQuantity,NOKQuantity) VALUES (");
                sql.Append($"@orderNo,");
                sql.Append($"@orderPos,");
                sql.Append($"@material,");
                sql.Append($"@status,");
                sql.Append($"@targetQuantity,");
                sql.Append($"@currentQuantity,");
                sql.Append($"@nokQuantity");
                sql.Append(")");

                return await ExecuteNonQueryAsync(sql.ToString(), cancellationToken, parameters);
            }

            return 0;
        }

        public static async Task<int> UpdateOrderItemAsync(OrderItem item, CancellationToken cancellationToken)
        {
            if (item != null)
            {
                List<SqlParameter> parameters = new List<SqlParameter>()
                {
                    new SqlParameter("material", item.Material),
                    new SqlParameter("status", item.Status),
                    new SqlParameter("targetQuantity", item.TargetQuantity),
                    new SqlParameter("currentQuantity", item.CurrentQuantity),
                    new SqlParameter("nokQuantity", item.NOKQuantity),
                    new SqlParameter("orderNo", item.OrderNo),
                    new SqlParameter("orderPos", item.OrderPos)
                };

                var sql = new StringBuilder();

                sql.Append($"UPDATE OrderItem ");
                sql.Append($"SET Material=@material,");
                sql.Append($"Status=@status,");
                sql.Append($"TargetQuantity=@targetQuantity,");
                sql.Append($"CurrentQuantity=@currentQuantity,");
                sql.Append($"NOKQuantity=@nokQuantity ");
                sql.Append($"WHERE OrderNo=@orderNo AND OrderPos=@orderPos");

                return await ExecuteNonQueryAsync(sql.ToString(), cancellationToken, parameters);
            }

            return 0;
        }

        public static async Task<int> DeleteOrderItemAsync(string orderNo, string orderPos, CancellationToken cancellationToken)
        {
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter("orderNo", orderNo),
                new SqlParameter("orderPos", orderPos)
            };

            return await ExecuteNonQueryAsync($"DELETE FROM OrderItem WHERE OrderNo=@orderNo AND OrderPos=@orderPos", cancellationToken, parameters);
        }

        public static async Task<OrderItemCollection> GetOrderItemsAsync(string orderNo, CancellationToken cancellationToken)
        {
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter("orderNo", orderNo),
            };

            var items = new OrderItemCollection();

            var table = await GetDataAsync($"SELECT * FROM OrderItem WHERE OrderNo=@orderNo", cancellationToken, parameters);

            foreach (DataRow row in table.Rows)
            {
                items.Add(MapToOrderItem(row));
            }

            return items;
        }

        #endregion

        #region Helper Methods

        public static async Task<DataTable> GetDataAsync(string statement, CancellationToken cancellationToken, List<SqlParameter> parameters = null)
        {
            var table = new DataTable();

            using (var connection = new SqlConnection(ConnectionString))
            {
                using var adapter = new SqlDataAdapter(statement, connection);

                if(parameters != null)
                {
                    foreach (SqlParameter parameter in parameters)
                    {
                        adapter.SelectCommand.Parameters.Add(parameter);
                    }
                }

                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                adapter.Fill(table);
            }

            return table;
        }


        public static async Task<int> ExecuteNonQueryAsync(string statement, CancellationToken cancellationToken, List<SqlParameter> parameters = null)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = new SqlCommand(statement, connection))
                {
                    if(parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

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
