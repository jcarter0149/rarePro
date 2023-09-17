using Rare.Web.Data;

namespace Rare.Web.Dtos
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public UserDataEntity Author { get; set; }
        public DateTime Created { get; set; }
        public PostUserReactionDataEntity UserReaction { get; set; }
        public List<PostUserReactionDataEntity> Reactions { get; set; }

    }
}
