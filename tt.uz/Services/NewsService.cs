using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Entities;
using tt.uz.Helpers;

namespace tt.uz.Services
{
    public interface INewsService{
        News Create(News news);
    }
    public class NewsService : INewsService
    {
        private DataContext _context;
        public NewsService(DataContext context) {
            _context = context;
        }
        public News Create(News news)
        {

            _context.News.Add(news);

            //_context.SaveChanges();

            return news;
        }
    }
}
