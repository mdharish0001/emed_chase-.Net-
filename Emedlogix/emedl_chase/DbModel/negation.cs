using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class negation
    {
        public negation()
        {
            NegationWithout_Word= new HashSet<negationwithout_word>();
        }
        public int Id { get; set; }
        public string Value { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<negationwithout_word> NegationWithout_Word { get; set; }
    }
}
