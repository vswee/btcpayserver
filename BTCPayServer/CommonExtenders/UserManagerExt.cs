using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BTCPayServer.Data;
using BTCPayServer.Models;
using BTCPayServer.Models.ServerViewModels;
using Microsoft.AspNetCore.Identity;

namespace BTCPayServer.CommonExtenders
{
    public static class UserManagerExt
    {
        public static UsersViewModel getUsersFromDatabase(this UserManager<ApplicationUser> userManager, int skip, int count)
        {
            var users = new UsersViewModel();
            users.Users = userManager.Users.Skip(skip).Take(count)
                .Select(u => new UsersViewModel.UserViewModel
                {
                    Name = u.UserName,
                    Email = u.Email,
                    Id = u.Id
                }).ToList();
            users.Skip = skip;
            users.Count = count;
            users.Total = userManager.Users.Count();

            return users;
        }
    }
}
