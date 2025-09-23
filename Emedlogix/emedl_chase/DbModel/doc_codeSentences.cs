using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public class doc_CodeSentences
    {
        public int Id { get; set; }
        public int PageNo { get; set; }
        public int SentenceId { get; set; }
        public string Sentence { get; set; }
        public DateTime? Dos { get; set; }
        public string Code { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public int DocId { get; set; }
        public int VersionId { get; set; }
        public documentMaster DocumentMaster { get; set; }

        public bool IsBestSentence { get; set; }

    }
}
