using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class jobmaster
    {
        public jobmaster()
        {
            DocumentMasters = new HashSet<documentMaster>();
        }
        public int JobId { get; set; }
        public DateTime Jobdate { get; set; }
        public int DocsCount { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int OrganisationId { get; set; }
        public DateTime JobStartTime { get; set; }
        public DateTime? JobEndTime { get; set; }
        public int? SuccessCount { get; set; }
        public int? FailiureCount { get; set; }
        public string Status { get; set; }
        public int? no_of_pages { get; set; }
        //public bool isdelete { get; set; }
        public bool isparent { get; set; }
        public ICollection<documentMaster> DocumentMasters { get; set; }
    }
}
