﻿using Microsoft.AspNetCore.Http;
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
        List<News> GetAllByFilter(NewsSearch newsSearch, int userId);
        bool PostFavourite(UserFavourites uf);
        bool DeleteFavourite(int newsId, int userId);
        int UploadImage(IFormFile file, int userId);
        bool DeleteImage(int imageId, int userId);
        List<News> GetAllFavourites(int userId);
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

        public List<News> GetAllByFilter(NewsSearch newsSearch, int userId)
        {

            var news = from n in _context.News
                       join fav in _context.UserFavourites on n.Id equals fav.NewsId
                        into gj  
                       from fav in gj.Where(x => x.UserId == userId).DefaultIfEmpty() 
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
                           Favourite = fav == null ? false : true
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

            return news.ToList();
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
            _context.Images.Remove(image);
            _context.SaveChanges();
            return true;
        }

        public List<News> GetAllFavourites(int userId)
        {

            var news = from n in _context.News
                       join fav in _context.UserFavourites on n.Id equals fav.NewsId
                       where fav.UserId == userId
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
                           Favourite = false
                       };

            return news.ToList();
        }

        public bool DeleteFavourite(int newsId, int userId) {
            var fav = _context.UserFavourites.SingleOrDefault(x => x.NewsId == newsId && x.UserId == userId);
            _context.UserFavourites.Remove(fav);
            _context.SaveChanges();
            return true;
        }
    }
}
