using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class classificationrules
    {
        public classificationrules()
        {
            //Rulewithoutwords = new HashSet<rulewithoutwords>();
        }
        public long RuleId { get; set; }
        public string RuleNo { get; set; }
        public string Code { get; set; }
        public string Descs { get; set; }
        public string Local_Descs { get; set; }
        public string Conditions { get; set; }
        public string Qualityspecificity { get; set; }
        public string NamePlace { get; set; }
        public string SequelaeHistory { get; set; }
        public string FamilialAutoimmune { get; set; }
        public string ComplicationProcess { get; set; }
        public string Manifestation { get; set; }
        public string Associatedcondition1 { get; set; }
        public string Associatedcondition2 { get; set; }
        public string Associatedcondition3 { get; set; }
        public string Context { get; set; }
        public string Externalcontext { get; set; }
        public string AltKeyword { get; set; }
        public string Timing { get; set; }
        public string Severity { get; set; }
        public string Locprop { get; set; }
        public string LocationBodypart { get; set; }
        public string LocationOrgan { get; set; }
        public string LocationTissue { get; set; }
        public string LocationLaterality { get; set; }
        public string LocationPosition { get; set; }
        public string LocationBodySystem { get; set; }
        public string Units { get; set; }
        public string AgeRelated { get; set; }
        public string GenderRelated { get; set; }
        public string StageEncounter { get; set; }
        public string OtherSpecific { get; set; }
        public string UnSpecified { get; set; }
        public int? Score { get; set; }
        public DateTime? ValidFromDate { get; set; }
        public DateTime? ValidToDate { get; set; }
        public string Status { get; set; }
        public int? CreatedBy { get; set; }        
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public int Source { get; set; }
        public int Priority { get; set; }
        public bool? IsNonHCC { get; set; }
        public bool? Reviewed { get; set; }//
        public bool? IsStndAlone { get; set; }
        public ICollection<rulewithoutwords> Rulewithoutwords { get; set; }

    }
}
