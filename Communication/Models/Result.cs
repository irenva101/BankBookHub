namespace Common.Models
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public double AccountBalance { get; set; }
        public Dictionary<int, Book> Books { get; set; }
    }
}
