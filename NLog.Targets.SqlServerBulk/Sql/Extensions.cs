using System.Text;

namespace NLog.Targets.SqlServerBulk.Sql
{
    public static class Extensions
    {
        public static string SqlQuote(this string s) => $"[{s.Replace("]", "]]")}]";

        public static void Trim(this StringBuilder sb)
        {
            while (sb.Length > 0 && char.IsWhiteSpace(sb[sb.Length - 1]))
                sb.Length = sb.Length - 1;
        }
    }
}
