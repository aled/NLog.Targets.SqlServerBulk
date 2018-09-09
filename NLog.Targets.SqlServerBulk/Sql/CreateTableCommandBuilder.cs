using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace NLog.Targets.SqlServerBulk.Sql
{
    public class CreateTableCommandBuilder
    {
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public IList<Column> Columns { get; set; } = new List<Column>();

        public CreateTableCommandBuilder(string SchemaName, String TableName)
        {
            this.SchemaName = SchemaName;
            this.TableName = TableName;
        }

        internal void AddGeneratedColumn(GeneratedColumn c)
        {
            bool isIdentity = false;
            string defaultValue = null;

            switch (c.SqlType)
            {
                case SqlType.INT:
                case SqlType.BIGINT:
                    isIdentity = true;
                    break;

                case SqlType.UNIQUEIDENTIFIER:
                    defaultValue = "NEWID()";
                    break;

                case SqlType.DATE:
                case SqlType.SMALLDATETIME:
                case SqlType.DATETIME:
                case SqlType.DATETIME2:
                case SqlType.DATETIMEOFFSET:
                    defaultValue = "GETUTCDATE()";
                    break;

                default:
                    throw new Exception($"Invalid data type for auto-generated column: {c.SqlType}");
            }

            Columns.Add(new Column
            {
                Name = c.Name,
                SqlType = c.SqlType,
                Precision = c.Precision,
                IsIdentity = isIdentity,
                IsPrimaryKey = c.IsPrimaryKey,
                DefaultValue = defaultValue,
                IsNullable = false
            });
        }

        public void AddLoggingColumn(LoggingColumn c)
        {
            Columns.Add(new Column
            {
                Name = c.Name,
                SqlType = c.SqlType,
                IsIdentity = false,
                DefaultValue = null,
                IsNullable = true,
                Length = c.Length,
                Scale = c.Scale,
                Precision = c.Precision
            });
        }

        public SqlCommand BuildSqlCommand()
        {
            var cmd = new SqlCommand(
                "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES\n" +
                "                 WHERE TABLE_CATALOG = DB_NAME()\n" +
                "                 AND TABLE_SCHEMA = @SchemaName\n" +
                "                 AND TABLE_NAME = @TableName\n" +
                "                 AND TABLE_TYPE = 'BASE TABLE')\n" +
                $"  CREATE TABLE {SchemaName.SqlQuote()}.{TableName.SqlQuote()} (\n    " +
                string.Join(",\n    ", Columns) + ")\n");

            cmd.Parameters.AddWithValue("@SchemaName", SchemaName);
            cmd.Parameters.AddWithValue("@TableName", TableName);

            return cmd;
        }
    }
}
