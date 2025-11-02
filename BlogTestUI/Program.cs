using BlogDataLibrary.Data;
using BlogDataLibrary.Database;
using BlogDataLibrary.Models;
using Microsoft.Extensions.Configuration;

internal class Program
{
    static void Main(string[] args)
    {
        var db = GetConnection();

        Console.WriteLine("1. Register new user");
        Console.WriteLine("2. Login");
        Console.Write("Choose option: ");
        var choice = Console.ReadLine();

        if (choice == "1")
        {
            Register(db);
        }
        else if (choice == "2")
        {
            var user = Authenticate(db);
            if (user == null) return;

            while (true)
            {
                Console.WriteLine("\n1. Add Post");
                Console.WriteLine("2. List Posts");
                Console.WriteLine("3. Show Post Details");
                Console.WriteLine("Any other key to exit");
                Console.Write("Enter option: ");
                var next = Console.ReadLine();

                if (next == "1") AddPost(db, user);
                else if (next == "2") ListPosts(db);
                else if (next == "3") ShowPostDetails(db);
                else break;
            }
        }
        else
        {
            Console.WriteLine("Invalid choice.");
        }

        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }

    static SqlData GetConnection()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var config = builder.Build();

        ISqlDataAccess dbAccess = new SqlDataAccess(config);
        return new SqlData(dbAccess);
    }

    static UserModel? GetCurrentUser(SqlData db)
    {
        Console.Write("Username: ");
        string? username = Console.ReadLine()?.Trim();

        Console.Write("Password: ");
        string? password = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return null;

        return db.Authenticate(username, password);
    }

    public static UserModel? Authenticate(SqlData db)
    {
        var user = GetCurrentUser(db);
        Console.WriteLine(user == null ? "Invalid credentials." : $"Welcome, {user.UserName}");
        return user;
    }

    public static void Register(SqlData db)
    {
        Console.Write("Enter new username: ");
        var username = Console.ReadLine()?.Trim();

        Console.Write("Enter new password: ");
        var password = Console.ReadLine();

        Console.Write("Enter first name: ");
        var firstName = Console.ReadLine();

        Console.Write("Enter last name: ");
        var lastName = Console.ReadLine();

        db.RegisterUser(username!, firstName!, lastName!, password!);
        Console.WriteLine("Registration successful!");
    }

    public static void AddPost(SqlData db, UserModel user)
    {
        Console.Write("Title: ");
        string title = Console.ReadLine() ?? string.Empty;

        Console.Write("Write body: ");
        string body = Console.ReadLine() ?? string.Empty;

        var post = new PostModel
        {
            Title = title,
            Body = body,
            DateCreated = DateTime.Now,
            UserId = user.Id
        };

        db.AddPost(post);
        Console.WriteLine("Post saved.");
    }

    public static void ListPosts(SqlData db)
    {
        var posts = db.ListPosts();

        foreach (var post in posts)
        {
            string body = post.Body ?? string.Empty;
            string preview = body.Length <= 20 ? body : body.Substring(0, 20) + "...";

            Console.WriteLine($"{post.Id}. Title: {post.Title} by {post.UserName} [{post.DateCreated:yyyy-MM-dd}]");
            Console.WriteLine(preview);
            Console.WriteLine();
        }
    }

    public static void ShowPostDetails(SqlData db)
    {
        Console.Write("Enter a post ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var post = db.ShowPostDetails(id);
        if (post is null)
        {
            Console.WriteLine("Post not found.");
            return;
        }

        Console.WriteLine(post.Title);
        Console.WriteLine($"by {post.FirstName} {post.LastName} [{post.UserName}]");
        Console.WriteLine();
        Console.WriteLine(post.Body);
        Console.WriteLine(post.DateCreated.ToString("MMM d yyyy"));
    }
}
