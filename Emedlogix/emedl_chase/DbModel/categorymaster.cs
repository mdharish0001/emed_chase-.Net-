using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class categorymaster
    {
        public categorymaster()
        {
            Hccversioncategories = new HashSet<hccversioncategory>();
            Hccinclusions = new HashSet<hccinclusion>();
            //Finalhccforcodes = new HashSet<finalhccforcode>();
            Finalhccs = new HashSet<finalhcc>();
        }
        public long Id { get; set; }
        public short Year { get; set; }
        public string Name { get; set; }
        public bool IsRxHcc { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDelete { get; set; }
        public ICollection<hccversioncategory> Hccversioncategories { get; set; }
        public ICollection<hccinclusion> Hccinclusions { get; set; }//finalhccforcode
        public ICollection<finalhcc> Finalhccs { get; set; }

        public string Version { get; set; }

    }
}
