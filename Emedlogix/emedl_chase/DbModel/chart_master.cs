using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace emedl_chase.DbModel;

   // [NotMapped]

    public class chart_master
    {
        public chart_master()
        {
            chart_Conditions_Lineitems = new HashSet<chart_conditions_lineitem>();
            CptDetails = new HashSet<CptDetails>();
            ErrorSummaryList = new HashSet<ErrorSummary>();
        }
        public long Id { get; set; }
        public long? client_uploads_id { get; set; }
        public string? Sentence { get; set; }
        public string? Unique_Id { get; set; }
        public DateTime? Date_Of_Service { get; set; }
        public int? Created_By { get; set; }
        public DateTime? Created_On { get; set; }
        public string? Status { get; set; }
        public string? ExpectedIcds { get; set; }
        public DateTime? Date_Of_Birth { get; set; }
        public string? Gender { get; set; }
        public string? CptCode { get; set; }
        public string? Ref_No { get; set; }
        public string? state { get; set; }
        public int? assign_user_id { get; set; }
        public string? chart_status { get; set; }
        public string? filepath { get; set; }
        public string? filename { get; set; }
        public string? patient_name { get; set; }
        public string? place { get; set; }
        public string? claim_id { get; set; }
        public int? org_id { get; set; }
        public int? no_of_pages { get; set; }
        public virtual client_uploads Client_Uploads { get; set; }
        public virtual organization Organization { get; set; }
        public ICollection<chart_conditions_lineitem> chart_Conditions_Lineitems { get; set; }
        public ICollection<CptDetails> CptDetails { get; set; }
        public ICollection<ErrorSummary> ErrorSummaryList { get; set; }

        public string CoderChartReviewStatus { get; set; }
      //  public string QaChartReviewStatus { get; set; }
        public string CoderComments { get; set; }
        public string PrimaryInsurance { get; set; }
        public string SecondaryInsurance { get; set; }      
        public string Provider { get; set; }        
        public int? age { get; set; }

        public bool isdelete { get; set; }

        public string serviceLocation { get; set; }
        public string? encounter { get; set; }
        public string? patientid { get; set; }

        public DateTime? coding_completed_on { get; set; }

        public int? roleId { get; set; }
        public DateTime? assigned_date { get; set; }

        public DateTime? clarification_completed_on_for_coder { get; set; }
        public DateTime? clarification_completed_on_for_auditor { get; set; }
        public DateTime? clarification_completed_on_for_sme { get; set; }

        public bool isCoderClarified { get; set; }
        public int? clarificationOfCoderUserId { get; set; }
        public int? clarificationOfAuditorUserId { get; set; }
        public int? clarificationOfSmeUserId { get; set; }
        public string CoderClarificationComment { get; set; }
        public string AuditorClarificationComment { get; set; }
        public string SmeClarificationComment { get; set; }
        public bool isAuditorClarified { get; set; }

        public string? OcrStatus { get; set; }
        public string? visionID { get; set; }

        public int? NoOfPages { get; set; }

        public string? ocr_outputpath { get; set; }
        public bool isOcrProcess { get; set; }

        public bool isExternalClarification { get; set; } = false;

        public string DemoStatus { get; set; }
        public int? DemoAssignUserId { get; set; }

        public DateTime? DemoCompleted_on { get; set; }
        public DateTime? Demo_assignDate { get; set; }

        public string? DemoType { get; set; }
        public bool IsDemo { get; set; } = false;

        public int? ChargeAssignUserId { get; set; }
        public string? ChargeStatus { get; set; }
        public DateTime? charge_submitted_on { get; set; }
        public DateTime? charge_completed_on { get; set; }

        public string? ChargeComments { get; set; }

        public bool IsCharge { get; set; } = false;

        public DateTime? started_on { get; set; }
        public DateTime? end_on { get; set; }


        public DateTime? re_assigned_date { get; set; }
        public int? userId_before_re_assign { get; set; }

        public int? coder_completed_userid_for_calrification { get; set; }

        public int? qa_completed_userid_for_calrification { get; set; }
        public int? sme_completed_userid_for_calrification { get; set; }


    }

