using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Rare.Web.Data
{
    public class SubscriptionDataEntity
    {
        public int Id { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public UserDataEntity Follower { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public UserDataEntity Author { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime EndedOn { get; set;}
    }
}
