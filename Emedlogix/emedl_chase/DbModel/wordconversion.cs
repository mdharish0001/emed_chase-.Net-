using System;
using System.Collections.Generic;
using System.Text;
namespace emedl_chase.DbModel
{
    public partial class wordconversion
    {
        public wordconversion()
        {
            AbbreviationWithoutWord = new HashSet<abbreviationwithout_word>();
        }
        public int Id { get; set; }
        public string Abbreviation { get; set; }
        public string Expansion { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsBeforeApplied { get; set; }
        public bool isDeleted { get; set; }
        public string SourceType { get; set; }
        public ICollection<abbreviationwithout_word> AbbreviationWithoutWord { get; set; }

    }
}
