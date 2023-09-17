using Microsoft.EntityFrameworkCore;
using Rare.Web;
using Rare.Web.Data;
using Rare.Web.Dtos;
using System.Runtime.InteropServices;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000");
                      });
});

builder.Services.AddDbContext<RareDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("rare"));

});



var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();





app.MapGet("/user/{id}", async (RareDbContext _context, int id) =>
{
    UserDataEntity user = await _context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();

    if (user == null)
    {
        return Results.Problem($"User with Id {id} not found");
    }

    return Results.Ok(user);
});

app.MapGet("/user/{uid}", async (RareDbContext _context, string uid) =>
{
    UserDataEntity user = await _context.Users.Where(x => x.Uid.ToLower() == uid.ToLower()).FirstOrDefaultAsync();

    if (user == null)
    {
        return Results.Problem($"User with UID {uid} not found");
    }

    return Results.Ok(user);
});

app.MapPost("/user", async (RareDbContext _context, UserDataEntity newUser) =>
{
    _context.Add(newUser);
    await _context.SaveChangesAsync();

    return Results.Ok(newUser);
});

app.MapGet("/users", async (RareDbContext _context) =>
{
    return await _context.Users.ToListAsync();
});

app.MapDelete("/user/{id}", async (RareDbContext _context, int id) =>
{
    UserDataEntity userToDelete = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
    _context.Users.Remove(userToDelete);


    await _context.SaveChangesAsync();

    return Results.Ok();
});

app.MapPost("/post", async (RareDbContext _context, CreatePostDto Post) =>
{
    PostDataEntity newPost = new()
    {
        Title = Post.Title,
        PublicationDate = Post.PublicationDate,
        ImageUrl = Post.ImageUrl,
        Content = Post.Content
    };

    newPost.User = await _context.Users.FirstOrDefaultAsync(x => x.Id == Post.UserId);

    newPost.Category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == Post.CategoryId);

    foreach (var id in Post.Tags)
    {
        TagDataEntity newTag = await _context.Tags.FirstOrDefaultAsync(x => x.Id == id);

        newPost.Tags.Add(newTag);
    }

    await _context.SaveChangesAsync();

    return Results.Ok();
});

app.MapGet("/post/{postId}", async (RareDbContext _context, int postId, int? userId) =>
{
    PostDataEntity postDataEntity = await  _context.Posts.FirstOrDefaultAsync(x => x.Id == postId);

    List<PostUserReactionDataEntity> reactions = await _context.PostReactions.Include(x => x.Reaction).Include(x => x.User).Where(x => x.Post.Id == postId).ToListAsync();

    PostUserReactionDataEntity userReactionDataEntity = reactions.Where(x => x.User.Id == userId).FirstOrDefault()
;
    PostDto post = new()
    {
        Id = postDataEntity.Id,
        Content = postDataEntity.Content,
        Reactions = reactions,
        UserReaction = userReactionDataEntity
    };

    return Results.Ok(post);

});

app.MapPost("/post/reaction", async (RareDbContext _context, CreateUserReactionDto userReaction) =>
{
    PostUserReactionDataEntity previousUserReaction = await _context.PostReactions.FirstOrDefaultAsync(x => x.Post.Id == userReaction.PostId && x.User.Id == userReaction.UserId);

    if(previousUserReaction != null)
    {
        _context.PostReactions.Remove(previousUserReaction);
    }
    PostUserReactionDataEntity newReaction = new();

    newReaction.User = await _context.Users.FirstOrDefaultAsync(x => x.Id == userReaction.UserId);
    newReaction.Post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == userReaction.PostId);
    newReaction.Reaction = await _context.Reactions.FirstOrDefaultAsync(x => x.Id == userReaction.ReactionId);

    _context.Add(newReaction);
    await _context.SaveChangesAsync();  

    return Results.Ok(newReaction);
});

app.MapDelete("post/reaction", async (RareDbContext _context, int id) =>
{
    PostUserReactionDataEntity reactionToDelete = await _context.PostReactions.FirstOrDefaultAsync(x => x.Id == id);

    _context.PostReactions.Remove(reactionToDelete);

    await _context.SaveChangesAsync();

    return Results.Ok();
});

app.Run();