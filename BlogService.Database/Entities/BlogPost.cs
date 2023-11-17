namespace BlogService.Database.Entities
{
    public class BlogPost
    {

        private Guid _id = Guid.NewGuid();
        public string Id
        {
            get => _id.ToString();
            set => _id = Guid.Parse(value);
        }
        public required string Author { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }


}
