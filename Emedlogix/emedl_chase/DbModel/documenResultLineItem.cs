using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class documenResultLineItem
    {
        public int Id { get; set; }
        public int JobDetailId { get; set; }
        public int SentenceId { get; set; }
        public string RuleNo { get; set; }
        public int Score { get; set; }
        public string Code { get; set; }
        public string Section { get; set; }
        public string Description { get; set; }
        public string notes { get; set; }
        public string CodeType { get; set; }
        public bool? isnonhcc { get; set; }
        public int? PageNo { get; set; }
        public int? ruleID { get; set; }
        public string dateOfService { get; set; }
        public string sentence { get; set; }
        public int? userID { get; set; }
        public int versionID { get; set; }
        public bool isCurrent { get; set; }
        public bool isExcludes { get; set; }
        public int? medSentenseID { get; set; }
        public int? hccOrder { get; set; }
        public int? order { get; set; }
        public bool isHistoryCode { get; set; }
        public string initialType { get; set; }
        public int? medicationPageNo { get; set; }
        public int action { get; set; }//0:non change 1: added 2:removed 3:Updated(ext-pot) 4:pot-Ext 5:rmd-pot 6:rmd-ext
        public int? duplicateCodeCnt { get; set; }
        public DateTime? dOS { get; set; }
        public bool actionInThisVersion { get; set; }
        public string responseText { get; set; }
        public string meatSentesne { get; set; }
        public int? extractedDueTo { get; set; }
        public int? createdBy { get; set; }
        public DateTime? createdOn { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public documentMaster DocumentMaster { get; set; }
        public bool isindirectfinding { get; set; } = false;
        public bool IsManual { get; set; }
        public bool IsHandWritten { get; set; }
        public string MeatCondition { get; set; }
        [NotMapped]
        public bool IsDelete { get; set; }
    }
}
