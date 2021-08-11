using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Shop.Models;
using Shop.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shop.Repositories
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public bool ChangeUserPassword(string userId, string newPassword)
        {
            try
            {
                var store = new UserStore<ApplicationUser>(Context);
                var manager = new UserManager<ApplicationUser>(store);
                //var manager = new ApplicationUserManager();
                var token = manager.GeneratePasswordResetToken(userId);
                manager.ResetPassword(userId, token, newPassword);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}