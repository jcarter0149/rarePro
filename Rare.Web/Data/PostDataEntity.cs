using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Rare.Web.Data
{
    public class PostDataEntity
    {
        public int Id { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public UserDataEntity User { get; set; }
        public string Title { get; set; }
        public DateTime PublicationDate { get; set; }
        public string ImageUrl { get; set; }
        public string Content { get; set; }
        public bool IsApproved { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public ICollection<TagDataEntity> Tags { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public CategoryDataEntity Category { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public ICollection<CommentDataEntity> Comments { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public ICollection<ReactionDataEntity> Reactions { get; set; }
    }
}
