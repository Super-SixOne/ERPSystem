using ERPSystem.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace ERPSystem.Helpers
{
    static class SqlHelper
    {
        public const string ConnectionString = "Data Source=demo.peakboard.rocks;Initial Catalog=AMZDB;User ID=AMZAdmin;Password=Gengenbach2021";

        public static async Task<OrderHeaderCollection> GetOrdersAsync(CancellationToken cancellationToken)
        {
            var orders = new OrderHeaderCollection();

            var table = await GetDataAsync("SELECT * FROM OrderHeader", cancellationToken);

            return orders;
        }

        public static async Task<OrderItemCollection> GetOrderItemsAsync(CancellationToken cancellationToken)
        {
            var orderItems = new OrderItemCollection();

            var table = await GetDataAsync("SELECT * FROM OrderItems", cancellationToken);

            return orderItems;
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


        public static Task<int> ExecuteNonQueryAsync(string statement)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = new SqlCommand(statement, connection))
                {
                    connection.Open();
                    return command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
