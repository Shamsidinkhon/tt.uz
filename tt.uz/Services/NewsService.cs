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
        News Create(News news, List<IFormFile> images);
        IQueryable<News> GetAllByFilter(NewsSearch newsSearch);
        bool PostFavourite(UserFavourites uf);
    }
    public class NewsService : INewsService
    {
        private DataContext _context;
        public NewsService(DataContext context) {
            _context = context;
        }
        public News Create(News news, List<IFormFile> files)
        {

            _context.News.Add(news);

            _context.SaveChanges();

            var folderName = Path.Combine("Resources", "Images");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            string[] extArray = { ".png", ".jpeg", ".jpg"};
            if (files.Any(f => f.Length == 0) || files.Any(f => Array.IndexOf(extArray, Path.GetExtension(ContentDispositionHeaderValue.Parse(f.ContentDisposition).FileName.Trim('"'))) == -1))
            {
                _context.Remove(news);
                _context.Remove(news.ContactDetail);
                _context.Remove(news.Location);
                _context.Remove(news.Price);
                _context.SaveChanges();
                throw new AppException("File size should be more than 0!; PNG, JPEG, JPG extensions are allowed");
            }

            foreach (var file in files)
            {
                var fileName = string.Concat(Guid.NewGuid(), Path.GetExtension(ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"')));
                var fullPath = Path.Combine(pathToSave, fileName);
                var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require

                Image image = new Image();
                image.Path = dbPath;
                image.NewsId = news.Id;
                _context.Images.Add(image);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            _context.SaveChanges();

            return news;
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
    }
}
