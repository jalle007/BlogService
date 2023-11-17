namespace BlogService.Database.Entities.DTO
{
    public class BlogPostDTO
    {
        public required string Author { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
    }

}
