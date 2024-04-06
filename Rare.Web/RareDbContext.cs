using Microsoft.EntityFrameworkCore;
using Rare.Web.Data;

namespace Rare.Web
{
    public class RareDbContext : DbContext
    {
        public RareDbContext(DbContextOptions options) : base(options) { }
        public DbSet<UserDataEntity> Users { get; set; }
        public DbSet<PostDataEntity> Posts { get; set; }
        public DbSet<CommentDataEntity> Comments { get; set; }
        public DbSet<CategoryDataEntity> Categories { get; set; }
        public DbSet<ReactionDataEntity> Reactions { get; set; }
        public DbSet<SubscriptionDataEntity> Subscriptions { get; set; }
        public DbSet<TagDataEntity> Tags { get; set; }
        public DbSet<PostUserReactionDataEntity> PostReactions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
        }


    }
}
