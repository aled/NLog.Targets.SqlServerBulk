using System.Data;
using System.Data.SqlClient;

namespace NLog.Targets.SqlServerBulk
{
    public interface IDatabase
    {
        void ExecuteSqlCommand(string connectionString, SqlCommand cmd);
        void ExecuteBulkInsert(string connectionString, string schema, string table, int batchSize, DataTable dataTable);
    }
}
