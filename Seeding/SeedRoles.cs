using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

//TODO: Upddate this namespace to match your project name
namespace sp23Team13FinalProject.Seeding
{
    public class SeedRoles
    {
        public static async Task AddAllRoles(RoleManager<IdentityRole> roleManager)
        {
            //TODO: Add the needed roles - admin and customer are provided
            //as examples
            //if the admin role doesn't exist, add it
            if (await roleManager.RoleExistsAsync("CSO") == false)
            {
                //this code uses the role manager object to create the admin role
                await roleManager.CreateAsync(new IdentityRole("CSO"));
            }

            //if the customer role doesn't exist, add it
            if (await roleManager.RoleExistsAsync("Recruiter") == false)
            {
                //this code uses the role manager object to create the customer role
                await roleManager.CreateAsync(new IdentityRole("Recruiter"));
            }

            //if the student role doesn't exist, add it
            if (await roleManager.RoleExistsAsync("Student") == false)
            {
                //this code uses the role manager object to create the customer role
                await roleManager.CreateAsync(new IdentityRole("Student"));
            }

        }
    }
}
