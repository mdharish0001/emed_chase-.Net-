using System;
using System.Collections.Generic;
using System.Text;
namespace emedl_chase.DbModel
{
    public partial class abbreviationwithout_word
    {
        public int Id { get; set; }
        public int AbbreviationId { get; set; }
        public string Withoutword { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public wordconversion Abbreviation { get; set; }
    }
}
