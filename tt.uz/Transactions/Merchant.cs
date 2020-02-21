using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Helpers;

namespace tt.uz.Transactions
{
    public class Merchant
    {
        public void Authorize(HttpRequest request, IConfiguration Configuration) 
{ 
            StringValues token;
            bool check;
            request.Headers.TryGetValue("Authorization", out token);
            string url = request.Host.Host.ToString();
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(string.Concat("Paycom", ":", Configuration.GetValue<string>("Payme_Test")));
            string selfToken = string.Concat("Basic ", Convert.ToBase64String(plainTextBytes));
            check = string.Compare(token.ToString(), selfToken) == 0;
            //if (string.Compare(url, "localhost") == 0)
            //{
            //    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(string.Concat("Paycom", ":", Configuration.GetValue<string>("Payme_Test")));
            //    string selfToken = string.Concat("Basic ", Convert.ToBase64String(plainTextBytes));
            //    check = string.Compare(token.ToString(), selfToken) == 0;
            //}
            //else
            //{
            //    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(string.Concat("Paycom", ":", Configuration.GetValue<string>("Payme_Prod")));
            //    string selfToken = string.Concat("Basic ", Convert.ToBase64String(plainTextBytes));
            //    check = string.Compare(token.ToString(), selfToken) == 0;
            //}
            if (!check)
            {
                throw new AppException("Insufficient privilege to perform this method.", -32504);
            }
        }
    }
}
