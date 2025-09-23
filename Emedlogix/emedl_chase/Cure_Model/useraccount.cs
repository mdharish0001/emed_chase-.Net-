using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.Cure_Model
{
    public partial class useraccount
    {

        public int id { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? user_name { get; set; }
        public string email { get; set; }
        public string? phonenumber { get; set; }
        public string? password { get; set; }
        public string? registed_by { get; set; }
        public DateTime? registered_on { get; set; }
        public string? logo_url { get; set; }
        public string? logo_path { get; set; }
        public string? createdby { get; set; }
        public DateTime? createdon { get; set; }
        public bool is_deleted { get; set; }
        public string? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public DateTime? last_login_time { get; set; }
        public string? login_source_id { get; set; }
        public string? usertype { get; set; }
    }
}
