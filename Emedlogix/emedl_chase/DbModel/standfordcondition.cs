using System;
using System.Collections.Generic;
using System.Text;
namespace emedl_chase.DbModel
{
    public partial class standfordcondition
    {
        public standfordcondition()
        {

        }
        public int id { get; set; }
        public int doc_masterid { get; set; }
        public string condition { get; set; }
        public string context { get; set; }
        public bool negation { get; set; }
        public int sentenceid { get; set; }
        public documentMaster DocumentMaster { get; set; }
    }
}
