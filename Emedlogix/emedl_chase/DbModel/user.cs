using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class User
    {
        public User()
        {
            Documentversionhistories = new HashSet<documentversionhistory>();
            user_Organizatation_Roles = new HashSet<user_organizatation_role>();
        }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string AltPhoneNo { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }

        public string Status { get; set; }
        public string UserName { get; set; }
        public DateTime? DOT { get; set; }
        public DateTime? DOJ { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? Target { get; set; }
        public double? QA { get; set; }
        public string Address { get; set; }

        public ICollection<user_organizatation_role> user_Organizatation_Roles { get; set; }
        public ICollection<documentversionhistory> Documentversionhistories { get; set; }
        
    }
}
