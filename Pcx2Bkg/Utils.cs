namespace Pcx2Bkg
{
    public class StrItem
    {
        public StrItem Next { get; set; }
        public string Str { get; set; }
    }

    public class StrList
    {
        public StrItem First { get; set; }
        public StrItem Last { get; set; }
    }

    public class Vector
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }

    public static class Utility
    {
        private static readonly HashSet<char> Alpha = new HashSet<char>("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_");
        private static readonly HashSet<char> Number = new HashSet<char>("0123456789-+");
        private static readonly HashSet<char> Symbol = new HashSet<char>("(),\\");

        public static string GetToken(ref string s)
        {
            string token = string.Empty;
            int i = 0;

            while (i < s.Length && string.IsNullOrEmpty(token))
            {
                if (Alpha.Contains(s[i]))
                {
                    while (i < s.Length && (Alpha.Contains(s[i]) || Number.Contains(s[i])))
                    {
                        token += s[i];
                        i++;
                    }
                }
                else if (Number.Contains(s[i]))
                {
                    token += s[i];
                    i++;
                    while (i < s.Length && (Number.Contains(s[i]) && s[i] != '-' && s[i] != '+'))
                    {
                        token += s[i];
                        i++;
                    }
                }
                else if (Symbol.Contains(s[i]))
                {
                    token = s[i].ToString();
                }
                else if (s[i] == '#')
                {
                    token = "#";
                    i++;
                    while (i < s.Length && (Alpha.Contains(s[i]) || Number.Contains(s[i])))
                    {
                        token += s[i];
                        i++;
                    }
                }
                else if (s[i] == '/')
                {
                    token = "/";
                    i++;
                    if (i < s.Length && s[i] == '/')
                    {
                        token += s.Substring(i);
                        i = s.Length;
                    }
                }
                else if (s[i] == '"')
                {
                    token = "\"";
                    i++;
                    while (i < s.Length && s[i] != '"')
                    {
                        token += s[i];
                        i++;
                    }
                }

                i++;
            }

            if (i > s.Length && string.IsNullOrEmpty(token))
                s = string.Empty;
            else
                s = s.Substring(i);

            return token;
        }
    }
}
