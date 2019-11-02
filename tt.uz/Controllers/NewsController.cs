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
using tt.uz.Dtos;
using tt.uz.Entities;
using tt.uz.Helpers;
using tt.uz.Services;

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
        public NewsController(
            IMapper mapper,
            INewsService newsService,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _mapper = mapper;
            _newsService = newsService;
            _httpContextAccessor = httpContextAccessor;

        }
        [HttpPost("add")]
        public IActionResult Add([FromForm]NewsDTO newsDTO)
        {
            // map dto to entity
            var news = _mapper.Map<News>(newsDTO);
            news.OwnerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.Identity.Name);
            try
            {
                // save 
                _newsService.Create(news, newsDTO.ImageIds);

                return Ok(new { status = true });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return Ok(new { status = false, message = ex.Message });
            }
        }

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
                return Ok(new { status = false, message = ex.Message });
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
                return Ok(new { status = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("get-all")]
        public IQueryable<News> GetAll([FromBody]NewsSearch newsSearch)
        {
            int[] statuses = { News.ACTIVE };

            if (!statuses.Contains(newsSearch.Status))
            {
                newsSearch.Status = News.ACTIVE;
            }
            return _newsService.GetAllByFilter(newsSearch);
        }

        [HttpPost("get-all-by-user")]
        public IQueryable<News> GetAllByUser([FromBody]NewsSearch newsSearch)
        {
            newsSearch.OwnerId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.Identity.Name);
            return _newsService.GetAllByFilter(newsSearch);
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
                return Ok(new { status = false, message = ex.Message });
            }
        }
    }
}