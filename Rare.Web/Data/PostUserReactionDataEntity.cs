namespace Rare.Web.Data
{
    public class PostUserReactionDataEntity
    {
        public int Id { get; set; }
        public PostDataEntity Post { get; set; }
        public ReactionDataEntity Reaction { get; set; }
        public UserDataEntity User { get; set; }
    }
}
