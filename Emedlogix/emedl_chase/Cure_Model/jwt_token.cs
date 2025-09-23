using System;

namespace emedl_chase.Cure_Model
{
    public partial class jwt_token  //also called as project master
    {
        public jwt_token()
        {
        }
        public int id { get; set; } //projectId
        public string token { get; set; }//projectname
        public int user_id { get; set; }
        public bool isdelete { get; set; }
        public DateTime? created_on { get; set; }
        public DateTime? modified_on { get; set; }

    }
}
