using System.ComponentModel.DataAnnotations;

namespace Rare.Web.Data
{
    public class CommentDataEntity
    {
        public int Id { get; set; }
        public UserDataEntity? User { get; set; }
        public PostDataEntity? Post { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
