using System;
using System.Collections.Generic;
using System.Linq;
using static Shared.Tools;

namespace Shared.Commands;
public static class Shared
{
    public const string NoExactsMessage = "This is not a valid name or svid!";

    public static string NoExacts(string search, IEnumerable<SearchReturn> nonExacts) 
        => NoExacts(search, nonExacts.Select(x => x.Name));

    public static string NoExacts(string search, IEnumerable<string> names)
    {
        if (names is null || !names.Any()) return NoExactsMessage;

        names = names.OrderBy(x => StringDifference(search, x)).Take(5);

        string NotExactMessage = NoExactsMessage + "\nDid you mean?";

        foreach (string name in names)
        {
            NotExactMessage += "\n" + name;
        }
        return NotExactMessage;
    }

    public static int StringDifference(string main, string comparer)
    {
        if (comparer.Contains(main + " ", StringComparison.OrdinalIgnoreCase)) return 0;

        return LevenshteinDistance(main.ToLower(), comparer.ToLower());
    }

    public static int LevenshteinDistance(string s, string t)
    {
        if (s == null || t == null)
        {
            throw new Exception("Strings must not be null");
        }

        int n = s.Length;
        int m = t.Length;

        if (n == 0)
        {
            return m;
        }
        else if (m == 0)
        {
            return n;
        }

        int[] p = new int[n + 1];
        int[] d = new int[n + 1];
        int[] _d;

        int i;
        int j;

        char t_j;

        int cost;

        for (i = 0; i <= n; i++)
        {
            p[i] = i;
        }

        for (j = 1; j <= m; j++)
        {
            t_j = t[j - 1];
            d[0] = j;

            for (i = 1; i <= n; i++)
            {
                cost = s[i - 1] == t_j ? 0 : 1;
                d[i] = Math.Min(Math.Min(d[i - 1] + 1, p[i] + 1), p[i - 1] + cost);
            }

            _d = p;
            p = d;
            d = _d;
        }

        return p[n];
    }
}
