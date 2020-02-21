using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tt.uz.Transactions
{
    public class Params
    {
        public string Id { get; set; }
        public int Amount { get; set; }
        public int Reason { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public Account Account { get; set; }
        public string Time { get; set; }
        public string Password { get; set; }
        public string Key { get; set; }
    }
}
