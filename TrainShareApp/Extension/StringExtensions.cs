using System;

namespace TrainShareApp.Extension
{
    public static class StringExtensions
    {
        public static int LengthSafe(this string src)
        {
            return string.IsNullOrEmpty(src) ? 0 : src.Length;
        }

        public static float LevensteinNorm(this string first, string second)
        {
            int distance = first.Levenshtein(second);
            int maxLength = Math.Max(first.LengthSafe(), second.LengthSafe());

            return (float)distance / maxLength;
        }

        public static int Levenshtein(this string src, string dest)
        {
            if (string.IsNullOrEmpty(src)) return dest.LengthSafe();
            if (string.IsNullOrEmpty(dest)) return src.LengthSafe();

            int i, j;
            var d = new int[src.Length + 1][];

            for (i = 0; i <= src.Length; i++)
            {
                d[i] = new int[dest.Length + 1];
                d[i][0] = i;
            }

            for (j = 0; j <= dest.Length; j++)
            {
                d[0][j] = j;
            }

            for (i = 1; i <= src.Length; i++)
            {
                for (j = 1; j <= dest.Length; j++)
                {
                    int cost = src[i - 1] == dest[j - 1] ? 0 : 1;

                    d[i][j] =
                        Math.Min(
                            d[i - 1][j] + 1,              // Deletion
                            Math.Min(
                                d[i][j - 1] + 1,          // Insertion
                                d[i - 1][j - 1] + cost)); // Substitution

                    if ((i > 1) && (j > 1) && (src[i - 1] ==
                        dest[j - 2]) && (src[i - 2] == dest[j - 1]))
                    {
                        d[i][j] = Math.Min(d[i][j], d[i - 2][j - 2] + cost);
                    }
                }
            }

            return d[src.Length][dest.Length];
        }
    }
}
