using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Entities;
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
        private IUserService _userService;
        public PaymeService(DataContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }
        public object ProcessTransaction(Request request)
        {
            switch (request.Method)
            {
                case "CheckPerformTransaction":
                    return CheckPerformTransaction(request);
                case "CheckTransaction":
                    return CheckTransaction(request);
                case "CreateTransaction":
                    return CreateTransaction(request);
                case "PerformTransaction":
                    return PerformTransaction(request);
                case "CancelTransaction":
                    return CancelTransaction(request);
                case "ChangePassword":
                    return ChangePassword(request);
                case "GetStatement":
                    return GetStatement(request);
                default:
                    throw new AppException("Method not found.", -32601);
            }
        }

        private object CheckPerformTransaction(Request request)
        {
            var user = _context.Users.SingleOrDefault(x => x.Id == int.Parse(request.Params.Account.UserId));
            if (user == null)
                throw new AppException("User Not Found", -31050);

            CheckAmount(request.Params.Amount);

            var transaction = _context.Transactions.SingleOrDefault(x => x.PaycomTransactionId == request.Params.Id);

            if (transaction == null)
            {
                var states = new int[] { TransactionEntity.STATE_CREATED, TransactionEntity.STATE_COMPLETED };
                transaction = _context.Transactions.SingleOrDefault(x => x.UserId == int.Parse(request.Params.Account.UserId) && states.Contains(x.State));
            }

            if (transaction != null)
            {
                if (transaction.State == TransactionEntity.STATE_CREATED || transaction.State == TransactionEntity.STATE_COMPLETED)
                    throw new AppException("There is other active/completed transaction for this order.", -31008);

                if (user.Id != transaction.UserId)
                    throw new AppException("User is not owner of this transaction", -31050);
            }

            return new
            {
                allow = true
            };
        }

        private object CheckTransaction(Request request)
        {
            var transaction = _context.Transactions.SingleOrDefault(x => x.PaycomTransactionId == request.Params.Id);
            if (transaction == null)
                throw new AppException("Transaction not found.", -31003);

            return new
            {
                create_time = Convert.ToInt64(transaction.PaycomTime),
                perform_time = DateHelper.GetTotalMillisecondsByDate(transaction.PerformTime),
                cancel_time = DateHelper.GetTotalMillisecondsByDate(transaction.CancelTime),
                transaction = transaction.Id.ToString(),
                state = transaction.State,
                reason = transaction.Reason
            };
        }
        private object CreateTransaction(Request request)
        {
            var user = _context.Users.SingleOrDefault(x => x.Id == int.Parse(request.Params.Account.UserId));
            if (user == null)
                throw new AppException("User Not Found", -31050);

            CheckAmount(request.Params.Amount);

            var states = new int[] { TransactionEntity.STATE_CREATED, TransactionEntity.STATE_COMPLETED };
            var transaction = _context.Transactions.SingleOrDefault(x => x.UserId == int.Parse(request.Params.Account.UserId) && states.Contains(x.State));

            if (transaction != null)
            {
                if ((transaction.State == TransactionEntity.STATE_CREATED || transaction.State == TransactionEntity.STATE_COMPLETED) && transaction.PaycomTransactionId != request.Params.Id)
                    throw new AppException("There is other active/completed transaction for this order.", -31008);
            }

            transaction = _context.Transactions.SingleOrDefault(x => x.PaycomTransactionId == request.Params.Id);
            if (transaction != null)
            {
                if (transaction.State != TransactionEntity.STATE_CREATED)
                {
                    throw new AppException("Transaction found, but is not active.", -31008);
                }
                else if (transaction.isExpired())
                {
                    Cancel(transaction, TransactionEntity.REASON_CANCELLED_BY_TIMEOUT);
                    throw new AppException("Transaction is expired.", -31008);
                }
                else
                {
                    return new
                    {
                        create_time = Convert.ToInt64(transaction.PaycomTime),
                        transaction = transaction.Id.ToString(),
                        state = transaction.State
                    };
                }
            }
            else
            {
                if (Convert.ToInt64(request.Params.Time) - DateHelper.GetTotalMilliseconds() >= TransactionEntity.TIMEOUT)
                {
                    throw new AppException(string.Concat("Since create time of the transaction passed ", TransactionEntity.TIMEOUT, "ms"), -31008);
                }
            }

            if (transaction == null)
                transaction = new TransactionEntity();


            transaction.PaycomTransactionId = request.Params.Id;
            transaction.PaycomTime = request.Params.Time;
            transaction.PaycomTimeDatetime = DateHelper.UnixTimeStampToDateTime(double.Parse(request.Params.Time));
            transaction.State = TransactionEntity.STATE_CREATED;
            transaction.Amount = request.Params.Amount;
            transaction.UserId = user.Id;
            _context.Transactions.Update(transaction);
            _context.SaveChanges();

            return new
            {
                create_time = Convert.ToInt64(transaction.PaycomTime),
                transaction = transaction.Id.ToString(),
                state = transaction.State
            };
        }
        private object PerformTransaction(Request request)
        {
            var transaction = _context.Transactions.SingleOrDefault(x => x.PaycomTransactionId == request.Params.Id);
            if (transaction == null)
                throw new AppException("Transaction not found.", -31003);
            switch (transaction.State)
            {
                case TransactionEntity.STATE_CREATED:
                    if (transaction.isExpired())
                    {
                        Cancel(transaction, TransactionEntity.REASON_CANCELLED_BY_TIMEOUT);
                        throw new AppException("Transaction is expired.", -31008);
                    }
                    else
                    {
                        _userService.AddAmountToBalance(transaction.UserId, transaction.Amount);
                        transaction.State = TransactionEntity.STATE_COMPLETED;
                        transaction.PerformTime = DateHelper.GetDate();
                        _context.Transactions.Update(transaction);
                        _context.SaveChanges();
                        return new
                        {
                            perform_time = DateHelper.GetTotalMillisecondsByDate(transaction.PerformTime),
                            transaction = transaction.Id.ToString(),
                            state = transaction.State
                        };
                    }
                case TransactionEntity.STATE_COMPLETED:
                    return new
                    {
                        perform_time = DateHelper.GetTotalMillisecondsByDate(transaction.PerformTime),
                        transaction = transaction.Id.ToString(),
                        state = transaction.State
                    };
                default:
                    throw new AppException("Could not perform this operation.", -31008);
            }
        }

        private object CancelTransaction(Request request)
        {
            var transaction = _context.Transactions.SingleOrDefault(x => x.PaycomTransactionId == request.Params.Id);
            if (transaction == null)
                throw new AppException("Transaction not found.", -31003);
            switch (transaction.State)
            {
                case TransactionEntity.STATE_CANCELLED:
                case TransactionEntity.STATE_CANCELLED_AFTER_COMPLETE:
                    return new
                    {
                        cancel_time = DateHelper.GetTotalMillisecondsByDate(transaction.CancelTime),
                        transaction = transaction.Id.ToString(),
                        state = transaction.State
                    };
                case TransactionEntity.STATE_CREATED:
                    Cancel(transaction, request.Params.Reason);
                    return new
                    {
                        cancel_time = DateHelper.GetTotalMillisecondsByDate(transaction.CancelTime),
                        transaction = transaction.Id.ToString(),
                        state = transaction.State
                    };
                case TransactionEntity.STATE_COMPLETED:
                    Cancel(transaction, request.Params.Reason);
                    _userService.SubstractAmountFromBalance(transaction.UserId, transaction.Amount);
                    return new
                    {
                        cancel_time = DateHelper.GetTotalMillisecondsByDate(transaction.CancelTime),
                        transaction = transaction.Id.ToString(),
                        state = transaction.State
                    };
                default:
                    throw new AppException("Could not cancel transaction. Order is delivered/Service is completed.", -31007);
            }
        }

        private object ChangePassword(Request request)
        {
            if (string.IsNullOrEmpty(request.Params.Password))
                throw new AppException("New password not specified.", -31050);

            return new
            {
                success = true
            };
        }

        private object GetStatement(Request request)
        {
            if (string.IsNullOrEmpty(request.Params.From))
                throw new AppException("Incorrect period. From", -31050);
            if (string.IsNullOrEmpty(request.Params.To))
                throw new AppException("Incorrect period. To", -31050);
            if (long.Parse(request.Params.From) >= long.Parse(request.Params.To))
                throw new AppException("Incorrect period. (from >= to)", -31050);
            return new
            {
                transactions = _context.Transactions.Where(
                    x => x.PaycomTimeDatetime >= DateHelper.UnixTimeStampToDateTime(double.Parse(request.Params.From))
                    &&
                   x.PaycomTimeDatetime < DateHelper.UnixTimeStampToDateTime(double.Parse(request.Params.To))
                    )
            };
        }

        private void Cancel(TransactionEntity transaction, int reason)
        {
            transaction.CancelTime = DateHelper.GetDate();
            if (transaction.State == TransactionEntity.STATE_COMPLETED)
            {
                transaction.State = TransactionEntity.STATE_CANCELLED_AFTER_COMPLETE;
            }
            else
            {
                transaction.State = TransactionEntity.STATE_CANCELLED;
            }
            transaction.Reason = reason;
            _context.Transactions.Update(transaction);
            _context.SaveChanges();
        }

        private void CheckAmount(int amount)
        {
            if (amount < 1 || amount > 1000000)
                throw new AppException("Incorrect amount.", -31001);
        }

    }
}
