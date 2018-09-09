using NLog.Config;
using NLog.Targets.SqlServerBulk.Sql;

namespace NLog.Targets.SqlServerBulk
{
    [NLogConfigurationItem]
    public abstract class AbstractColumn
    {
        [RequiredParameter]
        public string Name { get; set; }

        /// <summary>
        /// The SQL datatype
        /// </summary>
        /// <value>The type of the data.</value>
        [DefaultParameter]
        public SqlType SqlType { get; set; }

        [DefaultParameter]
        public int? Precision { get; set; }
    }
}
