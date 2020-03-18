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
using tt.uz.Dtos;
using Newtonsoft.Json;
using AutoMapper;

namespace tt.uz.Services
{
    public interface INewsService
    {
        News Create(News news, string imageIds);
        PagedListNews GetAllByFilter(NewsSearch newsSearch);
        bool PostFavourite(UserFavourites uf);
        bool DeleteFavourite(int newsId, int userId);
        int UploadImage(IFormFile file, int userId);
        bool DeleteImage(int imageId, int userId);
        PagedListNews GetAllFavourites(NewsSearch newsSearch);
        bool PostTariff(Tariff tariff);
        PagedListNews GetAllByTariff(NewsSearch newsSearch);
        bool PostVendorFavourite(VendorFavourite vf);
        bool DeleteVendorFavourite(int targetUserId, int userId);
        List<UserProfile> GetVendors(int userId);
        List<Category> Search(NewsSearch newsSearch);
        bool UpdateRegions();
        IEnumerable<Region> GetRegions(string lang = "ru");
        News Update(NewsDTO newsDTO, int Id, int userId);
        News ChangeStatus(int Id, int status, int userId);
    }
    public class NewsService : INewsService
    {
        private DataContext _context;
        private IMapper _mapper;
        public NewsService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public News Create(News news, string imageIds)
        {
            //news.Description = news.Description.Replace("\n", "<br />");
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

        public News Update(NewsDTO newsDTO, int Id, int userId)
        {
            var news = _context.News.SingleOrDefault(x => x.Id == Id);
            if (news == null)
                throw new AppException("News not found");

            if (news.OwnerId != userId)
                throw new AppException("User is not owner of this news");

            if (news.Status != News.ARCHIVE)
                throw new AppException("Action is not allowed");

            _mapper.Map(newsDTO, news);
            _context.News.Update(news);

            var ids = newsDTO.ImageIds.Split(',').Select(Int32.Parse).ToList();
            foreach (int id in ids)
            {
                var image = _context.Images.SingleOrDefault(x => x.ImageId == id);
                if (image.NewsId == news.Id)
                    continue;
                if (image != null && image.NewsId == 0)
                {
                    image.NewsId = news.Id;
                    _context.Images.Update(image);
                }
            }

            foreach (NewsAttribute newsAttribute in news.NewsAttribute)
            {
                var attr = _context.NewsAttribute.SingleOrDefault(x => x.NewsId == news.Id && x.AttributeId == newsAttribute.AttributeId);
                if (attr == null)
                {
                    newsAttribute.NewsId = news.Id;
                    _context.NewsAttribute.Add(newsAttribute);
                }
                else
                {
                    if (attr.Value != newsAttribute.Value)
                    {
                        attr.Value = newsAttribute.Value;
                        _context.NewsAttribute.Update(attr);
                    }
                }
            }

            _context.SaveChanges();
            news.Images = _context.Images.Where(x => x.NewsId == news.Id).ToList();
            return news;
        }

        public News ChangeStatus(int Id, int status, int userId)
        {
            var news = _context.News.SingleOrDefault(x => x.Id == Id);
            if (news == null)
                throw new AppException("News not found");

            if (news.OwnerId != userId)
                throw new AppException("User is not owner of this news");

            //int[] statuses = { News.ACTIVE, News.ARCHIVE };

            //if (!statuses.Contains(news.Status))
            //{
            //    throw new AppException("Action is not allowed");
            //}

            if (news.Status == News.ACTIVE && status == News.ARCHIVE)
            {
                news.Status = News.ARCHIVE;
                _context.News.Update(news);
            }

            int[] toStatuses = { News.DELETED, News.NEW };
            if (news.Status == News.ARCHIVE && toStatuses.Contains(status))
            {
                news.Status = status;
                _context.News.Update(news);
            }

            _context.SaveChanges();
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

        public PagedListNews GetAllByFilter(NewsSearch newsSearch)
        {
            var news = from n in _context.News
                       join u in _context.Users on n.OwnerId equals u.Id

                       join fav in _context.UserFavourites on n.Id equals fav.NewsId
                        into gj
                       from fav in gj.Where(x => x.UserId == newsSearch.UserId).DefaultIfEmpty()

                       join vfav in _context.VendorFavourite on n.OwnerId equals vfav.TargetUserId
                        into vf
                       from vfav in vf.Where(x => x.UserId == newsSearch.UserId).DefaultIfEmpty()

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

                       join tariff in _context.Tariff on n.Id equals tariff.NewsId
                       into tariff
                       from tariffs in tariff.Where(x => x.ExpireDate >= DateHelper.GetDate()).DefaultIfEmpty()
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
                           Tariffs = tariff.ToList(),
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
                               Rating = vr.DefaultIfEmpty().Average(c => Convert.ToInt32(c.Mark)).ToString()
                           },
                       };




            news = Query(news, newsSearch);
            news = news.GroupBy(item => item.Id).Select(group => group.FirstOrDefault());
            return PagedListNews.ToPagedList(news, newsSearch.PageNumber, newsSearch.PageSize, _context);
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
            if (image != null)
            {
                _context.Images.Remove(image);
                _context.SaveChanges();
                return true;
            }
            throw new AppException("Image Not Found");
        }

        public PagedListNews GetAllFavourites(NewsSearch newsSearch)
        {

            var news = from n in _context.News
                       join fav in _context.UserFavourites on n.Id equals fav.NewsId
                       where fav.UserId == newsSearch.UserId

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
                       };

            news = Query(news, newsSearch);

            news = news.GroupBy(item => item.Id).Select(group => group.FirstOrDefault());
            return PagedListNews.ToPagedList(news, newsSearch.PageNumber, newsSearch.PageSize, _context);
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

        public PagedListNews GetAllByTariff(NewsSearch newsSearch)
        {
            int[] tariffs = { Tariff.MAIN, Tariff.VIP, Tariff.TOP };
            if (!tariffs.Contains(newsSearch.Type))
                throw new AppException("Wrong Tariff Type");
            var news = from n in _context.News
                       join tariff in _context.Tariff on n.Id equals tariff.NewsId
                       where tariff.Type == newsSearch.Type && tariff.ExpireDate >= DateHelper.GetDate()

                       join fav in _context.UserFavourites on n.Id equals fav.NewsId
                       into gj
                       from fav in gj.Where(x => x.UserId == newsSearch.UserId).DefaultIfEmpty()

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
                       };

            news = Query(news, newsSearch);

            news = news.GroupBy(item => item.Id).Select(group => group.FirstOrDefault());
            return PagedListNews.ToPagedList(news, newsSearch.PageNumber, newsSearch.PageSize, _context);
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
        public bool UpdateRegions()
        {
            UpdateRegionsByLang("ru");
            UpdateRegionsByLang("uz");
            UpdateRegionsByLang("oz");
            return true;
        }

        private bool UpdateRegionsByLang(string lang)
        {
            var regions = _context.Region;

            var reg = regions.Where(x => x.Available == 1 && x.Depth == 2 && x.Lang == lang).OrderBy(x => x.Title);
            foreach (Region r in reg)
            {
                r.Children = regions.Where(x => x.Available == 1 && x.SoatoId == r.ParentId && x.Lang == lang).OrderBy(x => x.Title).ToList();
            }
            string json = JsonConvert.SerializeObject(reg.ToList());
            System.IO.File.WriteAllText(@"regions_" + lang + ".json", json);
            return true;
        }

        public IEnumerable<Region> GetRegions(string lang = "ru")
        {
            var regions = JsonConvert.DeserializeObject<IEnumerable<Region>>(System.IO.File.ReadAllText(@"regions_" + lang + ".json"));
            return _mapper.Map<IEnumerable<Region>>(regions);
        }

        protected IQueryable<News> Query(IQueryable<News> news, NewsSearch newsSearch)
        {
            if (newsSearch.Attributes != null && newsSearch.Attributes.Count > 0)
            {
                news = from n in news
                       join atr in _context.NewsAttribute on n.Id equals atr.NewsId
                       select n;
                foreach (var attr in newsSearch.Attributes)
                {
                    if (!string.IsNullOrEmpty(attr.Value))
                    {
                        news = from n in news
                               join atr in _context.NewsAttribute on n.Id equals atr.NewsId
                               where atr.AttributeId == attr.AttributeId && atr.Value == attr.Value
                               select n;
                    }
                    else if (attr.ValueFrom > 0 && attr.ValueTo > 0)
                    {
                        news = from n in news
                               join atr in _context.NewsAttribute on n.Id equals atr.NewsId
                               where atr.AttributeId == attr.AttributeId && Convert.ToInt32(atr.Value) >= attr.ValueFrom && Convert.ToInt32(atr.Value) <= attr.ValueTo
                               select n;
                    }
                    else if (attr.ValueFrom > 0)
                    {
                        news = from n in news
                               join atr in _context.NewsAttribute on n.Id equals atr.NewsId
                               where atr.AttributeId == attr.AttributeId && Convert.ToInt32(atr.Value) >= attr.ValueFrom
                               select n;
                    }
                    else if (attr.ValueTo > 0)
                    {
                        news = from n in news
                               join atr in _context.NewsAttribute on n.Id equals atr.NewsId
                               where atr.AttributeId == attr.AttributeId && Convert.ToInt32(atr.Value) <= attr.ValueTo
                               select n;
                    }
                }
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

            if (newsSearch.OwnerId > 0)
            {
                news = news.Where(x => x.OwnerId == newsSearch.OwnerId);
            }

            int[] statuses = { News.NEW, News.ACTIVE, News.ARCHIVE, News.REJECTED, News.DELETED };

            if (statuses.Contains(newsSearch.Status))
            {
                news = news.Where(x => x.Status == newsSearch.Status);
            }

            if (newsSearch.Price != null)
            {
                if (newsSearch.Price.AmountFrom > 0)
                {
                    news = news.Where(x => x.Price.Amount >= newsSearch.Price.AmountFrom);
                }
                if (newsSearch.Price.AmountTo > 0)
                {
                    news = news.Where(x => x.Price.Amount <= newsSearch.Price.AmountTo);
                }
                if (newsSearch.Price.Currency > 0)
                {
                    news = news.Where(x => x.Price.Currency == newsSearch.Price.Currency);
                }
            }

            if (newsSearch.Location != null)
            {
                if (newsSearch.Location.RegionId > 0)
                {
                    news = news.Where(x => x.Location.RegionId == newsSearch.Location.RegionId);
                }
                if (newsSearch.Location.DistrictId > 0)
                {
                    news = news.Where(x => x.Location.DistrictId == newsSearch.Location.DistrictId);
                }
                if (!string.IsNullOrEmpty(newsSearch.Location.Longtitude))
                {
                    news = news.Where(x => x.Location.Longtitude.Contains(newsSearch.Location.Longtitude));
                }
                if (!string.IsNullOrEmpty(newsSearch.Location.Latitude))
                {
                    news = news.Where(x => x.Location.Latitude.Contains(newsSearch.Location.Latitude));
                }
            }

            return news;
        }
    }
}
