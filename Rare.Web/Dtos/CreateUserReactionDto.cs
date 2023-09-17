namespace Rare.Web.Dtos
{
    public class CreateUserReactionDto
    {
        public int UserId { get; set; }
        public int ReactionId { get; set; }
        public int PostId { get; set; }
    }
}
