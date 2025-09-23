using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class organization
    {
        public organization()
        {
            user_Organizatation_Roles = new HashSet<user_organizatation_role>();
            specialityList = new HashSet<speciality>();

            client_uploads=new HashSet<client_uploads>();
            //TypeOfDeliverables = new HashSet<TypeOfDeliverable>();

            


        }
        public int Id { get; set; } //projectId
        public string OrgName { get; set; }//projectname
        public string OrgType { get; set; }
        public bool IsDeleted { get; set; }
        public string Address_1 { get; set; }
        public string Address_2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Status { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string Email { get; set; }
        public string OrganizationLogoPath { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        // public double? ClientSamplingPercentage { get; set; }
        public ICollection<user_organizatation_role> user_Organizatation_Roles { get; set; }
        //public bool IsInternalValidators { get; set; }
        public string CategoryVersion { get; set; }
        public string ESRDModels { get; set; }
        public string NonEsrdModel { get; set; }
        public string DefaultDos { get; set; }
        public string ReviewerYear { get; set; }
        public int? Target { get; set; }

        public ICollection<speciality> specialityList { get; set; }

        //public ICollection<TypeOfDeliverable> TypeOfDeliverables { get; set; }


        public ICollection<client_uploads> client_uploads { get; set; }
        public ICollection<chart_master> chart_master { get; set; }
        //public DateTime? chart_review_period_start_date { get; set; }
        //public bool model { get; set; }
    }
}
