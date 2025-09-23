using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace emedl_chase.DbModel
{
   // [NotMapped]

    public partial class charts_log_table_step3
    {
        public charts_log_table_step3()
        {

        }
        public long id { get; set; }
        public string ssh { get; set; }
        public string clinicaltext { get; set; }
        public string condition { get; set; }
        public string icdstatus { get; set; }
        public string icdcode { get; set; }
        public int rowid { get; set; }
        public string message { get; set; }
        public string filename { get; set; }
        public string file_batch { get; set; }
        public DateTime? createddate { get; set; }
        public int? stage { get; set; } //1.Pending 2.Assigned 3.Completed 4.Approve 5. Reject
        public int? assign_user_id { get; set; }
        public int? created_by { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
    }
}
