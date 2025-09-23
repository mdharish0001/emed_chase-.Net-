using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class section
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public string Heading { get; set; }
        public int? CharacterCnt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
