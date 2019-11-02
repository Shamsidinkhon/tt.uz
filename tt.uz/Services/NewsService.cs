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
    public interface INewsService{
        News Create(News news, string imageIds);
        IQueryable<News> GetAllByFilter(NewsSearch newsSearch);
        bool PostFavourite(UserFavourites uf);
        int UploadImage(IFormFile file, int userId);
        bool DeleteImage(int imageId, int userId);
    }
    public class NewsService : INewsService
    {
        private DataContext _context;
        public NewsService(DataContext context) {
            _context = context;
        }
        public News Create(News news, string imageIds)
        {
            _context.News.Add(news);
            _context.SaveChanges();
            var ids = imageIds.Split(',').Select(Int32.Parse).ToList();
            foreach(int id in ids) {
                var image = _context.Images.SingleOrDefault(x => x.ImageId == id);
                if (image != null) {
                    image.NewsId = news.Id;
                    _context.Images.Update(image);
                }
            }
            _context.SaveChanges();
            return news;
        }

        public int UploadImage(IFormFile file, int userId) {
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
                image.Path = dbPath;
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

        public IQueryable<News> GetAllByFilter(NewsSearch newsSearch)
        {

            IQueryable<News> news = from n in _context.News select n;

            news = news
                .Include(x => x.Category)
                .Include(x => x.Price)
                .Include(x => x.Location)
                .Include(x => x.ContactDetail);

            if (newsSearch.OwnerId > 0) {
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

            if (!String.IsNullOrEmpty(newsSearch.Title))
            {
                news = news.Where(x => x.Title.Contains(newsSearch.Title));
            }

            return news;
        }

        public bool PostFavourite(UserFavourites uf) {
            _context.UserFavourites.Add(uf);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteImage(int imageId, int userId) {
            var image = _context.Images.SingleOrDefault(x => x.ImageId == imageId && x.UserId == userId);
            _context.Images.Remove(image);
            _context.SaveChanges();
            return true;
        }
    }
}
