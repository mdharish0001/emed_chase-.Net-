using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
   public  class Otp
    {
       
            public long ID { get; set; }
            public int UserId { get; set; }
            public string SendTo { get; set; }
            public string Otpvalue { get; set; }
            public string Otptype { get; set; }
            public string Purpose { get; set; }
            public DateTime CreatedTime { get; set; }
            public DateTime ExpiryTime { get; set; }
            public bool IsUsed { get; set; }
            public int IsSent { get; set; }
        }
}
