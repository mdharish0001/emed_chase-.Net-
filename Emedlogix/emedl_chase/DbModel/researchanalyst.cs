using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class researchanalyst
    {
        public researchanalyst()
        {

        }
        public int Id { get; set; }
        [Column(TypeName = "json")]
        public string jresponse { get; set; }
        public int createdby { get; set; }
        public DateTime? createdon { get; set; }
        public int? assigneduserid { get; set; }
        public DateTime? assigneddate { get; set; }
        public string ra_status { get; set; }
        public string ra_comments { get; set; }
        public DateTime? completedon { get; set; }
        public string completedfrom { get; set; }
        public string ts_status { get; set; }
        public string modifiedby { get; set; }
        public DateTime? modifiedon { get; set; }
        public int? re_assigneduserid { get; set; }
        public int? re_assignedby { get; set; }
        public DateTime? re_assigneddate { get; set; }
        public int? it_resolvedby { get; set; }
        public DateTime? it_resolveddate { get; set; }
        public string it_comments { get; set; }

    }
}
