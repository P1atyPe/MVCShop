using Microsoft.EntityFrameworkCore;
using MVCShop.Models;
using MVCShop.Models.Comments;
using MVCShop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCShop.Data.Repository
{
    public class Repository : IRepository
    {
        private AppDbContext _ctx;

        public Repository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public void AddPost(Post post)
        {
            _ctx.Posts.Add(post);
        }

        public List<Post> GetAllPosts()
        {
            return _ctx.Posts.ToList();
        }

        public IndexViewModel GetAllPosts(int pageNumber, string category)
        {
            //Func<Post, bool> InCategory = (post) => { return post.Category.ToLower().Equals(category.ToLower()); };
            int pageSize = 1;
            int skipAmount = pageSize * (pageNumber - 1);
            int capacity = skipAmount + pageSize;

            var query = _ctx.Posts.AsQueryable();

            if (!String.IsNullOrEmpty(category))
                query = query.Where(x => x.Category.ToLower().Equals(category.ToLower()));
            int postsCount = query.Count();
            int pageCount = (int)Math.Ceiling(postsCount * 1.0 / pageSize);

            return new IndexViewModel
            {
                PageNumber = pageNumber,
                PageCount = pageCount,
                NextPage = postsCount > capacity,
                Pages = PageNumbers(pageNumber, pageCount),
                Category = category,
                Posts = query
                    .Skip(skipAmount)
                    .Take(pageSize)
                    .ToList()
            };
        }

        private IEnumerable<int> PageNumbers(int pageNumber, int pageCount)
        {
            List<int> pages = new List<int>();

            int midPoint = pageNumber;
            if (midPoint < 3)
            {
                midPoint = 3;
            }
            else if (midPoint > pageCount - 2)
            {
                midPoint = pageCount - 2;
            }

            for (int i = midPoint - 2; i <= midPoint + 2; i++)
            {
                pages.Add(i);
            }

            if (pages[0] != 1)
            {
                pages.Insert(0, 1);
                if (pages[1] - pages[0] > 1)
                {
                    pages.Insert(1, -1);
                }
            }

            if (pages[pages.Count - 1] != pageCount)
            {
                pages.Insert(pages.Count, pageCount);
                if (pages[pages.Count - 1] - pages[pages.Count - 2] > 1)
                {
                    pages.Insert(pages.Count - 1, -1);
                }
            }

            return pages;
        }

        public Post GetPost(int id)
        {
            return _ctx.Posts
                .Include(p => p.MainComments)
                .ThenInclude(mc => mc.SubComments)
                .FirstOrDefault(p => p.Id == id);
        }

        public void RemovePost(int id)
        {
            _ctx.Posts.Remove(GetPost(id));
        }


        public void UpdatePost(Post post)
        {
            _ctx.Posts.Update(post);
        }

        public async Task<bool> SaveChangesAsync()
        {
            if (await _ctx.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }

        public void AddSubComment(SubComment comment)
        {
            _ctx.SubComments.Add(comment);
        }
    }
}
