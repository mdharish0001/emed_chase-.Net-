using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace emedl_chase.DbModel
{
    //[NotMapped]

    public class application_config
    {
        public int id { get; set; }
        public string key { get; set; }
        public string value { get; set; }
        public string description { get; set; }
        public DateTime? created_on { get; set; }
        public int? created_by { get; set; }
        //public string local_Desc { get; set; }
        
    }
}
