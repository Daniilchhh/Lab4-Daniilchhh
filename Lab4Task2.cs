using System;
using System.Collections.Generic;
using System.Text;

namespace Lab4Task2
{
    struct Comment
    {
        public string AuthorNickname;
        public string Text;
        public DateTime Date;

        public Comment(string authorNickname, string text)
        {
            AuthorNickname = authorNickname;
            Text = text;
            Date = DateTime.Now;
        }
    }

    class Post
    {
        private static int nextId = 1;

        public int Id { get; }
        public DateTime CreatedAt { get; }
        public string Text { get; }
        public int Likes { get; private set; }
        public List<Comment> Comments { get; } = new List<Comment>();

        public Post(string text, DateTime createdAt)
        {
            Id = nextId++;
            Text = text;
            CreatedAt = createdAt;
            Likes = 0;
        }

        public void AddLike()
        {
            Likes++;
        }

        public void AddComment(Comment comment)
        {
            Comments.Add(comment);
        }

        public void Print()
        {
            Console.WriteLine($"[Пост №{Id}] Дата створення: {CreatedAt:dd.MM.yyyy HH:mm}");
            Console.WriteLine($"  Текст: {Text}");
            Console.WriteLine($"  Лайків: {Likes}");

            if (Comments.Count == 0)
            {
                Console.WriteLine("  Коментарі: немає");
            }
            else
            {
                Console.WriteLine("  Коментарі:");
                foreach (Comment c in Comments)
                {
                    Console.WriteLine($"    {c.AuthorNickname} ({c.Date:dd.MM.yyyy HH:mm}): {c.Text}");
                }
            }
        }
    }

    class User
    {
        public string Name { get; }
        public string Email { get; }
        public string Nickname { get; }
        public List<Post> Posts { get; } = new List<Post>();

        public User(string name, string email, string nickname)
        {
            Name = name;
            Email = email;
            Nickname = nickname.StartsWith("@") ? nickname : "@" + nickname;
        }

        public Post CreatePost(string text)
        {
            return CreatePost(text, DateTime.Now);
        }

        public Post CreatePost(string text, DateTime date)
        {
            Post post = new Post(text, date);
            Posts.Add(post);
            return post;
        }

        public bool DeletePost(int postId)
        {
            Post post = FindPost(postId);
            if (post == null)
            {
                return false;
            }
            Posts.Remove(post);
            return true;
        }

        public Post FindPost(int postId)
        {
            foreach (Post p in Posts)
            {
                if (p.Id == postId)
                {
                    return p;
                }
            }
            return null;
        }

        public List<Post> GetLastPosts(User other, int count)
        {
            List<Post> result = new List<Post>();
            for (int i = other.Posts.Count - 1; i >= 0 && result.Count < count; i--)
            {
                result.Add(other.Posts[i]);
            }
            return result;
        }

        public bool LikePost(User author, int postId)
        {
            if (author == this)
            {
                return false;
            }
            Post post = author.FindPost(postId);
            if (post == null)
            {
                return false;
            }
            post.AddLike();
            return true;
        }

        public bool CommentPost(User author, int postId, string text)
        {
            if (author == this)
            {
                return false;
            }
            Post post = author.FindPost(postId);
            if (post == null)
            {
                return false;
            }
            post.AddComment(new Comment(Nickname, text));
            return true;
        }
    }

    class Program
    {

        static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int value))
                {
                    return value;
                }
                Console.WriteLine("Помилка: введіть ціле число.");
            }
        }

        static string ReadText(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string text = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    return text.Trim();
                }
                Console.WriteLine("Текст не може бути порожнім.");
            }
        }


        static List<User> CreateUsers()
        {
            User daniil = new User("Даніїл Чернушич", "daniil.chernushych@gmail.com", "@daniil_ch");
            User maria = new User("Марія Шевченко", "maria.shevchenko@gmail.com", "@maria_s");
            User volodymyr = new User("Володимир Великий", "volodymyr.velykyi@gmail.com", "@volodymyr_v");

            daniil.CreatePost("Сьогодні почав вивчати C#!", DateTime.Now.AddDays(-7));
            daniil.CreatePost("Класи і структури - це цікаво.", DateTime.Now.AddDays(-6));
            daniil.CreatePost("Робота над лабораторною в розпалі.", DateTime.Now.AddDays(-5));
            daniil.CreatePost("Кава - головний інструмент програміста.", DateTime.Now.AddDays(-4));
            daniil.CreatePost("Нарешті вихідні!", DateTime.Now.AddDays(-3));
            daniil.CreatePost("Ходили сьогодні на прогулянку.", DateTime.Now.AddDays(-2));
            daniil.CreatePost("Завтра пара з програмування.", DateTime.Now.AddDays(-1));

            maria.CreatePost("Всім привіт! Це мій перший пост.", DateTime.Now.AddDays(-3));
            maria.CreatePost("Переглянула гарний фільм.", DateTime.Now.AddDays(-1));

            volodymyr.CreatePost("Хто йде на футбол сьогодні?", DateTime.Now.AddHours(-5));

            daniil.Posts[0].AddLike();
            daniil.Posts[0].AddComment(new Comment(maria.Nickname, "Успіхів у навчанні!"));

            return new List<User> { daniil, maria, volodymyr };
        }


        static User SelectUser(List<User> users, User exclude, string title)
        {
            Console.WriteLine();
            Console.WriteLine(title);
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i] != exclude)
                {
                    Console.WriteLine($"  {i + 1}. {users[i].Name} ({users[i].Nickname}, {users[i].Email})");
                }
            }
            Console.WriteLine("  0. Назад / вихід");

            while (true)
            {
                int choice = ReadInt("Ваш вибір: ");
                if (choice == 0)
                {
                    return null;
                }
                if (choice >= 1 && choice <= users.Count && users[choice - 1] != exclude)
                {
                    return users[choice - 1];
                }
                Console.WriteLine("Невірний вибір, спробуйте ще раз.");
            }
        }


        static void PrintPosts(User owner, List<Post> posts)
        {
            Console.WriteLine();
            if (posts.Count == 0)
            {
                Console.WriteLine($"У користувача {owner.Nickname} ще немає постів.");
                return;
            }

            Console.WriteLine($"Пости користувача {owner.Name} ({owner.Nickname}), кількість: {posts.Count}");
            Console.WriteLine("------------------------------------------");
            foreach (Post p in posts)
            {
                p.Print();
                Console.WriteLine();
            }
        }

        static void ShowLastPostsOfOther(User current, List<User> users)
        {
            User other = SelectUser(users, current, "Чиї пости переглянути?");
            if (other == null)
            {
                return;
            }
            PrintPosts(other, current.GetLastPosts(other, 5));
        }

        static void LikeOrComment(User current, List<User> users, bool isLike)
        {
            User other = SelectUser(users, current, "До поста якого користувача?");
            if (other == null)
            {
                return;
            }

            List<Post> lastPosts = current.GetLastPosts(other, 5);
            PrintPosts(other, lastPosts);
            if (lastPosts.Count == 0)
            {
                return;
            }

            int postId = ReadInt("Введіть номер поста: ");
            if (other.FindPost(postId) == null)
            {
                Console.WriteLine("Пост з таким номером не знайдено.");
                return;
            }

            if (isLike)
            {
                current.LikePost(other, postId);
                Console.WriteLine("Лайк поставлено!");
            }
            else
            {
                string text = ReadText("Ваш коментар: ");
                current.CommentPost(other, postId, text);
                Console.WriteLine("Коментар додано!");
            }
        }

        static void CreateNewPost(User current)
        {
            string text = ReadText("Текст нового поста: ");
            Post post = current.CreatePost(text);
            Console.WriteLine($"Пост №{post.Id} створено!");
        }

        static void DeleteOwnPost(User current)
        {
            if (current.Posts.Count == 0)
            {
                Console.WriteLine("У вас немає постів для видалення.");
                return;
            }

            PrintPosts(current, current.Posts);
            int postId = ReadInt("Введіть номер поста для видалення: ");
            if (current.DeletePost(postId))
            {
                Console.WriteLine("Пост видалено!");
            }
            else
            {
                Console.WriteLine("Пост з таким номером не знайдено серед ваших постів.");
            }
        }

        static void RunUserSession(User current, List<User> users)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine($"=== Ви увійшли як {current.Name} ({current.Nickname}) ===");
                Console.WriteLine("1. Переглянути останні 5 постів іншого користувача");
                Console.WriteLine("2. Поставити лайк посту іншого користувача");
                Console.WriteLine("3. Написати коментар до поста іншого користувача");
                Console.WriteLine("4. Створити новий пост");
                Console.WriteLine("5. Видалити свій пост");
                Console.WriteLine("6. Показати всі мої пости");
                Console.WriteLine("0. Вийти з акаунта");

                int choice = ReadInt("Ваш вибір: ");
                switch (choice)
                {
                    case 1: ShowLastPostsOfOther(current, users); break;
                    case 2: LikeOrComment(current, users, true); break;
                    case 3: LikeOrComment(current, users, false); break;
                    case 4: CreateNewPost(current); break;
                    case 5: DeleteOwnPost(current); break;
                    case 6: PrintPosts(current, current.Posts); break;
                    case 0: return;
                    default: Console.WriteLine("Невірний пункт меню."); break;
                }
            }
        }

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            List<User> users = CreateUsers();

            while (true)
            {
                User current = SelectUser(users, null, "Увійти як користувач:");
                if (current == null)
                {
                    break;
                }
                RunUserSession(current, users);
            }

            Console.WriteLine("Роботу завершено.");
        }
    }
}
