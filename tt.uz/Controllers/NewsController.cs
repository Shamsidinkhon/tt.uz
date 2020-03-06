using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using tt.uz.Dtos;
using tt.uz.Entities;
using tt.uz.Helpers;
using tt.uz.Services;
using Newtonsoft.Json;

namespace tt.uz.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private IMapper _mapper;
        private INewsService _newsService;
        private IHttpContextAccessor _httpContextAccessor;
        private IVendorReviewService _vendorReviewService;
        private readonly AppSettings _appSettings;
        public NewsController(
            IMapper mapper,
            INewsService newsService,
             IVendorReviewService vendorReviewService,
            IHttpContextAccessor httpContextAccessor,
             IOptions<AppSettings> appSettings
            )
        {
            _mapper = mapper;
            _newsService = newsService;
            _httpContextAccessor = httpContextAccessor;
            _vendorReviewService = vendorReviewService;

        }
        [HttpPost("add")]
        public IActionResult Add([FromBody]NewsDTO newsDTO)
        {
            // map dto to entity
            var news = _mapper.Map<News>(newsDTO);
            news.OwnerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.Identity.Name);
            try
            {
                // save 
                var item = _newsService.Create(news, newsDTO.ImageIds);

                return Ok(new { status = true, data = item });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return Ok(new { code = false, message = ex.Message });
            }
        }

        /*         [HttpPost("change-status")]
                public IActionResult ChangeStatus(int newsId, int status)
                {
                    // map dto to entity
                    var news = _mapper.Map<News>(newsDTO);
                    news.OwnerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.Identity.Name);
                    try
                    {
                        // save 
                        var item = _newsService.Create(news, newsDTO.ImageIds);

                        return Ok(new { status = true, data = item });
                    }
                    catch (AppException ex)
                    {
                        // return error message if there was an exception
                        return Ok(new { status = false, message = ex.Message });
                    }
                } */

        [HttpPost("upload-image")]
        public IActionResult UploadImage(IFormFile image)
        {
            // map dto to entity
            try
            {
                return Ok(new { status = true, imageId = _newsService.UploadImage(image, Convert.ToInt32(_httpContextAccessor.HttpContext.User.Identity.Name)) });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return Ok(new { code = false, message = ex.Message });
            }
        }

        [HttpPost("delete-image")]
        public IActionResult DeleteImage(int imageId)
        {
            // map dto to entity
            try
            {
                return Ok(new { status = true, imageId = _newsService.DeleteImage(imageId, Convert.ToInt32(_httpContextAccessor.HttpContext.User.Identity.Name)) });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return Ok(new { code = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("get-all")]
        public PagedListNews GetAll([FromBody]NewsSearch newsSearch)
        {
            int[] statuses = { News.ACTIVE };

            if (!statuses.Contains(newsSearch.Status))
            {
                newsSearch.Status = News.ACTIVE;
            }
            newsSearch.OwnerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.Identity.Name);
            var news = _newsService.GetAllByFilter(newsSearch);

            var metadata = new
            {
                news.TotalCount,
                news.PageSize,
                news.CurrentPage,
                news.TotalPages,
                news.HasNext,
                news.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            Response.Headers.Add("Access-Control-Expose-Headers", "*");

            return news;
        }

        [HttpPost("get-all-by-user")]
        public PagedListNews GetAllByUser([FromBody]NewsSearch newsSearch)
        {
            newsSearch.OwnerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.Identity.Name);
            var news = _newsService.GetAllByFilter(newsSearch);
            var metadata = new
            {
                news.TotalCount,
                news.PageSize,
                news.CurrentPage,
                news.TotalPages,
                news.HasNext,
                news.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            Response.Headers.Add("Access-Control-Expose-Headers", "*");
            return news;
        }

        [HttpPost("post-favourite")]
        public IActionResult PostFavourite(int newsId)
        {
            try
            {
                UserFavourites uf = new UserFavourites();
                uf.NewsId = newsId;
                uf.UserId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.Identity.Name);
                return Ok(new { status = _newsService.PostFavourite(uf) });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return Ok(new { code = false, message = ex.Message });
            }
        }

        [HttpPost("delete-favourite")]
        public IActionResult DeleteFavourite(int newsId)
        {
            try
            {
                return Ok(new { status = _newsService.DeleteFavourite(newsId, Convert.ToInt32(_httpContextAccessor.HttpContext.User.Identity.Name)) });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return Ok(new { code = false, message = ex.Message });
            }
        }

        [HttpPost("get-all-favourites")]
        public PagedListNews GetAllFavourites([FromBody]NewsSearch newsSearch)
        {
            newsSearch.OwnerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.Identity.Name);
            var news = _newsService.GetAllFavourites(newsSearch);
            var metadata = new
            {
                news.TotalCount,
                news.PageSize,
                news.CurrentPage,
                news.TotalPages,
                news.HasNext,
                news.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            Response.Headers.Add("Access-Control-Expose-Headers", "*");
            return news;
        }

        [HttpPost("post-tariff")]
        public IActionResult PostTariff([FromBody]TariffDTO tariffDto)
        {
            try
            {
                var tariff = _mapper.Map<Tariff>(tariffDto);
                tariff.UserId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.Identity.Name);
                return Ok(new { status = _newsService.PostTariff(tariff) });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return Ok(new { code = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("get-all-by-tariff")]
        public PagedListNews GetAllByTariff([FromBody]NewsSearch newsSearch)
        {
            newsSearch.OwnerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.Identity.Name);
            var news = _newsService.GetAllByTariff(newsSearch);
            var metadata = new
            {
                news.TotalCount,
                news.PageSize,
                news.CurrentPage,
                news.TotalPages,
                news.HasNext,
                news.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            Response.Headers.Add("Access-Control-Expose-Headers", "*");
            return news;
        }

        [HttpPost("post-vendor-favourite")]
        public IActionResult PostVendorFavourite(int targetUserId)
        {
            try
            {
                VendorFavourite vf = new VendorFavourite();
                vf.TargetUserId = targetUserId;
                vf.UserId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.Identity.Name);
                return Ok(new { status = _newsService.PostVendorFavourite(vf) });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return Ok(new { code = false, message = ex.Message });
            }
        }

        [HttpPost("delete-vendor-favourite")]
        public IActionResult DeleteVendorFavourite(int targetUserId)
        {
            try
            {
                return Ok(new { status = _newsService.DeleteVendorFavourite(targetUserId, Convert.ToInt32(_httpContextAccessor.HttpContext.User.Identity.Name)) });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return Ok(new { code = false, message = ex.Message });
            }
        }

        [HttpPost("get-vendors")]
        public List<UserProfile> GetVendorFavourites()
        {
            return _newsService.GetVendors(Convert.ToInt32(_httpContextAccessor.HttpContext.User.Identity.Name));
        }

        [HttpPost("post-vendor-review")]
        public IActionResult PostVendorReview([FromBody]VendorReviews vendorReviews)
        {
            try
            {
                vendorReviews.UserId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.Identity.Name);
                return Ok(new { status = _vendorReviewService.Add(vendorReviews) });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return Ok(new { code = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("get-vendor-reviews")]
        public IActionResult GetVendorReviews(int targetUserId)
        {
            try
            {
                return Ok(_vendorReviewService.GetAll(targetUserId));
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return Ok(new { code = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("search")]
        public List<Category> Search([FromBody]NewsSearch newsSearch)
        {
            int[] statuses = { News.ACTIVE };

            if (!statuses.Contains(newsSearch.Status))
            {
                newsSearch.Status = News.ACTIVE;
            }
            return _newsService.Search(newsSearch);
        }

        [AllowAnonymous]
        [HttpGet("update-regions")]
        public IActionResult UpdateRegions()
        {
            try
            {
                return Ok(_newsService.UpdateRegions());
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return Ok(new { code = false, message = ex.Message });
            }

        }
        [AllowAnonymous]
        [HttpGet("get-regions")]
        public IActionResult GetRegions(string lang)
        {
            try
            {
                string[] langs = { "ru", "uz", "oz" };
                if (string.IsNullOrEmpty(lang) || !langs.Contains(lang))
                    lang = "ru";
                IEnumerable<Region> regions = _newsService.GetRegions(lang);
                return Ok(regions);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return Ok(new
                {
                    code = false,
                    message = ex.Message
                });
            }

        }
    }
}