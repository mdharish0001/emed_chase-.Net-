using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace emedl_chase.DbModel
{
    [Table("TypeOfDeliverable")] 
    public partial class TypeOfDeliverable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment ID
        public int Id { get; set; }

        public string Deliverable { get; set; }

        public bool isdeleted { get; set; }

        public int? Target { get; set; }

        public DateTime? Created_on { get; set; }

        public DateTime? Updated_on { get; set; }

        public int? Created_by { get; set; }

        public string type { get; set; }
    }
}

