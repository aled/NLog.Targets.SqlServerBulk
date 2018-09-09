using System;
using NLog;

namespace NLog.Targets.SqlServerBulk.Example1
{
    class Program
    {
        static Logger log = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            for (int i = 0; i < 10000; i++)
                log.Info($"Iteration {i}");

            LogManager.Flush();
            LogManager.Shutdown();
        }
    }
}
