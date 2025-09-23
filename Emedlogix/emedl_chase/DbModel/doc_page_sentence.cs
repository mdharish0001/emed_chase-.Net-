using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class doc_page_sentence
    {
        public doc_page_sentence()
        {

        }
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int JobId { get; set; }
        public string FullText { get; set; }
        public int? PageNo { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Message { get; set; }
        public DateTime? StartedOn { get; set; }
        public DateTime? EndOn { get; set; }
        public int? no_of_pages { get; set; }
        public int? retrycnt { get; set; }
        public double? diff { get; set; }
        public string DosList { get; set; }
        public DateTime? DOS { get; set; }
        public int? org_id { get; set; }
        public documentMaster DocumentMaster { get; set; }
    }
}
