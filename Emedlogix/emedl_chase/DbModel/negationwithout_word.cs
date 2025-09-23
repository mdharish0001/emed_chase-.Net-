using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class negationwithout_word
    {
        public int Id { get; set; }
        public int NegationId { get; set; }
        public string Withoutword { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public negation Negation { get; set; }
    }
}
