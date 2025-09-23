using System;
using System.Collections.Generic;
using System.Text;
namespace emedl_chase.DbModel
{
    public partial class hccversioncategory
    {
        public long Id { get; set; }
        public long CategoryID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string HccCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public categorymaster Categorymaster { get; set; }
    }
}
