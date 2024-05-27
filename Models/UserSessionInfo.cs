using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoT2.Models
{
    public class UserSessionInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; } // Chú ý: Cách này không nên được sử dụng để lưu mật khẩu
        public string LoginName { get; set; }
        public List<string> RoleNames { get; set; }
        public List<string> Tables { get; set; }
    }

}