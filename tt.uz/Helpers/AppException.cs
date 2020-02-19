using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace tt.uz.Helpers
{
    public class AppException : Exception
    {
        public int Code { get; set; }
        public AppException() : base() { }

        public AppException(string message) : base(message) { }
        public AppException(string message, int Code) : base(message) { this.Code = Code; }

        public AppException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
