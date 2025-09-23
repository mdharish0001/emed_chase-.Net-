using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
   public partial class documentAssignment
    {
        public long Id { get; set; }
        public int DocumentID { get; set; }
        public int UserID { get; set; }
        public int VersionID { get; set; }
        public string Status { get; set; }
        public DateTime AssignedOn { get; set; }
        public int AssignedBy { get; set; }
        public documentMaster DocumentMaster { get; set; }
    }
}
