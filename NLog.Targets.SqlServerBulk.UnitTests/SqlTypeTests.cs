using System;
using System.Collections.Generic;
using NLog.Config;
using NLog.Targets.SqlServerBulk.Sql;
using Xunit;

namespace NLog.Targets.SqlServerBulk.UnitTests
{
    public class SqlTypeTests
    {
        [Fact]
        public void MissingValuesShouldBeSetToNullOrEmptyString()
        {
            var database = new MockDatabase();
            var configuration = new LoggingConfiguration();
            var target = new SqlServerBulkTarget(database)
            {
                Name = "SqlServerBulk",
                ConnectionString = "[ConnectionString]",
                Table = "Log",
                CreateTableIfNotExists = false,
                LoggingColumns = new List<LoggingColumn>
                {
                    new LoggingColumn { Name = "Message", SqlType = SqlType.NVARCHAR, Length = 10, Layout = "${message}"},
                    new LoggingColumn { Name = "VarcharCol", SqlType = SqlType.VARCHAR, Length = 3, Layout = "${mdlc:item=varchar}"},
                    new LoggingColumn { Name = "DecimalCol", SqlType = SqlType.DECIMAL, Scale = 1, Precision = 1, Layout = "${mdlc:item=decimal}"},
                    new LoggingColumn { Name = "NumericCol", SqlType = SqlType.NUMERIC, Layout = "${mdlc:item=decimal}"},
                    new LoggingColumn { Name = "IntCol", SqlType = SqlType.INT, Layout = "${mdlc:item=int}"},
                    new LoggingColumn { Name = "BigintCol", SqlType = SqlType.BIGINT, Layout = "${mdlc:item=bigint}"},
                    new LoggingColumn { Name = "UuidCol", SqlType = SqlType.UNIQUEIDENTIFIER, Layout = "${mdlc:item=uuid}"},
                    new LoggingColumn { Name = "DateCol", SqlType = SqlType.DATE, Layout = "${mdlc:item=date}"},
                    new LoggingColumn { Name = "SmallDateTimeCol", SqlType = SqlType.SMALLDATETIME, Layout = "${mdlc:item=smalldatetime}"},
                    new LoggingColumn { Name = "DateTimeCol", SqlType = SqlType.DATETIME, Layout = "${mdlc:item=datetime}"},
                    new LoggingColumn { Name = "DateTime2Col", SqlType = SqlType.DATETIME2, Precision = 1, Layout = "${mdlc:item=datetime2}"},
                    new LoggingColumn { Name = "DateTimeOffsetCol", SqlType = SqlType.DATETIMEOFFSET, Precision = 1, Layout = "${mdlc:item=datetimeoffset}"}
                }
            };
            configuration.AddTarget(target);
            configuration.AddRuleForAllLevels("SqlServerBulk");
            LogManager.Configuration = configuration;

            var Logger = LogManager.GetLogger("logger");

            Logger.Info((string)null);

            var expected = new[] {
                "BULK INSERT: [ConnectionString] Log",
                "ROW: Message=(String) VarcharCol=(String) DecimalCol=(DBNull) NumericCol=(DBNull) IntCol=(DBNull) BigintCol=(DBNull) UuidCol=(DBNull) DateCol=(DBNull) SmallDateTimeCol=(DBNull) DateTimeCol=(DBNull) DateTime2Col=(DBNull) DateTimeOffsetCol=(DBNull)"
            };

            for (int i = 0; i < expected.Length; i++)
                Assert.Equal(expected[i], database.Log[i]);
        }

        [Fact]
        public void UnconvertibleValuesShouldBeSetToDBNullOrTrucatedIfString()
        {
            var database = new MockDatabase();
            var configuration = new LoggingConfiguration();
            var target = new SqlServerBulkTarget(database)
            {
                Name = "SqlServerBulk",
                ConnectionString = "[ConnectionString]",
                Table = "Log",
                CreateTableIfNotExists = false,
                LoggingColumns = new List<LoggingColumn>
                {
                    new LoggingColumn { Name = "Message", SqlType = SqlType.NVARCHAR, Length = 10, Layout = "${message}"},
                    new LoggingColumn { Name = "VarcharCol", SqlType = SqlType.VARCHAR, Length = 3, Layout = "${mdlc:item=varchar}"},
                    new LoggingColumn { Name = "DecimalCol", SqlType = SqlType.DECIMAL, Scale = 1, Precision = 1, Layout = "${mdlc:item=decimal}"},
                    new LoggingColumn { Name = "NumericCol", SqlType = SqlType.NUMERIC, Layout = "${mdlc:item=decimal}"},
                    new LoggingColumn { Name = "IntCol", SqlType = SqlType.INT, Layout = "${mdlc:item=int}"},
                    new LoggingColumn { Name = "BigintCol", SqlType = SqlType.BIGINT, Layout = "${mdlc:item=bigint}"},
                    new LoggingColumn { Name = "UuidCol", SqlType = SqlType.UNIQUEIDENTIFIER, Layout = "${mdlc:item=uuid}"},
                    new LoggingColumn { Name = "DateCol", SqlType = SqlType.DATE, Layout = "${mdlc:item=date}"},
                    new LoggingColumn { Name = "SmallDateTimeCol", SqlType = SqlType.SMALLDATETIME, Layout = "${mdlc:item=smalldatetime}"},
                    new LoggingColumn { Name = "DateTimeCol", SqlType = SqlType.DATETIME, Layout = "${mdlc:item=datetime}"},
                    new LoggingColumn { Name = "DateTime2Col", SqlType = SqlType.DATETIME2, Precision = 1, Layout = "${mdlc:item=datetime2}"},
                    new LoggingColumn { Name = "DateTimeOffsetCol", SqlType = SqlType.DATETIMEOFFSET, Precision = 1, Layout = "${mdlc:item=datetimeoffset}"}
                }
            };
            configuration.AddTarget(target);
            configuration.AddRuleForAllLevels("SqlServerBulk");
            LogManager.Configuration = configuration;

            var Logger = LogManager.GetLogger("logger");

            MappedDiagnosticsLogicalContext.Set("varchar", "qwertyuiop");
            MappedDiagnosticsLogicalContext.Set("decimal", "");
            MappedDiagnosticsLogicalContext.Set("numeric", "");
            MappedDiagnosticsLogicalContext.Set("int", "");
            MappedDiagnosticsLogicalContext.Set("bigint", "");
            MappedDiagnosticsLogicalContext.Set("uuid", 1);
            MappedDiagnosticsLogicalContext.Set("date", "");
            MappedDiagnosticsLogicalContext.Set("smalldatetime", new DateTime(1899, 1, 1)); // invalid
            MappedDiagnosticsLogicalContext.Set("datetime", new DateTime(1752, 12, 31)); // invalid
            MappedDiagnosticsLogicalContext.Set("datetime2", "");
            MappedDiagnosticsLogicalContext.Set("datetimeoffset", "");

            Logger.Info("hello world");

            var expected = new[] {
                "BULK INSERT: [ConnectionString] Log",
                "ROW: Message=(String)hello worl VarcharCol=(String)qwe DecimalCol=(DBNull) NumericCol=(DBNull) IntCol=(DBNull) BigintCol=(DBNull) UuidCol=(DBNull) DateCol=(DBNull) SmallDateTimeCol=(DBNull) DateTimeCol=(DBNull) DateTime2Col=(DBNull) DateTimeOffsetCol=(DBNull)"
            };

            for (int i = 0; i < expected.Length; i++)
                Assert.Equal(expected[i], database.Log[i]);
        }

        [Fact]
        public void ValidValuesShouldBeWrittenWithCorrectType()
        {
            var database = new MockDatabase();
            var configuration = new LoggingConfiguration();
            var target = new SqlServerBulkTarget(database)
            {
                Name = "SqlServerBulk",
                ConnectionString = "[ConnectionString]",
                Table = "Log",
                CreateTableIfNotExists = false,
                LoggingColumns = new List<LoggingColumn>
                {
                    new LoggingColumn { Name = "Message", SqlType = SqlType.NVARCHAR, Length = 10, Layout = "${message}"},
                    new LoggingColumn { Name = "VarcharCol", SqlType = SqlType.VARCHAR, Length = 3, Layout = "${mdlc:item=varchar}"},
                    new LoggingColumn { Name = "DecimalCol", SqlType = SqlType.DECIMAL, Scale = 1, Precision = 1, Layout = "${mdlc:item=decimal}"},
                    new LoggingColumn { Name = "NumericCol", SqlType = SqlType.NUMERIC, Layout = "${mdlc:item=decimal}"},
                    new LoggingColumn { Name = "IntCol", SqlType = SqlType.INT, Layout = "${mdlc:item=int}"},
                    new LoggingColumn { Name = "BigintCol", SqlType = SqlType.BIGINT, Layout = "${mdlc:item=bigint}"},
                    new LoggingColumn { Name = "UuidCol", SqlType = SqlType.UNIQUEIDENTIFIER, Layout = "${mdlc:item=uuid}"},
                    new LoggingColumn { Name = "DateCol", SqlType = SqlType.DATE, Layout = "${mdlc:item=date}"},
                    new LoggingColumn { Name = "SmallDateTimeCol", SqlType = SqlType.SMALLDATETIME, Layout = "${mdlc:item=smalldatetime}"},
                    new LoggingColumn { Name = "DateTimeCol", SqlType = SqlType.DATETIME, Layout = "${mdlc:item=datetime}"},
                    new LoggingColumn { Name = "DateTime2Col", SqlType = SqlType.DATETIME2, Precision = 1, Layout = "${mdlc:item=datetime2}"},
                    new LoggingColumn { Name = "DateTimeOffsetCol", SqlType = SqlType.DATETIMEOFFSET, Precision = 1, Layout = "${mdlc:item=datetimeoffset}"}
                }
            };
            configuration.AddTarget(target);
            configuration.AddRuleForAllLevels("SqlServerBulk");
            LogManager.Configuration = configuration;

            var Logger = LogManager.GetLogger("logger");

            MappedDiagnosticsLogicalContext.Set("varchar", "qwertyuiop");
            MappedDiagnosticsLogicalContext.Set("decimal", 1.23M);
            MappedDiagnosticsLogicalContext.Set("numeric", 1.23M);
            MappedDiagnosticsLogicalContext.Set("int", 123);
            MappedDiagnosticsLogicalContext.Set("bigint", 1234567890);
            MappedDiagnosticsLogicalContext.Set("uuid", Guid.Parse("00000000-0000-0000-0000-000000000000"));
            MappedDiagnosticsLogicalContext.Set("date", new DateTime(2000, 01, 01));
            MappedDiagnosticsLogicalContext.Set("smalldatetime", new DateTime(2000, 01, 01));
            MappedDiagnosticsLogicalContext.Set("datetime",  new DateTime(2000, 01, 01));
            MappedDiagnosticsLogicalContext.Set("datetime2", new DateTime(2000, 01, 01));
            MappedDiagnosticsLogicalContext.Set("datetimeoffset", new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.FromHours(1)));

            Logger.Info("hello world");

            var expected = new[] {
                "BULK INSERT: [ConnectionString] Log",
                "ROW: Message=(String)hello worl VarcharCol=(String)qwe DecimalCol=(Decimal)1.23 NumericCol=(Decimal)1.23 IntCol=(Int32)123 BigintCol=(Int64)1234567890 UuidCol=(Guid)00000000-0000-0000-0000-000000000000 DateCol=(DateTime)2000-01-01T00:00:00.0000000 SmallDateTimeCol=(DateTime)2000-01-01T00:00:00.0000000 DateTimeCol=(DateTime)2000-01-01T00:00:00.0000000 DateTime2Col=(DateTime)2000-01-01T00:00:00.0000000 DateTimeOffsetCol=(DateTimeOffset)2000-01-01T00:00:00.0000000+01:00"
            };

            for (int i = 0; i < expected.Length; i++)
                Assert.Equal(expected[i], database.Log[i]);
        }
    }
}
