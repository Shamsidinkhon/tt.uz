using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using tt.uz.Entities;
using tt.uz.Helpers;

namespace tt.uz.Services
{
    public interface INewsService{
        News Create(News news, List<IFormFile> images);
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
    }
}
