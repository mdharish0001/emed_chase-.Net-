using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class abbreviation
    {
        
        public int Id { get; set; }
        public string Abbreviation { get; set; }
        public string Expansion { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsBeforeApplied { get; set; }
    }
}
