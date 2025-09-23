using Npgsql.EntityFrameworkCore.PostgreSQL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emedl_chase.DbModel
{
    public class DenialClaimLine
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        public string? PatientId { get; set; }

        [Column("claim_id")]
        public string ClaimId { get; set; }

        [Column("claim_line_id")]
        public string ClaimLineId { get; set; }

        [Column("member_id")]
        public string MemberId { get; set; }

        [Column("member_name")]
        public string MemberName { get; set; }

        [Column("member_age")]
        public int? MemberAge { get; set; }

        [Column("member_gender")]
        public string MemberGender { get; set; }

        [Column("provider_id")]
        public string ProviderId { get; set; }

        [Column("provider_npi")]
        public string ProviderNpi { get; set; }

        [Column("insurance")]
        public string Insurance { get; set; }

        [Column("cob_details")]
        public string CobDetails { get; set; }

        [Column("dos_from")]
        public DateTime? DosFrom { get; set; }

        [Column("dos_to")]
        public DateTime? DosTo { get; set; }

        [Column("pos")]
        public string Pos { get; set; }

        [Column("cpt")]
        public string Cpt { get; set; }

        [Column("modifiers")]
        public string Modifiers { get; set; }

        [Column("units")]
        public int? Units { get; set; }

        [Column("icds")]
        public string Icds { get; set; }

        [Column("allowed_amount")]
        public decimal? AllowedAmount { get; set; }

        [Column("paid_amount")]
        public decimal? PaidAmount { get; set; }

        [Column("denied_amount")]
        public decimal? DeniedAmount { get; set; }

        public string Status { get; set; }
        public int? assign_user_id { get; set; }
        public int? roleId { get; set; }

        public bool isdelete { get; set; }

        public string? mod1 { get; set; }
        public string? mod2 { get; set; }
        public string? mod3 { get; set; }
        public string? mod4 { get; set; }

        public string? icd1 { get; set; }
        public string? icd2 { get; set; }
        public string? icd3 { get; set; }
        public string? icd4 { get; set; }

        public string filepath { get; set; }
        public string findings { get; set; }
        public string Recommendation { get; set; }

        public string HistoryIds { get; set; }
        public bool isReWork { get; set; }
        public DateTime? ReWork_denial_coder_completed_on { get; set; }
        public DateTime? Denial_coder_completed_on { get; set; }
        public DateTime? Assigned_to_denialcoder_on { get; set; }

        public string PaymentStatus { get; set; }
        public string FinalPaid { get; set; }
        public string RightOffDesc { get; set; }
        public string DenialCode { get; set; }
        public string DenialReason { get; set; }
        public int? client_id { get; set; }
        public int? speciality_id { get; set; }

        public string clientName { get; set; }
        public string SpecialityName { get; set; }
        public string? deliverablesSource { get; set; }
        public string? typeOfDeliverables { get; set; }

        public int? type_of_deliverableId { get; set; }

        public int? VersionId { get; set; }

        public DateTime? Assigned_to_AR_analyst_on { get; set; }
        public DateTime? AR_analyst_completed_on { get; set; }

        public int? DenialCoderUserId { get; set; }
        public int? ARAnalaystUserId { get; set; }
        public int? AlreadyWorkedArAnalystUserId { get; set; }

        public bool isArRole { get; set; }
        public int? Ar_user_id { get; set; }

        public DateTime? created_on { get; set; }
        public int? Uploaded_user_id { get; set; }

    }
}
