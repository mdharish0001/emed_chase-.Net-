using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace emedl_chase.DbModel
{
   // [NotMapped]

    public partial class rulewithoutwordsforicdmapping
    {
        public long id { get; set; }
        public long? ruleid { get; set; }
        public string word { get; set; }
        public string code { get; set; }
        public bool? iscodelevel { get; set; } = false;
        public string createdby { get; set; }
        public DateTime? createdon { get; set; }
        public string modifiedby { get; set; }
        public DateTime? modifiedon { get; set; }
        public bool isdelete { get; set; }
        

    }
}
