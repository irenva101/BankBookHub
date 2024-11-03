using Common.Models;

namespace Common
{
    public class DB
    {
        private readonly Dictionary<int, Book> _books;
        private double accountBalance;
        public double AccountBalance { get => accountBalance; set => accountBalance = Math.Round(value, 2); }

        public DB()
        {
            _books = new Dictionary<int, Book>();
            AccountBalance = 100;

            _books[1] = new Book
            {
                Id = 1,
                Title = "The Name of the Wind",
                Description = "The Name of the Wind follows the journey of Kvothe, a gifted young man who grows up to become a legendary figure.",
                Price = 19.99,
                Quantity = 3,
                Author = "Patric Rothfuss"
            };
            _books[2] = new Book
            {
                Id = 2,
                Title = "Rebecca",
                Author = "Daphne du Maurier",
                Price = 14.99,
                Quantity = 3,
                Description = "Rebecca tells the story of a young, unnamed protagonist who marries the wealthy widower Maxim de Winter and moves to his grand estate, Manderley.",
            };
            _books[3] = new Book
            {
                Id = 3,
                Title = "1984",
                Author = "George Orwell",
                Price = 17.99,
                Quantity = 5,
                Description = "1984 is a dystopian novel set in a totalitarian regime governed by the Party, led by the figurehead Big Brother.",
            };
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return _books.Values;
        }
        public Book GetBookById(int id)
        {
            if (_books.TryGetValue(id, out var book))
            {
                return book;
            }
            return null;
        }
    }
}



