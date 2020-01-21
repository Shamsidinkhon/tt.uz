using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Dtos;
using tt.uz.Entities;
using tt.uz.Helpers;

namespace tt.uz.Services
{
    public interface ICategoryService
    {
        IEnumerable<CategoryDTO> GetAll();
        IEnumerable<CategoryDTO> GetWithChildren();
        List<CoreAttribute> GetCategoryAttribites(int catId);
        bool UpdateCategories();
    }

    public class CategoryService : ICategoryService
    {
        private DataContext _context;
        private IMapper _mapper;

        public CategoryService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IEnumerable<CategoryDTO> GetAll()
        {
            var cats = JsonConvert.DeserializeObject<IEnumerable<CategoryDTO>>(System.IO.File.ReadAllText(@"categories.json"));
            return _mapper.Map<IEnumerable<CategoryDTO>>(cats);
            // return _context.Categories;
        }

        public IEnumerable<CategoryDTO> GetWithChildren()
        {
            var cats = JsonConvert.DeserializeObject<IEnumerable<CategoryDTO>>(System.IO.File.ReadAllText(@"categoriesWithChildren.json"));
            return _mapper.Map<IEnumerable<CategoryDTO>>(cats);
            //var cats =  _context.Categories.Where(x => x.ParentId == null || x.ParentId.ToString() == "");
            //foreach (Category cat in cats) {
            //    cat.Children = GetChilds(cat.Id, cats);
            //    foreach (Category child in cat.Children) {
            //        child.Children = GetChilds(child.Id, cats);
            //    }
            //}
            //return cats;
        }

        private List<Category> GetChilds(int parentId)
        {
            return _context.Categories.Where(x => x.ParentId == parentId).ToList();
        }

        public List<CoreAttribute> GetCategoryAttribites(int catId)
        {
            var cat = _context.Categories.SingleOrDefault(x => x.Id == catId);
            if (cat == null)
                throw new AppException("Category Not Found");
            var attributes = from a in _context.CoreAttribute
                             join al in _context.AttributeLink on a.Id equals al.AttributeId
                             where al.TypeId == cat.AttributeType
                             select new CoreAttribute()
                             {
                                 Id = a.Id,
                                 Name = a.Name,
                                 Title = a.Title,
                                 Type = a.Type,
                                 Unit = a.Unit,
                                 Required = a.Required,
                                 AttributeOptions = _context.AttributeOption.Where(x => x.AttributeId == a.Id).ToList()
                             };
            return attributes.ToList();
        }

        public bool UpdateCategories()
        {
            var categories = _context.Categories;
            string json = JsonConvert.SerializeObject(categories.ToList().ToArray());
            System.IO.File.WriteAllText(@"categories.json", json);

            var cats = categories.Where(x => x.ParentId == null || x.ParentId.ToString() == "");
            foreach (Category cat in cats)
            {
                cat.Children = GetChilds(cat.Id);
                foreach (Category child in cat.Children)
                {
                    child.Children = GetChilds(child.Id);
                }
            }
            json = JsonConvert.SerializeObject(cats.ToList().ToArray());
            System.IO.File.WriteAllText(@"categoriesWithChildren.json", json);
            return true;
        }
    }
}
