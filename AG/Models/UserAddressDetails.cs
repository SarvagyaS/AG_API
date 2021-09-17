using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AG.Models
{
    public class UserAddressDetails
    {
        public long Id { get; set; }
        public long UserDetailsId { get; set; }
        public UserDetails UserDetails { get; set; }
        public string name { get; set; }
        public bool is_billing_address { get; set; }
        public bool is_same_as_postal_add { get; set; }
        public string country { get; set; }
        public string address_line_1 { get; set; }
        public string address_line_2 { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
        public string city { get; set; }
        public DateTime? created_date { get; set; }
        public bool is_active { get; set; }
    }
}