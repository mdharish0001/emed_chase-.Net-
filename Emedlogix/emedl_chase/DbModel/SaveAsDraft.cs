using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emedl_chase.DbModel
{
    public partial class SaveAsDraft
    {
        public int id { get; set; }
        public string? batch { get; set; }
        public int? client_id { get; set; }
        public int? speciality_id { get; set; }
        public string? deliverablesSource { get; set; }
        public string? Status { get; set; }
        public string? typeOfDeliverables { get; set; }

        public int? type_of_deliverableId { get; set; }

        public int? created_by { get; set; }
        public int? modfied_by { get; set; }
        public DateTime? created_on { get; set; }

        public DateTime? modfied_on { get; set; }

        public bool isdelete { get; set; }
    }
}
