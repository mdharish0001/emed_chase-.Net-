using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emedl_chase.DbModel
{
    public partial class targets
    {
        public int? Id { get; set; }
        public int? PageOrder { get; set; }
        public int? TargetId { get; set; }

        public int? noOfChartsTarget { get; set; }
        public usertarget usertarger { get; set; }
        public bool isdeleted { get; set; }
        public int? createdby { get; set; }
        public DateTime? createdon { get; set; }
        public int? modifiedby { get; set; }
        public DateTime? modifiedon { get; set; }

    }
}
