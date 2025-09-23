using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emedl_chase.DbModel
{
    public partial class user_organizatation_role
    {
        public user_organizatation_role()
        {

            userdaterangesamplings = new HashSet<userdaterangesampling>();
            userDateRangeTarget = new HashSet<usertarget>();

        }
        public int Id { get; set; }
        public int? OrganizatationId { get; set; } ////projectId
        public int RoleId { get; set; }
        public int UserId { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? Target { get; set; }
        public double? QA { get; set; }
        public role_master RoleMaster { get; set; }
        public User User { get; set; }
        public organization Organization { get; set; }

        public ICollection<userdaterangesampling> userdaterangesamplings { get; set; }
        public ICollection<usertarget> userDateRangeTarget { get; set; }
    }
}
