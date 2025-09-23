using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emedl_chase.DbModel
{
    public partial class patient_history
    {
        public long Id { get; set; } 

        public string? PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientMiddleIntial { get; set; }
        public string PatientLastName { get; set; }
        public DateTime? PatientDOB { get; set; }
        public int? PatientAge { get; set; }

        public string PatientGender{ get; set; }
        public string PrimaryInsuranceName{ get; set; }
        public string Practicename { get; set; }
        public string Type{ get; set; }
        public int? org_id { get; set; }

    }
}
