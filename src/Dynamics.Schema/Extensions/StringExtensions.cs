namespace Dynamics.Schema.Extensions
{
    public static class StringExtensions
    {
        public static string SupplimentIfDifferentTo(this string first, string second, string format = " ({0})")
        {
            return first + (first.Equals(second) ? "" : string.Format(format, second));
        }

        public static string Fill(this string str, params object[] objs)
        {
            return string.Format(str, objs);
        }
    }
}
