namespace Bookify.DTO
{
    public class BookDTO : BaseDTO
    {
        public int BookId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int BookCount { get; set; }
        public bool IsDeleted { get; set; }
    }
}
