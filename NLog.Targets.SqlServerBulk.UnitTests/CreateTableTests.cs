﻿using System;
using System.Collections.Generic;
using NLog.Config;
using NLog.Targets.SqlServerBulk.Sql;
using Xunit;

namespace NLog.Targets.SqlServerBulk.UnitTests
{
    public class CreateTableTests
    {
        [Fact]
        public void LogEventShouldCreateDatabase()
        {
            var database = new MockDatabase();

            var configuration = new LoggingConfiguration();

            var target = new SqlServerBulkTarget(database)
            {
                Name = "SqlServerBulk",
                ConnectionString = "[ConnectionString]",
                Table = "Log",
                LoggingColumns = new List<LoggingColumn>
                {
                    new LoggingColumn { Name = "Message", SqlType = SqlType.NVARCHAR, Length = 10, Layout = "${message}"}
                }
            };
            configuration.AddTarget(target);
            configuration.AddRuleForAllLevels("SqlServerBulk");
            LogManager.Configuration = configuration;

            var Logger = LogManager.GetLogger("logger");

            List<Exception> exceptions = new List<Exception>();
            Logger.Info("message");

            var expected = new[] {
                "SQL COMMAND: [ConnectionString] IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES\n" +
                "                 WHERE TABLE_CATALOG = DB_NAME()\n" +
                "                 AND TABLE_SCHEMA = @SchemaName\n" +
                "                 AND TABLE_NAME = @TableName\n" +
                "                 AND TABLE_TYPE = 'BASE TABLE')\n" +
                "  CREATE TABLE [dbo].[Log] (\n" +
                "    [Message] NVARCHAR(10) NULL)\n",

                "PARAMETER: @SchemaName=(String)dbo",

                "PARAMETER: @TableName=(String)Log",

                "BULK INSERT: [ConnectionString] Log",

                "ROW: Message=(String)message"
            };

            for (int i = 0; i < expected.Length; i++)
                Assert.Equal(expected[i], database.Log[i]);
        }

        [Fact]
        public void LogTableWithGeneratedColumnsShouldBeCreated()
        {
            var database = new MockDatabase();

            var configuration = new LoggingConfiguration();

            var target = new SqlServerBulkTarget(database)
            {
                Name = "SqlServerBulk",
                ConnectionString = "[ConnectionString]",
                Table = "Log",
                AutoGeneratedColumns = new List<GeneratedColumn>
                {
                    new GeneratedColumn { Name = "Id", SqlType = SqlType.BIGINT, IsPrimaryKey = true},
                    new GeneratedColumn { Name = "Uuid", SqlType = SqlType.UNIQUEIDENTIFIER },
                    new GeneratedColumn { Name = "Timestamp", SqlType = SqlType.DATETIME2, Precision = 6 },
                },
                LoggingColumns = new List<LoggingColumn>
                {
                    new LoggingColumn { Name = "Message", SqlType = SqlType.NVARCHAR, Length = 10, Layout = "${message}"}
                }

            };
            configuration.AddTarget(target);
            configuration.AddRuleForAllLevels("SqlServerBulk");
            LogManager.Configuration = configuration;

            var Logger = LogManager.GetLogger("logger");

            Logger.Info("message");

            var expected = new[] {
                "SQL COMMAND: [ConnectionString] IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES\n" +
                "                 WHERE TABLE_CATALOG = DB_NAME()\n" +
                "                 AND TABLE_SCHEMA = @SchemaName\n" +
                "                 AND TABLE_NAME = @TableName\n" +
                "                 AND TABLE_TYPE = 'BASE TABLE')\n" +
                "  CREATE TABLE [dbo].[Log] (\n" +
                "    [Id] BIGINT IDENTITY PRIMARY KEY,\n" +
                "    [Uuid] UNIQUEIDENTIFIER DEFAULT(NEWID()),\n" +
                "    [Timestamp] DATETIME2(6) DEFAULT(GETUTCDATE()),\n" +
                "    [Message] NVARCHAR(10) NULL)\n",

                "PARAMETER: @SchemaName=(String)dbo",

                "PARAMETER: @TableName=(String)Log",

                "BULK INSERT: [ConnectionString] Log",

                "ROW: Message=(String)message"
            };

            for (int i = 0; i < expected.Length; i++)
                Assert.Equal(expected[i], database.Log[i]);
        }
    }
}
