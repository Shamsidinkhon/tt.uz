using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Dtos;
using tt.uz.Entities;
using tt.uz.Helpers;

namespace tt.uz.Services
{
    
    public interface IExternalLoginService {
        ExternalLogin CreateOrUpdate(User user, ExternalLoginDTO externalLogin, string type);
    }
    public class ExternalLoginService : IExternalLoginService
    {
        private DataContext _context;
        private IMapper _mapper;
        public ExternalLoginService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public ExternalLogin CreateOrUpdate(User user, ExternalLoginDTO externalLogin, string type) {
            var el = _context.ExternalLogin.SingleOrDefault(x => x.ClientId == externalLogin.Id && x.Type == ExternalLogin.FACEBOOK);
            if (el == null)
            {
                el = _mapper.Map<ExternalLogin>(externalLogin);
                el.UserId = user.Id;
                el.Type = type;
                _context.ExternalLogin.Add(el);
            }
            else {
                el.FirstName = externalLogin.FirstName;
                el.LastName = externalLogin.LastName;
                el.FullName = externalLogin.FullName;
                el.UpdatedDate = DateHelper.GetDate();
                _context.ExternalLogin.Update(el);
            }
            _context.SaveChanges();
            return el;
        }
    }
}
