using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace IdentityV2
{
    class Program
    {
        static void Main(string[] args)
        {
            var username = "ken@ken.com";
            var password = "password";

            var userStore = new UserStore<IdentityUser>();
            var userManager = new UserManager<IdentityUser>(userStore);

            //var createResult = userManager.Create(new IdentityUser("ken@ken.com"), "password");
            //Console.WriteLine("created : {0}",createResult.Succeeded);

            IdentityUser user = userManager.FindByName(username);
            var claimResult = userManager.AddClaim(user.Id, new Claim("give_type", "give_value"));
            //Console.WriteLine("Claim:{0}", claimResult.Succeeded);
            //Console.ReadLine();

            var isMatch = userManager.CheckPassword(user, password);
            Console.WriteLine("Password Match: {0}", isMatch);
            Console.ReadLine();
        }
    }
}
