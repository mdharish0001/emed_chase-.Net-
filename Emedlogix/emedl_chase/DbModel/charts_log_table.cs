using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace emedl_chase.DbModel
{
   // [NotMapped]

    public partial class charts_log_table
    {
        public charts_log_table()
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
        public DateTime? modified_on { get; set; }
        public int? stage { get; set; }
    }
}
