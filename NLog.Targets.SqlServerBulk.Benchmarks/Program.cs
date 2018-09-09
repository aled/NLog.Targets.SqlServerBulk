using System;
using NLog;
using NLog.Targets;
using NLog.Targets.SqlServerBulk;

namespace NLog.SqlServerBulkInsert.Benchmarks
{
    class Program
    {
        // TODO:
        //
        // Matrix of benchmark tests
        // -------------------------
        // Bulk insert (column-based)
        // Bulk insert (json; one log entry per row)
        // Bulk insert (json; multiple entries per row)
        // Generic database insert (column-based)
        // Generic database insert (json)

        // NVARCHAR(MAX)
        // NVARCHAR(4000)
        // VARCHAR(MAX)
        // VARCHAR(8000)

        // No wrapper
        // Buffering wrapper
        // Async wrapper

        // No compression
        // Client-side gzip compression
        // Server-side page compression [Note - does not work with NVARCHAR(MAX)]
        // Server-side row compression [Note - does not work with NVARCHAR(MAX)]

        static void Main(string[] args)
        {

        }
    }
}
