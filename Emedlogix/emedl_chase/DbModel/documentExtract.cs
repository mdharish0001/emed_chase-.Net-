using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class documentExtract
    {
        public documentExtract()
        {

        }
        public int Id { get; set; }
        public int Doc_MasterID { get; set; }
        public int SentenseID { get; set; }
        public string ActualSentense { get; set; }
        public string ProcessedSentense { get; set; }
        public bool IsNegated { get; set; }
        public int? PageNo { get; set; }
        public DateTime? DateOfService { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public documentMaster DocumentMaster { get; set; }
        public bool IsManual { get; set; }
        public bool IsHandWritten { get; set; }
        public string Section { get; set; }
        public string SectionType { get; set; }
        public DateTime? DateOfService_To { get; set; }
        public string Provider { get; set; }
        public bool IsMissingSignature { get; set; }
        public string DocType { get; set; }
    }
}
