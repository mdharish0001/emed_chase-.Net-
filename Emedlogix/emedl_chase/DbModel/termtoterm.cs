using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class termtoterm
    {
        public int Id { get; set; }
        public string Term { get; set; }
        public string MapTo { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}

