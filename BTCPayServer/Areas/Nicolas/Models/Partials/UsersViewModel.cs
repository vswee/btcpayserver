using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BTCPayServer.Areas.Nicolas.Models.Partials
{
    public class UsersViewModel
    {
        public class UserListItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
        }

        public int Skip { get; set; }
        public int Count { get; set; }
        public int Total { get; set; }
        public string StatusMessage { get; set; }

        public List<UserListItem> Users { get; set; } = new List<UserListItem>();
    }


}
