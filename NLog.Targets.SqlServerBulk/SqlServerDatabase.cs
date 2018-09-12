using System.Data;
using System.Data.SqlClient;
using NLog.Targets.SqlServerBulk.Sql;

namespace NLog.Targets.SqlServerBulk
{
    public class SqlServerDatabase : IDatabase
    {
        public void ExecuteSqlCommand(string connectionString, SqlCommand cmd)
        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 30;
                cmd.ExecuteNonQuery();
            }
        }

        public void ExecuteBulkInsert(string connectionString, string schema, string table, int batchSize, DataTable dataTable)
        {
            using (var bc = new SqlBulkCopy(connectionString))
            {
                bc.DestinationTableName = $"{schema.SqlQuote()}.{table.SqlQuote()}";
                bc.BatchSize = batchSize;
                bc.BulkCopyTimeout = 300; // TODO: make this configurable

                foreach (DataColumn c in dataTable.Columns)
                    bc.ColumnMappings.Add(c.ColumnName, c.ColumnName);

                bc.WriteToServer(dataTable);
            }
        }
    }
}
