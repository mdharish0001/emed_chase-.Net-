using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public class Mailverify
    {
        public int Id { get; set; }
        public int userid { get; set; }
        public string token { get; set; }
        public string email { get; set; }
        public DateTime? created_on { get; set; }
        public DateTime? expried_on { get; set; }
        public DateTime? used_on { get; set; }
        public bool isused { get; set; }
        public bool isexpried { get; set; }
        public bool is_send { get; set; }
        public string message { get; set; }


    }
}
