using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emedl_chase.DbModel
{
    public partial class userdaterangesampling
    {
        public userdaterangesampling()
        {
        }
        public int id { get; set; }
        public int user_organizatation_role_id { get; set; }
        public DateTime from_date { get; set; }
        public DateTime? to_date { get; set; }
        public double samplingpercentage { get; set; }
        //public bool isCurrent { get; set; }
        public int? createdby { get; set; }
        public DateTime? createdon { get; set; }
        public int? modifiedby { get; set; }
        public DateTime? modifiedon { get; set; }
        public bool isdeleted { get; set; }
        public virtual user_organizatation_role User_Organizatation_Role { get; set; }
    }
}
