using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace NLog.Targets.SqlServerBulk.UnitTests
{
    public class MockDatabase : IDatabase
    {
        public IList<string> Log = new List<string>();

        public void ExecuteSqlCommand(string connectionString, SqlCommand cmd)
        {
            Log.Add($"SQL COMMAND: {connectionString} {cmd.CommandText}");
            foreach (SqlParameter p in cmd.Parameters)
                Log.Add($"PARAMETER: {p.ParameterName}=({p.Value.GetType().Name}){p.Value}");
        }

        public void ExecuteBulkInsert(string connectionString, string schema, string table, int batchSize, DataTable dataTable)
        {
            Log.Add($"BULK INSERT: {connectionString} {table}");

            foreach (DataRow row in dataTable.Rows)
            {
                var sb = new StringBuilder();

                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    if (i > 0)
                        sb.Append(" ");

                    var column = dataTable.Columns[i];
                    var dataType = row[column].GetType().Name;

                    var value =
                        row[column] is DateTime ? ((DateTime)row[column]).ToString("yyyy-MM-ddTHH:mm:ss.fffffff") :
                        row[column] is DateTimeOffset ? ((DateTimeOffset)row[column]).ToString("yyyy-MM-ddTHH:mm:ss.fffffffK") :
                        Convert.ToString(row[column]);

                    sb.Append($"{column}=({dataType}){value}");
                }

                Log.Add($"ROW: {sb}");
            }
        }
    }
}
