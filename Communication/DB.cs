using Common.Models;

namespace Common
{
    public class DB
    {
        public double AccountBalance { get; set; }

        public Dictionary<int, Book> Books { get; set; }

        private static DB _db;

        private DB()
        {
            AccountBalance = 100.00;
            Books = new Dictionary<int, Book>
            {
                [1] = new Book { Id = 1, Title = "The Name of the Wind", Author = "Patrick Rothfuss", Description = "The Name of the Wind follows the journey of Kvothe, a gifted young man who grows up to become a legendary figure.", Price = 19.99, Quantity = 3 },
                [2] = new Book { Id = 2, Title = "Rebecca", Author = "Daphne du Maurier", Description = "Rebecca tells the story of a young, unnamed protagonist who marries the wealthy widower Maxim de Winter and moves to his grand estate, Manderley.", Price = 14.99, Quantity = 6 },
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
        }

        public static DB GetDB()
        {
            if (_db == null)
            {
                _db = new DB();
            }
            return _db;
        }

        public void AddFunds(double amount)
        {
            AccountBalance += amount;
        }

        public bool RemoveFunds(double amount)
        {
            if (AccountBalance >= amount)
            {
                AccountBalance -= amount;
                Console.WriteLine($"Funds removed. New balance: {AccountBalance}");
                return true;
            }
            return false;
        }



        public bool ModifyQuantity(int id, int newAmount)
        {
            if (Books.ContainsKey(id))
            {
                Books[id].Quantity = newAmount;
                return true;
            }
            return false;
        }

        public Book GetBookById(int id)
        {
            if (Books.ContainsKey(id))
            {
                return Books[id];
            }
            return null;
        }
    }
}



