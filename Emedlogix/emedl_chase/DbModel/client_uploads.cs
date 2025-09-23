using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace emedl_chase.DbModel
{

   // [NotMapped]

    public class client_uploads
    {
        public client_uploads()
        {
            chart_masters = new HashSet<chart_master>();
        }
        public long id { get; set; }
        public string? batch { get; set; }
        public int org_id { get; set; }
        public int specialties { get; set; }
        public string deliverables_source { get; set; }
        public string type_of_deliverables { get; set; }

        public int? type_of_deliverables_id { get; set; }

        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        public string? status { get; set; }
        public int file_count { get; set; }
        public virtual organization Organization { get; set; }
        public virtual speciality Speciality { get; set; }
        public ICollection<chart_master> chart_masters { get; set; }

        //public int? type_of_deliverableId { get; set; }

        public bool isdelete { get; set; }
    }
}
