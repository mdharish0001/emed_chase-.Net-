using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class doc_Demographic
    {
        public long Id { get; set; }
        public string PatientName { get; set; }
        public DateTime? DOB { get; set; }
        public string Gender { get; set; }
        public string PatientType { get; set; }
        public DateTime? AdmitDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string MemberID { get; set; }
        public bool IsCurrent { get; set; }
        public int VersionID { get; set; }
        public int? Age { get; set; }
        public int Doc_MasterID { get; set; }
        public int? NoOfPage { get; set; }
        public string MRN { get; set; }
        public string Account { get; set; }
        public string MemberState { get; set; }
        public string SSN { get; set; }
        public string ServiceProviderNo { get; set; }
        public string ServiceProviderName { get; set; }
        public string NationalProviderIdentifier { get; set; }
        public string FacilityName { get; set; }
        public string ServiceAddress { get; set; }
        public string ServiceState { get; set; }
        public string RefProviderName { get; set; }
        public string AdmitProviderName { get; set; }
        public string DocumentType { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public documentMaster DocumentMaster { get; set; }
    }
}
