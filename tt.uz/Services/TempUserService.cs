using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Entities;
using tt.uz.Helpers;

namespace tt.uz.Services
{
    public interface ITempUserService
    {
        TempUser Create(TempUser user, string password, bool isEmail);
    }
    public class TempUserService : ITempUserService
    {
        private DataContext _context;
        private IVerificationCodeService _vcodeService;
        public TempUserService(DataContext context, IVerificationCodeService vcodeService)
        {
            _context = context;
            _vcodeService = vcodeService;
        }
        public TempUser Create(TempUser user, string password, bool isEmail)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (isEmail)
            {
                if (_context.Users.Any(x => x.Email == user.Email))
                    throw new AppException("Email \"" + user.Email + "\" is already taken");
            }
            else
            {
                if (_context.Users.Any(x => x.Phone == user.Phone))
                    throw new AppException("Phone \"" + user.Phone + "\" is already taken");
            }

            if (user.ReferrerCode != 0) {
                if (!_context.Users.Any(x => x.ReferralCode == user.ReferrerCode))
                    throw new AppException("Referrer Code not found");
            }

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            Random rnd = new Random();
            user.ReferralCode = rnd.Next(10000, 99999);
            _context.TempUsers.Add(user);

            VerificationCode vcode = new VerificationCode
            {
                FieldType = isEmail ? VerificationCode.EMAIL : VerificationCode.PHONE,
                FieldValue = isEmail ? user.Email : user.Phone
            };
            if (vcode.FieldType == VerificationCode.EMAIL)
                vcode.ExpireDate = DateHelper.AddDay(1);
            else if (vcode.FieldType == VerificationCode.PHONE)
                vcode.ExpireDate = DateHelper.AddMinut(10);
            if (_vcodeService.Send(vcode))
                _context.VerificationCodes.Add(vcode);

            _context.SaveChanges();

            return user;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
