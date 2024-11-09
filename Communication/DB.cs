using Common.Models;

namespace Common
{
    public class DB
    {
        private static readonly object balanceLock = new object();
        private static double accountBalance = 100.0;
        public static double AccountBalance
        {
            get
            {
                lock (balanceLock)
                { return accountBalance; }
            }
            set
            {
                lock (balanceLock)
                { accountBalance = value; }
            }
        }
        public static Dictionary<int, Book> Books { get; } = new Dictionary<int, Book>
        {
            [1] = new Book { Id = 1, Title = "The Name of the Wind", Author = "Patrick Rothfuss", Description = "The Name of the Wind follows the journey of Kvothe, a gifted young man who grows up to become a legendary figure.", Price = 19.99, Quantity = 3 },
            [2] = new Book { Id = 2, Title = "Rebecca", Author = "Daphne du Maurier", Description = "Rebecca tells the story of a young, unnamed protagonist who marries the wealthy widower Maxim de Winter and moves to his grand estate, Manderley.", Price = 14.99, Quantity = 1 },
            [3] = new Book
            {
                Id = 3,
                Title = "1984",
                Author = "George Orwell",
                Description = "1984 is a dystopian novel set in a totalitarian regime governed by the Party, led by the figurehead Big Brother.",
                Price = 17.99,
                Quantity = 5
            }
        };

        public DB()
        {

        }

        public static bool RemoveFunds(double amount)
        {
            lock (DB.balanceLock)
            {
                if (DB.AccountBalance >= amount)
                {
                    DB.AccountBalance -= amount;
                    return true;
                }
                return false;
            }
        }

        public static IEnumerable<Book> GetAllBooks()
        {
            return Books.Values;
        }
        public static Book GetBookById(int id)
        {
            if (Books.TryGetValue(id, out var book))
            {
                return book;
            }
            return null;
        }
    }
}



