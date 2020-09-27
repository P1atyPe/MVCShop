﻿using Microsoft.AspNetCore.Mvc;
using MVCShop.Data;
using MVCShop.Data.FileManager;
using MVCShop.Data.Repository;
using MVCShop.Models;
using MVCShop.Models.Comments;
using MVCShop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCShop.Controllers
{
    public class HomeController : Controller
    {
        private IRepository _repo;
        private IFileManager _fileManager;

        public HomeController(
            IRepository repo,
            IFileManager fileManager)
        {
            _repo = repo;
            _fileManager = fileManager;
        }

        public IActionResult Index(string category)
        {
            var posts = string.IsNullOrEmpty(category) ? _repo.GetAllPosts() : _repo.GetAllPosts(category);

            return View(posts);
        }

        public IActionResult Post(int id)
        {
            var post = _repo.GetPost(id);

            return View(post);
        }
        
        [HttpGet("/Image/{image}")]
        [ResponseCache(CacheProfileName = "Monthly")]
        public IActionResult Image(string image)
        {
            var mime = image.Substring(image.LastIndexOf('.') + 1);
            return new FileStreamResult(_fileManager.ImageStream(image), $"image/{mime}");
        }

        [HttpPost]
        public async Task<IActionResult> Comment(CommentViewModel vm)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Post", new { id = vm.PostId });

            var post = _repo.GetPost(vm.PostId);
            if (vm.MainCommentId == 0)
            {
                post.MainComments = post.MainComments ?? new List<MainComment>();

                post.MainComments.Add(new MainComment
                {
                    Message = vm.Message,
                    Created = DateTime.Now,
                });

                _repo.UpdatePost(post);
            }
            else
            {
                var comment = new SubComment
                {
                    MainCommentId = vm.MainCommentId,
                    Message = vm.Message,
                    Created = DateTime.Now,
                };
                _repo.AddSubComment(comment);
            }

            await _repo.SaveChangesAsync();

            return RedirectToAction("Post", new { id = vm.PostId });

        }

    }
}
