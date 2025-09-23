using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
   public partial class documentversionhistory
    {
        public int Id { get; set; }
        public int Doc_MasterID { get; set; }
        public int? UserId { get; set; }
        public int VersionID { get; set; }
        public int? CodeAdded { get; set; }
        public int? CodeRemoved { get; set; }
        public string Status { get; set; }        
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public string VersionStatus { get; set; }
        public bool IsCancelled { get; set; }
        public int? CancelleBy { get; set; }
        public DateTime? CancelledOn { get; set; }
        public int? PreviousVersion { get; set; }
        public documentMaster DocumentMaster { get; set; }
        public User User { get; set; }
        public int? DateValidatedBy { get; set; }
        public int? CodeValidateddBy { get; set; }
        public bool? ishccfound { get; set; }
    }
}
