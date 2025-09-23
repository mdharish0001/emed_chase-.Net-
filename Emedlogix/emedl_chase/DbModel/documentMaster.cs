using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class documentMaster
    {
        public documentMaster()
        {
            Demographics = new HashSet<doc_Demographic>();
            DocumenResultLineItems = new HashSet<documenResultLineItem>();
            //DocumentAssignments = new HashSet<documentAssignment>();
            Finalhccs = new HashSet<finalhcc>();
            Documentversionhistories = new HashSet<documentversionhistory>();
            DocumenDefaultCodeExtracts = new HashSet<documenDefaultCodeExtract>();
            CodeChangeLogs = new HashSet<codeChangeLog>();
            CodeSentences = new HashSet<doc_CodeSentences>();
            meatSentences = new HashSet<meatSentence>();
            CodeMutualExcludes = new HashSet<doc_MutualExclues>();
            Alreadyexistmembercodes = new HashSet<alreadyexistmembercodes>();
            standfordconditions = new HashSet<standfordcondition>();
            doc_page_sentences = new HashSet<doc_page_sentence>();
        }
        public int Id { get; set; }
        public int JobId { get; set; }
        public string DocId { get; set; }
        public string DocName { get; set; }
        public string DocDesc { get; set; }
        public string Status { get; set; }
        public int? TemplateId { get; set; }
        public int OrganisationId { get; set; }
        public string FilePath { get; set; }
        public string ActualDocumentName { get; set; }
        public bool isErrorCode { get; set; }
        public bool isRebuttal { get; set; }
        public int? userID { get; set; }
        //public int? auditorID { get; set; }
        public int currentVersionID { get; set; }
        //public bool isCurrent { get; set; }
        public int? createdBy { get; set; }
        public DateTime? createdOn { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public DateTime? extractedOn { get; set; }
        public string message { get; set; }
        public string errorAgainst { get; set; }
        public int? approvedVersionID { get; set; }
        public int? NoOfPages { get; set; }

        public string visionID { get; set; }
        public string fileSource { get; set; }
        public ICollection<doc_Demographic> Demographics { get; set; }
        public ICollection<documenResultLineItem> DocumenResultLineItems { get; set; }//documenDefaultCodeExtract
        public ICollection<documenDefaultCodeExtract> DocumenDefaultCodeExtracts { get; set; }
        public ICollection<documentExtract> documentExtracts { get; set; }
        public ICollection<finalhcc> Finalhccs { get; set; }
        public ICollection<documentversionhistory> Documentversionhistories { get; set; }
        public ICollection<codeChangeLog> CodeChangeLogs { get; set; }
        public ICollection<meatSentence> meatSentences { get; set; }
        public ICollection<doc_CodeSentences> CodeSentences { get; set; }
        public ICollection<doc_MutualExclues> CodeMutualExcludes { get; set; }
        public ICollection<alreadyexistmembercodes> Alreadyexistmembercodes { get; set; }
        public ICollection<standfordcondition> standfordconditions { get; set; }
        public virtual jobmaster Jobmaster { get; set; }

        //public ICollection<documentAssignment>  DocumentAssignments{ get; set; }
        public int? DateValidatedBy { get; set; }
        public int? CodeValidateddBy { get; set; }
        public DateTime? ocr_started_on { get; set; }
        public DateTime? ocr_end_on { get; set; }
        public string ocr_message { get; set; }
        public DateTime? nlp_started_on { get; set; }
        public DateTime? nlp_end_on { get; set; }
        public string nlp_message { get; set; }
        public DateTime? demo_started_on { get; set; }
        public DateTime? demo_end_on { get; set; }
        public string demo_message { get; set; }
        public DateTime? rule_started_on { get; set; }
        public DateTime? rule_end_on { get; set; }
        public string rule_message { get; set; }
        public DateTime? duplication_started_on { get; set; }
        public DateTime? duplication_end_on { get; set; }
        public string duplication_message { get; set; }
        public string ocr_outputpath { get; set; }
        public bool? isesrd { get; set; }
        public bool? is_moved_to_output { get; set; }
        //public bool isdelete { get; set; }
        public DateTime? moved_to_output_on { get; set; }
        public ICollection<doc_page_sentence> doc_page_sentences { get; set; }
        public int? icdcount { get; set; }
    }
}
