using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tt.uz.Transactions
{
    public class Request
    {
        public string Id { get; set; }
        public string Method { get; set; }
        public Params Params { get; set; }
    }
}
