using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emedl_chase.DbModel
{
    public partial class ClientOrganizations
    {

        public ClientOrganizations() {

            ClientTargetList = new HashSet<ClientTarget>();
        }
        public int Id { get; set; }

        public int[] SpecialityId { get; set; }
        public int? typedeliverable_id { get; set; }
        public int? orgId { get; set; }
        public int? target { get; set; }
        public bool IsDeleted { get; set; }

        public string OrgName { get; set; }
        public string SpcalityName { get; set; }
        public string TypeOfdeliverable { get; set; }

        public DateTime? created_on { get; set; }
        public DateTime? modfiedon { get; set; }
        public int? created_by { get; set; }
        public int? modfied_by { get; set; }
        public int? roleId { get; set; }

        public ICollection<ClientTarget> ClientTargetList { get; set; }


    }
}
