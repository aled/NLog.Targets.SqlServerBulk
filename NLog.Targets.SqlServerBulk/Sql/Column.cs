using System.Text;

namespace NLog.Targets.SqlServerBulk.Sql
{
    public class Column
    {
        public string Name { get; set; }
        public bool IsIdentity { get; set; }
        public string DefaultValue { get; set; }
        public bool IsNullable { get; set; }
        public bool IsPrimaryKey { get; set; }
        public SqlType SqlType { get; set; }
        public int? Precision { get; set; }
        public int? Scale { get; set; }
        public int Length { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{Name.SqlQuote()} {SqlType} ");

            switch (SqlType)
            {
                case SqlType.VARCHAR:
                    sb.Trim();
                    sb.Append($"({(Length < 0 || Length > 8000 ? "MAX" : Length.ToString())}) ");
                    break;

                case SqlType.NVARCHAR:
                    sb.Trim();
                    sb.Append($"({(Length < 0 || Length > 4000 ? "MAX" : Length.ToString())}) ");
                    break;

                case SqlType.DECIMAL:
                case SqlType.NUMERIC:
                    if (Precision.HasValue)
                    {
                        sb.Trim();
                        sb.Append($"({Precision}");
                        if (Scale.HasValue)
                            sb.Append($",{Scale}");

                        sb.Append(") ");
                    }
                    break;

                case SqlType.DATETIME2:
                case SqlType.DATETIMEOFFSET:
                    if (Precision.HasValue)
                    {
                        sb.Trim();
                        sb.Append($"({Precision}) ");
                    }
                    break;
            }

            if (IsIdentity)
                sb.Append("IDENTITY ");
            else
                if (DefaultValue != null)
                    sb.Append($"DEFAULT({DefaultValue}) ");

            if (!IsNullable)
                sb.Append("NOT NULL ");

            if (IsPrimaryKey)
                sb.Append("PRIMARY KEY ");

            sb.Trim();

            return sb.ToString();
        }
    }
}
