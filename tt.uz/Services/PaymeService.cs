using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Helpers;
using tt.uz.Transactions;

namespace tt.uz.Services
{
    public interface IPaymeService
    {
        object ProcessTransaction(Request request);
    }
    public class PaymeService : IPaymeService
    {
        private DataContext _context;
        public PaymeService(DataContext context)
        {
            _context = context;
        }
        public object ProcessTransaction(Request request)
        {
            switch (request.Method)
            {
                case "CheckPerformTransaction":
                    return CheckPerformTransaction(request);
                default:
                    throw new AppException("Method not found.", -32601);
            }
        }

        private object CheckPerformTransaction(Request request)
        {
            var transaction = _context.Transactions.SingleOrDefault(x => x.PaycomTransactionId == request.Id);
            if(transaction != null)
                throw new AppException("There is other active/completed transaction for this order.", -31008);

            var user = _context.Users.SingleOrDefault(x => x.Id == request.Params.Account.UserId);
            if (user == null)
                throw new AppException("User Not Found", -31050);

            return new
            {
                result = new { allow = true }
            };
        }
    }
}
