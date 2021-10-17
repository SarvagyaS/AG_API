using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AG.Models
{
    public class UserDetails
    {
        public UserDetails()
        {
            UserAddressDetails = new List<UserAddressDetails>();
        }
        public long Id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string nick_name { get; set; }
        public string email { get; set; }
        public string country_code { get; set; }
        public string mobile { get; set; }
        public string password { get; set; }
        public bool is_otp_verified { get; set; }
        public string register_from { get; set; }
        public bool is_profile_update { get; set; }
        public string gender { get; set; }
        public string dob { get; set; }
        public string hear_aboutus { get; set; }
        public string interested_in { get; set; }
        public string interested_in_bidding { get; set; }
        public string gst_no { get; set; }
        public string device_code { get; set; }
        public DateTime? last_login_date { get; set; }
        public DateTime? profile_update_date { get; set; }
        public DateTime? registration_date { get; set; }
        public string user_ip { get; set; }
        public string user_agent { get; set; }
        public string bank_name { get; set; }
        public string account_num { get; set; }
        public string ifsc_code { get; set; }
        public string pan_card { get; set; }
        public string aadhar_card { get; set; }
        public bool is_active { get; set; }
        public int? birthDay { get; set; }
        public int? birthMonth { get; set; }
        public int? birthYear { get; set; }

        [NotMapped]
        public string token { get; set; }

        public List<UserAddressDetails> UserAddressDetails { get; set; }
    }

    public  class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string PhoneNo { get; set; }
        public bool IsUserNameSelected { get; set; }
    }
}