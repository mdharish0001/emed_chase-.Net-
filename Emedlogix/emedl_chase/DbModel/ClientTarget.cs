using Npgsql.EntityFrameworkCore.PostgreSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emedl_chase.DbModel

{
    public class ClientTarget
    {
        public int Id { get; set; }

        public int? target { get; set; }
        public string TypeOfDeliverable { get; set; }
        public int? TypeOfDeliverable_Id { get; set; }
        public int clientOrgId { get; set; }
        public bool IsDelete { get; set; } = false;

        public ClientOrganizations clientOrganization { get; set; }
    }
}
