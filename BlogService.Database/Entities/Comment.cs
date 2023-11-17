namespace BlogService.Database.Entities
{
    public class Comment
    {
        private Guid _id = Guid.NewGuid();
        public string Id
        {
            get => _id.ToString();
            set => _id = Guid.Parse(value);
        }
        public required string BlogPostId { get; set; }
        public required string Author { get; set; }
        public required string Text { get; set; }
        //public virtual BlogPost BlogPost { get; set; }
    }



}
