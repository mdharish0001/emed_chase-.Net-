using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public class auditLog
    {
        public int ID { get; set; }
        public int? UserID { get; set; }
        public int? RoleId { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public DateTime CreatedOn { get; set; }
        public string IP { get; set; }
        public string UserAgent { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string UserName { get; set; }
    }
}
