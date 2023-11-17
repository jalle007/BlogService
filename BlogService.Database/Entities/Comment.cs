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
        public string BlogPostId { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
        //public virtual BlogPost BlogPost { get; set; }
    }



}
