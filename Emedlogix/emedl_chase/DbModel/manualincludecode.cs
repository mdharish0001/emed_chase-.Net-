using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
   public class manualincludecode
    {
        public int Id { get; set; }
        public string HCCCode { get; set; }
        public string AdditionalHCC { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
