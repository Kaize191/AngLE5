using BlogDataLibrary.Database;
using BlogDataLibrary.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDataLibrary.Data
{
    public class SqlData
    {
        private readonly ISqlDataAccess _db;
        private const string connectionStringName = "SqlDb";

        public SqlData(ISqlDataAccess db)
        {
            _db = db;
        }

        public UserModel? Authenticate(string username, string password)
        {
            UserModel? result = _db.LoadData<UserModel, dynamic>(
                "dbo.spUsers_Authenticate",
                new { username, password },
                connectionStringName,
                true
            ).FirstOrDefault();

            return result;
        }

        public void RegisterUser(string username, string firstName, string lastName, string password)
        {
            _db.SaveData(
                "dbo.spUsers_Register",
                new { username, firstName, lastName, password },
                connectionStringName,
                true
            );
        }

        public void AddPost(PostModel post)
        {
            _db.SaveData(
                "dbo.spPosts_Insert",
                new
                {
                    Title = post.Title,
                    Body = post.Body,
                    UserId = post.UserId,
                    DateCreated = post.DateCreated
                },
                "SqlDb",
                true
            );
        }

        public List<ListPostModel> ListPosts()
        {
            return _db.LoadData<ListPostModel, dynamic>(
                "dbo.spPosts_List",
                new { },
                "SqlDb",
                true
            );
        }

        public PostModel? ShowPostDetails(int id)
        {
            var rows = _db.LoadData<PostModel, dynamic>(
                "dbo.spPosts_Detail",
                new { Id = id },
                "SqlDb",
                true
            );

            return rows.FirstOrDefault();
        }
    }
}
