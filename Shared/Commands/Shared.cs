using System.Collections.Generic;
using System.Linq;
using static Shared.Tools;

namespace Shared.Commands
{
    public class Shared
    {
        public static string NoExactsMessage(IEnumerable<SearchReturn> nonExacts)
        {
            string NotExactMessage = "This is not a valid name or svid!";
            if (!nonExacts.Any()) return NotExactMessage;
            NotExactMessage += "\nDid you mean?";
            nonExacts = nonExacts.Take(5);
            foreach (var item in nonExacts)
            {
                NotExactMessage += "\n" + item.Name;
            }
            return NotExactMessage;
        }
    }
}
