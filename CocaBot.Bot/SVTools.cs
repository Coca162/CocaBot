using SpookVooper.Api.Entities;
using SpookVooper.Api.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CocaBot
{
    static public class SVTools
    {
        static public async Task<string> SVIDToName(string SVID)
        {
            bool isgroup = SVID[..1].Contains("g-");
            bool isuser = SVID[..1].Contains("u-");

            if (isgroup == false && isuser == false) { return null; }
            if (isgroup == true && isuser == true)
            {
                throw new Exception("A item cannot both have 'g-' and 'u-' at the 2 starting letters of a string!");
            }
            else if (isgroup == false && isuser == true)
            {
                User user = new User(SVID);
                return await user.GetUsernameAsync();
            }
            else
            {
                Group group = new Group(SVID);
                return await group.GetNameAsync();
            }
        }

        static public async Task<Dictionary<string, string>> NameToSVID(string name)
        {
            Dictionary<string, string> svids = new Dictionary<string, string>();

            string gsvid = await Group.GetSVIDFromNameAsync(name);
            string usvid = await User.GetSVIDFromUsernameAsync(name);

            Console.WriteLine(gsvid);
            Console.WriteLine(usvid);

            bool isgroup = !gsvid.Contains("HTTP Error: NotFound; Could not find");
            bool isuser = !usvid.Contains("HTTP Error: NotFound; Could not find");

            if (!isgroup && !isuser)
            {
                return null;
            }
            else if (isgroup && isuser)
            {
                svids.Add("User", usvid);
                svids.Add("Group", gsvid);
            }
            else if (isuser)
            {
                svids.Add("User", usvid);
            }
            else
            {
                svids.Add("Group", gsvid);
            }

            return svids;
        }
    }
}
