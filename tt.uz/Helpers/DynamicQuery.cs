using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using tt.uz.Entities;

namespace tt.uz.Helpers
{
    public class DynamicQuery
    {
        public System.Linq.IQueryable<News> QueryNewsAttributesByCondition(System.Linq.IQueryable<News> news, NewsSearch newsSearch, DataContext _context)
        {
            string AttributeConditions = "";
            if (newsSearch.Attributes != null && newsSearch.Attributes.Count > 0)
            {
                foreach (var attr in newsSearch.Attributes)
                {
                    if (!string.IsNullOrEmpty(attr.Value))
                    {
                        AttributeConditions += AttributeConditions + "AND (NewsAttribute.Id = " + attr.AttributeId + " AND NewsAttribute.Value = '" + attr.Value + "')";
                    }
                    else if (attr.ValueFrom > 0 && attr.ValueTo > 0)
                    {
                        AttributeConditions += AttributeConditions + "AND (NewsAttribute.Id = " + attr.AttributeId + " AND NewsAttribute.Value >= CONVERT(int, " + attr.ValueFrom + ") AND NewsAttribute.Value <= CONVERT(int, " + attr.ValueTo + "))";
                    }
                }
            }
            if (!string.IsNullOrEmpty(AttributeConditions))
            {
                news = news.Where(AttributeConditions);
            }
            return news;
        }
    }
}
