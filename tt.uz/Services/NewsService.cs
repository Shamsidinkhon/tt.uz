using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using tt.uz.Entities;
using tt.uz.Helpers;
using Microsoft.EntityFrameworkCore;

namespace tt.uz.Services
{
    public interface INewsService
    {
        News Create(News news, string imageIds);
        PagedList<News> GetAllByFilter(NewsSearch newsSearch, int userId);
        bool PostFavourite(UserFavourites uf);
        bool DeleteFavourite(int newsId, int userId);
        int UploadImage(IFormFile file, int userId);
        bool DeleteImage(int imageId, int userId);
        List<News> GetAllFavourites(int userId);
        bool PostTariff(Tariff tariff);
        List<News> GetAllByTariff(int type, int userId);
        bool PostVendorFavourite(VendorFavourite vf);
        bool DeleteVendorFavourite(int targetUserId, int userId);
        List<UserProfile> GetVendors(int userId);
        List<Category> Search(NewsSearch newsSearch);
    }
    public class NewsService : INewsService
    {
        private DataContext _context;
        public NewsService(DataContext context)
        {
            _context = context;
        }
        public News Create(News news, string imageIds)
        {
            news.Description = news.Description.Replace("\n", "<br />");
            _context.News.Add(news);
            _context.SaveChanges();
            var ids = imageIds.Split(',').Select(Int32.Parse).ToList();
            foreach (int id in ids)
            {
                var image = _context.Images.SingleOrDefault(x => x.ImageId == id);
                if (image != null)
                {
                    image.NewsId = news.Id;
                    _context.Images.Update(image);
                }
            }

            foreach (NewsAttribute newsAttribute in news.NewsAttribute)
            {
                newsAttribute.NewsId = news.Id;
                _context.NewsAttribute.Add(newsAttribute);
            }

            _context.SaveChanges();
            news.Images = _context.Images.Where(x => x.NewsId == news.Id).ToList();
            return news;
        }

        public int UploadImage(IFormFile file, int userId)
        {
            var folderName = Path.Combine("Resources", "Images");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            string[] extArray = { ".png", ".jpeg", ".jpg" };

            if (file == null)
            {
                throw new AppException("File is required");
            }

            if (file.Length == 0 || Array.IndexOf(extArray, Path.GetExtension(ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"'))) == -1)
            {
                throw new AppException("File size should be more than 0!; PNG, JPEG, JPG extensions are allowed");
            }

            var fileName = string.Concat(Guid.NewGuid(), Path.GetExtension(ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"')));
            var fullPath = Path.Combine(pathToSave, fileName);
            var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require

            Image image = new Image();
            image.Path = fileName;
            image.NewsId = 0;
            image.UserId = userId;
            _context.Images.Add(image);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            _context.SaveChanges();
            return image.ImageId;
        }

        public PagedList<News> GetAllByFilter(NewsSearch newsSearch, int userId)
        {
            var news = from n in _context.News
                       join u in _context.Users on n.OwnerId equals u.Id

                       join fav in _context.UserFavourites on n.Id equals fav.NewsId
                        into gj
                       from fav in gj.Where(x => x.UserId == userId).DefaultIfEmpty()

                       join vfav in _context.VendorFavourite on n.OwnerId equals vfav.TargetUserId
                        into vf
                       from vfav in vf.Where(x => x.UserId == userId).DefaultIfEmpty()

                       join p in _context.UserProfile on n.OwnerId equals p.UserId
                       into p
                       from profile in p.DefaultIfEmpty()

                       join f in _context.ExternalLogin on n.OwnerId equals f.UserId
                       into f
                       from facebook in f.DefaultIfEmpty()

                       join i in _context.Images on n.Id equals i.NewsId
                       into i
                       from images in i.DefaultIfEmpty()

                       join vr in _context.VendorReviews on u.Id equals vr.TargetUserId
                       into vr
                       from vrs in vr.DefaultIfEmpty()

                       select new News()
                       {
                           Id = n.Id,
                           Title = n.Title,
                           CategoryId = n.CategoryId,
                           Category = n.Category,
                           PriceId = n.PriceId,
                           Price = n.Price,
                           Description = n.Description,
                           LocationId = n.LocationId,
                           Location = n.Location,
                           ContactDetailId = n.ContactDetailId,
                           ContactDetail = n.ContactDetail,
                           Status = n.Status,
                           CreatedDate = n.CreatedDate,
                           UpdatedDate = n.UpdatedDate,
                           OwnerId = n.OwnerId,
                           Images = i == null ? new List<Image>() : i.ToList(),
                           Favourite = fav == null ? false : true,
                           VendorFavourite = vfav == null ? false : true,
                           OwnerDetails = new UserProfile()
                           {
                               UserId = u.Id,
                               UserCreatedAt = u.CreatedDate,
                               Name = profile != null ? (profile.Name != null ? profile.Name : u.FullName) : u.FullName,
                               Surname = profile != null ? (profile.Surname != null ? profile.Surname : u.FullName) : u.FullName,
                               Email = profile != null ? (profile.Email != null ? profile.Email : u.Email) : u.Email,
                               Phone = profile != null ? (profile.Phone != null ? profile.Phone : u.Phone) : u.Phone,
                               FacebookId = facebook != null ? facebook.ClientId : null,
                               Longtitude = profile != null ? profile.Longtitude : null,
                               Latitude = profile != null ? profile.Latitude : null,
                               RegionId = profile != null ? profile.RegionId : 0,
                               DistrictId = profile != null ? profile.DistrictId : 0,
                               CreatedDate = profile != null ? profile.CreatedDate : DateHelper.GetDate(),
                               UpdatedDate = profile != null ? profile.UpdatedDate : DateHelper.GetDate(),
                               Rating = vr.Average(c => Convert.ToInt32(c.Mark)).ToString()
                           },
                           NewsAttribute = (from newsAtr in _context.NewsAttribute
                                            where newsAtr.NewsId == n.Id
                                            join atr in _context.CoreAttribute on newsAtr.AttributeId equals atr.Id
                                            select new NewsAttribute()
                                            {
                                                Id = newsAtr.Id,
                                                NewsId = newsAtr.NewsId,
                                                AttributeId = newsAtr.AttributeId,
                                                Value = newsAtr.Value,
                                                AttributeInfo = new CoreAttribute()
                                                {
                                                    Id = atr.Id,
                                                    Name = atr.Name,
                                                    Title = atr.Title,
                                                    Type = atr.Type,
                                                    Unit = atr.Unit,
                                                    Required = atr.Required
                                                }

                                            }
                                           ).ToList(),
                       };



            if (newsSearch.OwnerId > 0)
            {
                news = news.Where(x => x.OwnerId == newsSearch.OwnerId);
            }

            int[] statuses = { News.NEW, News.ACTIVE, News.ARCHIVE, News.REJECTED };

            if (statuses.Contains(newsSearch.Status))
            {
                news = news.Where(x => x.Status == newsSearch.Status);
            }

            if (newsSearch.Id > 0)
            {
                news = news.Where(x => x.Id == newsSearch.Id);
            }

            if (newsSearch.CategoryId > 0)
            {
                news = news.Where(x => x.CategoryId == newsSearch.CategoryId);
            }

            if (!string.IsNullOrEmpty(newsSearch.Title))
            {
                news = news.Where(x => x.Title.Contains(newsSearch.Title));
            }

            return PagedList<News>.ToPagedList(news, newsSearch.PageNumber, newsSearch.PageSize);
        }

        public bool PostFavourite(UserFavourites uf)
        {
            var fav = _context.UserFavourites.SingleOrDefault(x => x.NewsId == uf.NewsId && x.UserId == uf.UserId);
            if (fav == null)
            {
                _context.UserFavourites.Add(uf);
                _context.SaveChanges();
            }
            return true;
        }

        public bool DeleteImage(int imageId, int userId)
        {
            var image = _context.Images.SingleOrDefault(x => x.ImageId == imageId && x.UserId == userId);
            if(image != null){
                _context.Images.Remove(image);
                _context.SaveChanges();
                return true;
            }
            throw new AppException("Image Not Found");
        }

        public List<News> GetAllFavourites(int userId)
        {

            var news = from n in _context.News
                       join fav in _context.UserFavourites on n.Id equals fav.NewsId
                       where fav.UserId == userId

                       join i in _context.Images on n.Id equals i.NewsId
                       into i
                       from images in i.DefaultIfEmpty()

                       select new News()
                       {
                           Id = n.Id,
                           Title = n.Title,
                           CategoryId = n.CategoryId,
                           Category = n.Category,
                           PriceId = n.PriceId,
                           Price = n.Price,
                           Description = n.Description,
                           LocationId = n.LocationId,
                           Location = n.Location,
                           ContactDetailId = n.ContactDetailId,
                           ContactDetail = n.ContactDetail,
                           Status = n.Status,
                           CreatedDate = n.CreatedDate,
                           UpdatedDate = n.UpdatedDate,
                           OwnerId = n.OwnerId,
                           Images = i == null ? new List<Image>() : i.ToList(),
                           Favourite = true,
                           NewsAttribute = (from newsAtr in _context.NewsAttribute
                                            where newsAtr.NewsId == n.Id
                                            join atr in _context.CoreAttribute on newsAtr.AttributeId equals atr.Id
                                            select new NewsAttribute()
                                            {
                                                Id = newsAtr.Id,
                                                NewsId = newsAtr.NewsId,
                                                AttributeId = newsAtr.AttributeId,
                                                Value = newsAtr.Value,
                                                AttributeInfo = new CoreAttribute()
                                                {
                                                    Id = atr.Id,
                                                    Name = atr.Name,
                                                    Title = atr.Title,
                                                    Type = atr.Type,
                                                    Unit = atr.Unit,
                                                    Required = atr.Required
                                                }

                                            }
                                           ).ToList(),
                       };

            return news.ToList();
        }

        public bool DeleteFavourite(int newsId, int userId)
        {
            var fav = _context.UserFavourites.SingleOrDefault(x => x.NewsId == newsId && x.UserId == userId);
            _context.UserFavourites.Remove(fav);
            _context.SaveChanges();
            return true;
        }

        public bool PostTariff(Tariff tariff)
        {
            int[] tariffs = { Tariff.MAIN, Tariff.VIP, Tariff.TOP };
            if (!tariffs.Contains(tariff.Type))
                throw new AppException("Wrong Tariff Type");
            var user = _context.Users.SingleOrDefault(x => x.Id == tariff.UserId);
            if (user == null)
                throw new AppException("User Not Found");
            var news = _context.News.SingleOrDefault(x => x.Id == tariff.NewsId);
            if (news == null)
                throw new AppException("News Not Found");
            if (news.OwnerId != tariff.UserId)
                throw new AppException("Wrong News Owner Id");
            tariff.Days = 7;
            tariff.ExpireDate = DateHelper.GetDate().AddDays(tariff.Days);
            _context.Tariff.Add(tariff);
            _context.SaveChanges();
            return true;
        }

        public List<News> GetAllByTariff(int type, int userId)
        {
            int[] tariffs = { Tariff.MAIN, Tariff.VIP, Tariff.TOP };
            if (!tariffs.Contains(type))
                throw new AppException("Wrong Tariff Type");
            var news = from n in _context.News
                       join tariff in _context.Tariff on n.Id equals tariff.NewsId
                       where tariff.Type == type && tariff.ExpireDate >= DateHelper.GetDate()
                       
                       join fav in _context.UserFavourites on n.Id equals fav.NewsId
                       into gj
                       from fav in gj.Where(x => x.UserId == userId).DefaultIfEmpty()
                       
                       join i in _context.Images on n.Id equals i.NewsId
                       into i
                       from images in i.DefaultIfEmpty()
                       
                       select new News()
                       {
                           Id = n.Id,
                           Title = n.Title,
                           CategoryId = n.CategoryId,
                           Category = n.Category,
                           PriceId = n.PriceId,
                           Price = n.Price,
                           Description = n.Description,
                           LocationId = n.LocationId,
                           Location = n.Location,
                           ContactDetailId = n.ContactDetailId,
                           ContactDetail = n.ContactDetail,
                           Status = n.Status,
                           CreatedDate = n.CreatedDate,
                           UpdatedDate = n.UpdatedDate,
                           OwnerId = n.OwnerId,
                           Images = _context.Images.Where(x => x.NewsId == n.Id).ToList(),
                           Favourite = fav == null ? false : true,
                           NewsAttribute = (from newsAtr in _context.NewsAttribute
                                            where newsAtr.NewsId == n.Id
                                            join atr in _context.CoreAttribute on newsAtr.AttributeId equals atr.Id
                                            select new NewsAttribute()
                                            {
                                                Id = newsAtr.Id,
                                                NewsId = newsAtr.NewsId,
                                                AttributeId = newsAtr.AttributeId,
                                                Value = newsAtr.Value,
                                                AttributeInfo = new CoreAttribute()
                                                {
                                                    Id = atr.Id,
                                                    Name = atr.Name,
                                                    Title = atr.Title,
                                                    Type = atr.Type,
                                                    Unit = atr.Unit,
                                                    Required = atr.Required
                                                }

                                            }
                                           ).ToList(),
                       };

            return news.ToList();
        }

        public bool PostVendorFavourite(VendorFavourite vf)
        {
            var vfModel = _context.VendorFavourite.SingleOrDefault(x => x.TargetUserId == vf.TargetUserId && x.UserId == vf.UserId);
            if (vfModel == null)
            {
                _context.VendorFavourite.Add(vf);
                _context.SaveChanges();
            }
            return true;
        }

        public bool DeleteVendorFavourite(int targetUserId, int userId)
        {
            var vf = _context.VendorFavourite.SingleOrDefault(x => x.TargetUserId == targetUserId && x.UserId == userId);
            _context.VendorFavourite.Remove(vf);
            _context.SaveChanges();
            return true;
        }

        public List<UserProfile> GetVendors(int userId)
        {
            var vendors = from u in _context.Users
                          join vf in _context.VendorFavourite on u.Id equals vf.TargetUserId
                          where vf.UserId == userId
                          join p in _context.UserProfile on u.Id equals p.UserId
                       into p
                          from profile in p.DefaultIfEmpty()
                          join f in _context.ExternalLogin on u.Id equals f.UserId
                          into f
                          from facebook in f.DefaultIfEmpty()
                          select new UserProfile()
                          {
                              UserId = u.Id,
                              Name = profile != null ? (profile.Name != null ? profile.Name : u.FullName) : u.FullName,
                              Surname = profile != null ? (profile.Surname != null ? profile.Surname : u.FullName) : u.FullName,
                              Email = profile != null ? (profile.Email != null ? profile.Email : u.Email) : u.Email,
                              Phone = profile != null ? (profile.Phone != null ? profile.Phone : u.Phone) : u.Phone,
                              FacebookId = facebook != null ? facebook.ClientId : null,
                              Longtitude = profile != null ? profile.Longtitude : null,
                              Latitude = profile != null ? profile.Latitude : null,
                              RegionId = profile != null ? profile.RegionId : 0,
                              DistrictId = profile != null ? profile.DistrictId : 0,
                              CreatedDate = profile != null ? profile.CreatedDate : DateHelper.GetDate(),
                              UpdatedDate = profile != null ? profile.UpdatedDate : DateHelper.GetDate()
                          };
            return vendors.ToList();
        }

        public List<Category> Search(NewsSearch newsSearch)
        {
            var result = (from news in _context.News
                          join cat in _context.Categories on news.CategoryId equals cat.Id
                          where news.Title.Contains(newsSearch.Title)
                          group news by new
                          {
                              news.CategoryId,
                              cat.Name
                          } into r
                          select new Category
                          {
                              Id = r.Key.CategoryId,
                              Name = r.Key.Name,
                              CountNews = r.Count()
                          }).ToList();
            return result;
        }
    }
}
