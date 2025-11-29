using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace WarehouseBD
{
    public static class DatabaseHelper
    {
        private static readonly string connectionString =
            @"Server=localhost\SQLEXPRESS;Database=WarehouseDB;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        public static DataTable GetStock()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = @"
                    SELECT 
                        t.Наименование,
                        ISNULL(SUM(i.Количество), 0) - ISNULL(SUM(e.Количество), 0) AS Наличие
                    FROM Товар t
                    LEFT JOIN Приход i ON t.ID = i.ТоварID
                    LEFT JOIN Расход e ON t.ID = e.ТоварID
                    GROUP BY t.ID, t.Наименование
                    ORDER BY t.Наименование";
                var adapter = new SqlDataAdapter(sql, conn);
                var table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        public static DataTable GetProducts()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT ID, Наименование FROM Товар ORDER BY Наименование";
                var adapter = new SqlDataAdapter(sql, conn);
                var table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        public static bool IsProductNameExists(string name, int? excludeId = null)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM Товар WHERE Наименование = @Name";
                if (excludeId.HasValue)
                    sql += " AND ID != @ExcludeId";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    if (excludeId.HasValue)
                        cmd.Parameters.AddWithValue("@ExcludeId", excludeId.Value);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        // Добавить товар
        public static void AddProduct(string name)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("INSERT INTO Товар (Наименование) VALUES (@Name)", conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Обновить товар
        public static void UpdateProduct(int id, string name)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("UPDATE Товар SET Наименование = @Name WHERE ID = @ID", conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Удалить товар
        public static void DeleteProduct(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("DELETE FROM Товар WHERE ID = @ID", conn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Получить записи (Приход или Расход)
        public static DataTable GetRecords(string table, DateTime? from = null, DateTime? to = null)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = $@"
                    SELECT r.ID, r.Дата, t.Наименование AS Товар, r.Количество
                    FROM {table} r
                    JOIN Товар t ON r.ТоварID = t.ID
                    WHERE 1=1";

                if (from.HasValue)
                    sql += " AND r.Дата >= @From";
                if (to.HasValue)
                    sql += " AND r.Дата <= @To";

                sql += " ORDER BY r.Дата DESC";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    if (from.HasValue) cmd.Parameters.AddWithValue("@From", from.Value);
                    if (to.HasValue) cmd.Parameters.AddWithValue("@To", to.Value);

                    var adapter = new SqlDataAdapter(cmd);
                    var tableData = new DataTable();
                    adapter.Fill(tableData);
                    return tableData;
                }
            }
        }
        // Удалить запись (Приход или Расход)
        public static void DeleteRecord(string table, int id)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand($"DELETE FROM {table} WHERE ID = @ID", conn);
            cmd.Parameters.AddWithValue("@ID", id);
            cmd.ExecuteNonQuery();
        }
        // Добавить/обновить запись (Приход или Расход)
        public static void AddOrUpdateRecord(string table, int? id, DateTime date, int productId, int quantity)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql;
                SqlParameter[] parameters;

                if (id.HasValue)
                {
                    sql = $"UPDATE {table} SET Дата = @Date, ТоварID = @ProductID, Количество = @Quantity WHERE ID = @ID";
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@ID", id.Value),
                        new SqlParameter("@Date", date),
                        new SqlParameter("@ProductID", productId),
                        new SqlParameter("@Quantity", quantity)
                    };
                }
                else
                {
                    sql = $"INSERT INTO {table} (Дата, ТоварID, Количество) VALUES (@Date, @ProductID, @Quantity)";
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@Date", date),
                        new SqlParameter("@ProductID", productId),
                        new SqlParameter("@Quantity", quantity) 
                        };
                }
            }
        }
    }
}