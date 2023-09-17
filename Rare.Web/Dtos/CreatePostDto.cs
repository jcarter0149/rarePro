using Rare.Web.Data;

namespace Rare.Web.Dtos
{
    public class CreatePostDto
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public DateTime PublicationDate => DateTime.Now;
        public string ImageUrl { get; set; }
        public string Content { get; set; }
        public int CategoryId { get; set; }
        public ICollection<int> Tags { get; set; }
    }
}
