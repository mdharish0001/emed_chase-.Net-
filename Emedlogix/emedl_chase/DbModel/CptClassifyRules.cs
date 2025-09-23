using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emedl_chase.DbModel
{
    public partial class CptClassifyRules
    {
        public CptClassifyRules()
        {
            //MedicalNecessityItems = new HashSet<MedicalNecessity>();
            //CptRuleWithoutWordsLists = new HashSet<CptRuleWithoutWords>();
            WithOutWord = new HashSet<LeftCptWithOutWord>();
            WithWord = new HashSet<CptWithWord>();

        }
        public long Id { get; set; }
        public string RuleId { get; set; }
        public string Cpt_code { get; set; }
        public string Speciality { get; set; }
        public string Section { get; set; }
        public string Category { get; set; }
        public string Group { get; set; }
        public string LongDesc { get; set; }
        public string MediumDesc { get; set; }
        public string ShortDesc { get; set; }
        public string Procedure { get; set; }
        public string ProcedureType { get; set; }
        public string Method { get; set; }
        public string Approach { get; set; }
        public string Device { get; set; }
        public string Location_BodyPart { get; set; }
        public string Location_Tissues { get; set; }
        public string Location_Postion { get; set; }
        public string Laterality { get; set; }
        public string Location_organ { get; set; }
        public string Qualifier_SizeAndVolume { get; set; }
        public string Qualifier_Single_Muliple { get; set; }
        public string Qualifier_Time_Taken { get; set; }

        public string Procedure_desc { get; set; }

        public DateTime? created_on { get; set; }

        public int? created_by { get; set; }

        public DateTime? modfied_on { get; set; }

        public int? modfied_by { get; set; }


        public int? score { get; set; }
        public ICollection<LeftCptWithOutWord> WithOutWord { get; set; }
        public ICollection<CptWithWord> WithWord { get; set; }

        public ICollection<cptcodewithoutword> CptCodeWithOutWord { get; set; }

        //public ICollection<MedicalNecessity> MedicalNecessityItems { get; set; }
        //public ICollection<CptRuleWithoutWords> CptRuleWithoutWordsLists { get; set; }
        [NotMapped]
        public string? row_id { get; set; }
        [NotMapped]
        public string? ErrorMessage { get; set; }

        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public bool isdeleted
        {
            get; set;
        }
    }
}
