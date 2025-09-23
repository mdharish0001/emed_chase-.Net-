using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public class dosExcel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime? DOS { get; set; }
        public int PageNo { get; set; }

        public int JobId { get; set; }
        public int DocumentMasterId { get; set; }

        public int? NoOfPage { get; set; }
    }
}
