using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emedl_chase.DbModel
{
    public partial class Mod
    {
        public int Id { get; set; }

        public string modifier { get; set; }
        public string description { get; set; }
    }
}
