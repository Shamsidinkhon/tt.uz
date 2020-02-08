using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using tt.uz.Entities;
using tt.uz.Helpers;
using tt.uz.Dtos;
using AutoMapper;

namespace tt.uz.Services
{
    public interface IUserService
    {
        User AuthenticateWithEmail(string email, string password);
        User AuthenticateWithPhone(string phone, string password);
        User FindByEmail(string email);
        IEnumerable<User> GetAll();
        User GetById(int id);
        User Create(User user, string password, bool isEmail);
        //void Update(User user, string password = null);
        void Delete(int id);
        User CreateExternalUser(User user);
        UserProfile GetProfile(int id, int currentUserId);
        Image GetProfileImage(int id);
        bool CreateOrUpdateProfile(int currentUserId, UserProfileDto ptofile);
        bool ForgetPassword(User user, bool isEmail);
    }

    public class UserService : IUserService
    {
        private DataContext _context;
        private IVerificationCodeService _vcodeService;
        private IMapper _mapper;

        public UserService(DataContext context, IMapper mapper, IVerificationCodeService vcodeService)
        {
            _context = context;
            _vcodeService = vcodeService;
            _mapper = mapper;
        }

        public User FindByEmail(string email)
        {
            var user = _context.Users.SingleOrDefault(x => x.Email == email);
            if (user == null)
                return null;
            return user;
        }

        public User AuthenticateWithEmail(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;
            var user = _context.Users.SingleOrDefault(x => x.Email == email);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public User AuthenticateWithPhone(string phone, string password)
        {
            if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(password))
                return null;
            var user = _context.Users.SingleOrDefault(x => x.Phone == phone);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public User Create(User user, string password, bool isEmail)
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

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);

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

        public User CreateExternalUser(User user)
        {

            _context.Users.Add(user);

            _context.SaveChanges();

            return user;
        }

        //public void Update(User userParam, string password = null)
        //{
        //    var user = _context.Users.Find(userParam.Id);

        //    if (user == null)
        //        throw new AppException("User not found");

        //    if (userParam.Username != user.Username)
        //    {
        //        // username has changed so check if the new username is already taken
        //        if (_context.Users.Any(x => x.Username == userParam.Username))
        //            throw new AppException("Username " + userParam.Username + " is already taken");
        //    }

        //    // update user properties
        //    user.FirstName = userParam.FirstName;
        //    user.Phone = userParam.Phone;
        //    user.Username = userParam.Username;

        //    // update password if it was entered
        //    if (!string.IsNullOrWhiteSpace(password))
        //    {
        //        byte[] passwordHash, passwordSalt;
        //        CreatePasswordHash(password, out passwordHash, out passwordSalt);

        //        user.PasswordHash = passwordHash;
        //        user.PasswordSalt = passwordSalt;
        //    }

        //    _context.Users.Update(user);
        //    _context.SaveChanges();
        //}

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        // private helper methods

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

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        public UserProfile GetProfile(int id, int currentUserId)
        {
            var profile = _context.UserProfile.SingleOrDefault(x => x.UserId == id);
            var user = _context.Users.SingleOrDefault(x => x.Id == id);
            if (profile == null)
                profile = new UserProfile();
            profile.VendorFavourite = _context.VendorFavourite.Any(x => x.UserId == currentUserId && x.TargetUserId == id);
            var facebook = _context.ExternalLogin.SingleOrDefault(x => x.UserId == id);
            if (facebook != null)
                profile.FacebookId = facebook.ClientId;

            if (profile.ImageId != 0)
            {
                profile.Image = _context.Images.SingleOrDefault(x => x.ImageId == profile.ImageId);
            }

            if (user != null)
            {
                if (profile.Name == null)
                {
                    profile.Name = user.FullName;
                }
                if (profile.Email == null)
                {
                    profile.Email = user.Email;
                }
                if (profile.Phone == null)
                {
                    profile.Phone = user.Phone;
                }
                if (profile.UserId == 0)
                {
                    profile.UserId = user.Id;
                }
                profile.Balance = user.Balance;
            }
            return profile;
        }

        public bool CreateOrUpdateProfile(int currentUserId, UserProfileDto profileDto)
        {
            var profile = _context.UserProfile.SingleOrDefault(x => x.UserId == currentUserId);

            if (profile == null){
                profile = new UserProfile();
                profile.UserId = currentUserId;
            }

            _mapper.Map<UserProfileDto, UserProfile>(profileDto, profile);
            _context.UserProfile.Update(profile);
            return _context.SaveChanges() > 0;
        }

        public bool ForgetPassword(User user, bool isEmail)
        {
            // validation
            var u = new User();
            if (isEmail)
            {
                u = _context.Users.SingleOrDefault(x => x.Email == user.Email);
                if (u == null)
                    throw new AppException("Email " + user.Email + " not found");
            }
            else
            {
                u = _context.Users.SingleOrDefault(x => x.Phone == user.Phone);
                if (u == null)
                    throw new AppException("Phone " + user.Phone + " not found");
            }

            byte[] passwordHash, passwordSalt;

            Random rnd = new Random();
            int tempPass = rnd.Next(111111, 999999);
            CreatePasswordHash(tempPass.ToString(), out passwordHash, out passwordSalt);

            u.PasswordHash = passwordHash;
            u.PasswordSalt = passwordSalt;

            _context.Users.Update(u);
            _context.SaveChanges();

            return _vcodeService.SendPassword(u, tempPass, isEmail);
        }

        public Image GetProfileImage(int id){
            return _context.Images.SingleOrDefault(x => x.ImageId == id);
        }
    }
}
