using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tt.uz.Dtos
{
    public class VCodeDto
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public int Code { get; set; }
        public bool IsEmail { get; set; }
    }
}
