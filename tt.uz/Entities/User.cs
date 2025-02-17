﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Helpers;

namespace tt.uz.Entities
{
    public class User
    {
        public User() {
            Balance = 0;
            CreatedDate = DateHelper.GetDate();
            UpdatedDate = DateHelper.GetDate();
        }
        public int Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public int Balance { get; set; }
        public string FullName { get; set; }
        public int ReferralCode { get; set; }
        public int ReferrerCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
