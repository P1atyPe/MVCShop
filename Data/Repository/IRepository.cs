using MVCShop.Models;
using MVCShop.Models.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCShop.Data.Repository
{
    public interface IRepository
    {
        Post GetPost(int id);
        List<Post> GetAllPosts();
        List<Post> GetAllPosts(string Category);
        void AddPost(Post post);
        void UpdatePost(Post post);
        void RemovePost(int id);

        void AddSubComment(SubComment comment);

        Task<bool> SaveChangesAsync();
    }
}
