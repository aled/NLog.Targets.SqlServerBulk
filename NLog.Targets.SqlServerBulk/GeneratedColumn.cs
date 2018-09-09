using System;
using NLog.Config;

namespace NLog.Targets.SqlServerBulk
{
    [NLogConfigurationItem]
    public class GeneratedColumn : AbstractColumn
    {
        [DefaultParameter]
        public bool IsPrimaryKey { get; set; }
    }
}
