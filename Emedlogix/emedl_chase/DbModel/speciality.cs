using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emedl_chase.DbModel
{
    public partial class speciality
    {
        public speciality()
        {
            client_Uploads = new HashSet<client_uploads>();
        }
        public int id {  get; set; }
        public string Speciality { get; set; }
        public int clientid { get; set; }

        public bool IsDeleted { get; set; }

        public organization organization { get; set; }
        public ICollection<client_uploads> client_Uploads { get; set; }


    }
}
