using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace emedl_chase.DbModel
{
    public class ServiceModel
    {

    }
    public class MysqlParameterModel
    {
        public string ParameterName { get; set; }
        public object Value { get; set; }
        public bool IsNullable { get; set; }
        public bool SearchByContains { get; set; }
    }

}
