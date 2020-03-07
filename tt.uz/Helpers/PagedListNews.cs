using System;
using System.Collections.Generic;
using System.Linq;
using tt.uz.Entities;

namespace tt.uz.Helpers
{
    public class PagedListNews : List<News>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        private DataContext _context;

        public PagedListNews(List<News> items, int count, int pageNumber, int pageSize, DataContext context)
        {
            _context = context;
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            items.ForEach(SetValues);
            AddRange(items);
        }

        public static PagedListNews ToPagedList(IEnumerable<News> source, int pageNumber, int pageSize, DataContext context)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PagedListNews(items, count, pageNumber, pageSize, context);
        }
        protected void SetValues(News news)
        {
            news.NewsAttribute = (from newsAtr in _context.NewsAttribute
                                  where newsAtr.NewsId == news.Id
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
                                            ).ToList();
        }
    }
}