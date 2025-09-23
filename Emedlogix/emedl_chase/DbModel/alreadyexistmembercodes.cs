using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class alreadyexistmembercodes
    {
        public long id { get; set; }
        public int docmasterid { get; set; }
        public int hccvalue { get; set; }
        public int modelid { get; set; }
        public string memberid { get; set; }
        public int dosyear { get; set; }
        public int? createdby { get; set; }
        public DateTime? createdon { get; set; }
        public documentMaster DocumentMaster { get; set; }
    }
}
