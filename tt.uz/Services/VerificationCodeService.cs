using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Threading.Tasks;
using tt.uz.Entities;
using tt.uz.Helpers;

namespace tt.uz.Services
{
    public interface IVerificationCodeService
    {
        bool Verify(string value, string type, int code);
        bool Send(VerificationCode vcode);
    }
    public class VerificationCodeService : IVerificationCodeService
    {
        private DataContext _context;
        private readonly IHttpClientFactory _clientFactory;
        private readonly AppSettings _appSettings;
        private IMemoryCache _cache;
        public VerificationCodeService(DataContext context, IHttpClientFactory clientFactory, IOptions<AppSettings> appSettings, IMemoryCache memoryCache)
        {
            _context = context;
            _clientFactory = clientFactory;
            _appSettings = appSettings.Value;
            _cache = memoryCache;
        }
        public bool Verify(string value, string type, int code)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new AppException("Email or Phone is required");
            if (string.IsNullOrWhiteSpace(type))
                throw new AppException("Verification Type is required");
            var vcode = _context.VerificationCodes.FirstOrDefault(p => p.FieldType == type && p.FieldValue == value && p.Code == code && DateTime.Compare(p.ExpireDate, DateHelper.GetDate()) >= 0);
            if (vcode == null)
                return false;
            return true;
        }

        public bool Send(VerificationCode vcode)
        {
            if (vcode.FieldType == VerificationCode.EMAIL)
            {
                SmtpClient client = new SmtpClient("mail.tt.uz");
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("no_reply@tt.uz", "nkv%tcF.Lp}4");

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("no_reply@tt.uz");
                mailMessage.To.Add(vcode.FieldValue);
                mailMessage.Body = vcode.Code.ToString();
                mailMessage.Subject = "subject";
                client.Send(mailMessage);
                return true;
            }
            if (vcode.FieldType == VerificationCode.PHONE)
            {
                string token = GetToken();
                if (!string.IsNullOrWhiteSpace(token))
                {
                    var values = new Dictionary<string, string>
                        {
                           { "mobile_phone", vcode.FieldValue},
                           { "message", string.Concat("Код подтверждения: ", vcode.Code, " tt.uz")}
                        };
                    var content = new FormUrlEncodedContent(values);
                    var client = _clientFactory.CreateClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = client.PostAsync(_appSettings.SmsPostUrl, content);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        throw new Exception("Sms send falied");
                    }

                }
            }
            return false;
        }

        public string GetToken()
        {
            if (_cache.TryGetValue(CacheKeys.TokenSms, out string _out))
            {
                return _cache.Get<string>(CacheKeys.TokenSms);
            }
            else
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(82800));
                var values = new Dictionary<string, string>
                {
                   { "email", _appSettings.SmsEmail},
                   { "password", _appSettings.SmsPassword}
                };
                var content = new FormUrlEncodedContent(values);
                var client = _clientFactory.CreateClient();

                var response = client.PostAsync(_appSettings.SmsTokenUrl, content);
                if (response.Result.IsSuccessStatusCode)
                {
                    var responseString = response.Result.Content.ReadAsStringAsync().Result;
                    var resource = JObject.Parse(responseString);
                    string token = resource["data"]["token"].ToString();
                    _cache.Set(CacheKeys.TokenSms, token, cacheEntryOptions);
                    return token;
                }
                else
                {
                    throw new Exception("Get Token falied");
                }
            }
        }
    }
}
