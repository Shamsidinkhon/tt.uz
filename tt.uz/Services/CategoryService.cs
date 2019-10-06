using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Entities;
using tt.uz.Helpers;

namespace tt.uz.Services
{
    public interface ICategoryService
    {
        IEnumerable<Category> GetAll();
        IEnumerable<Category> GetWithChildren();
    }

    public class CategoryService : ICategoryService
    {
        private DataContext _context;

        public CategoryService(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<Category> GetAll()
        {
            return JsonConvert.DeserializeObject<IEnumerable<Category>>(System.IO.File.ReadAllText(@"categories.json"));
            //return _context.Categories;
        }

        public IEnumerable<Category> GetWithChildren()
        {
            return JsonConvert.DeserializeObject<IEnumerable<Category>>(System.IO.File.ReadAllText(@"categoriesWithChildren.json"));
            //var cats =  _context.Categories.Where(x => x.ParentId == null || x.ParentId.ToString() == "");
            //foreach (Category cat in cats) {
            //    cat.Children = GetChilds(cat.Id, cats);
            //    foreach (Category child in cat.Children) {
            //        child.Children = GetChilds(child.Id, cats);
            //    }
            //}
            //return cats;
        }

        private IEnumerable<Category>  GetChilds(int parentId, IEnumerable<Category> cats) {
            return cats.Where(x => x.ParentId == parentId);
        }

    }
}
