using NLog.Layouts;
using NLog.Config;

namespace NLog.Targets.SqlServerBulk
{
    [NLogConfigurationItem]
    public class LoggingColumn : AbstractColumn
    {
        [RequiredParameter]
        public Layout Layout { get; set; }

        [DefaultParameter]
        public int Length { get; set; } = -1;

        [DefaultParameter]
        public int? Scale { get; set; }
    }
}
