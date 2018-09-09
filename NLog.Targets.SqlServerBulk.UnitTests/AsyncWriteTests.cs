using System.Collections.Generic;
using System.Threading;
using NLog.Config;
using NLog.Targets.SqlServerBulk.Sql;
using NLog.Targets.Wrappers;
using Xunit;

namespace NLog.Targets.SqlServerBulk.UnitTests
{
    public class AsyncWriteTests
    {
        [Fact]
        public void MultipleLogsShouldBeWrittenInOneBatch()
        {
            var database = new MockDatabase();

            var configuration = new LoggingConfiguration();

            var target = new AsyncTargetWrapper
            {
                Name = "AsyncSqlServerBulk",
                BatchSize = 2,
                OverflowAction = AsyncTargetWrapperOverflowAction.Block,
                TimeToSleepBetweenBatches = 100,
                QueueLimit = 1000,
                WrappedTarget = new SqlServerBulkTarget(database)
                {
                    Name = "SqlServerBulk",
                    ConnectionString = "[ConnectionString]",
                    Table = "Log",
                    CreateTableIfNotExists = false,
                    LoggingColumns = new List<LoggingColumn>
                    {
                        new LoggingColumn { Name = "Message", SqlType = SqlType.NVARCHAR, Length = 10, Layout = "${message}"}
                    }
                }
            };

            configuration.AddTarget(target);
            configuration.AddRuleForAllLevels("AsyncSqlServerBulk");
            LogManager.Configuration = configuration;

            var Logger = LogManager.GetLogger("logger");

            Logger.Info("message1");
            Logger.Info("message2");
            Logger.Info("message3");

            var expected = new[] {
                "BULK INSERT: [ConnectionString] Log",
                "ROW: Message=(String)message1",
                "ROW: Message=(String)message2",

                "BULK INSERT: [ConnectionString] Log",
                "ROW: Message=(String)message3",
            };

            // Allow to spin for 1000ms, as the log entries are written asynchronously
            for (int i = 0; i < 100; i++)
            {
                if (database.Log.Count >= expected.Length)
                    break;

                Thread.Sleep(10);
            }

            for (int i = 0; i < expected.Length; i++)
                Assert.Equal(expected[i], database.Log[i]);
        }
    }
}
