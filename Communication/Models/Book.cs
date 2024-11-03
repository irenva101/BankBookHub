namespace Common.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string? Description { get; set; }

        public Book(int id, string? title, string? author, double price, int quantity, string? description = null)
        {
            Id = id;
            Title = title;
            Author = author;
            Price = price;
            Quantity = quantity;
            Description = description;
        }
        public Book()
        {

        }
    }
}
