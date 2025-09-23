using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emedl_chase.DbModel
{
    public class role_master
    {
        public role_master() {

            user_Organizatation_Roles = new HashSet<user_organizatation_role>();

        }    
        public int Id { get; set; }
        public string Role { get; set; }
        public bool isdelete { get; set; }
        public string createdby { get; set; }
        public DateTime? createdon { get; set; }
        public string modifiedby { get; set; }
        public DateTime? modifiedon { get; set; }

        public ICollection<user_organizatation_role> user_Organizatation_Roles { get; set; }
    }
}
