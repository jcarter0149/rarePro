using Microsoft.EntityFrameworkCore;
using Rare.Web;
using Rare.Web.Data;
using Rare.Web.Dtos;
using System.Runtime.InteropServices;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddDbContext<RareDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("rare"));

});



var app = builder.Build();

app.UseCors(builder => builder
 .AllowAnyOrigin()
 .AllowAnyMethod()
 .AllowAnyHeader()
);

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

app.MapPost("/user/uid", async (RareDbContext _context, UidRequest request) =>
{
    UserDataEntity user = await _context.Users.Where(x => x.Uid.ToLower() == request.Uid.ToLower()).FirstOrDefaultAsync();

    if (user == null)
    {
        return Results.Unauthorized();
    }

    return Results.Ok(user);
});

app.MapPost("/user", async (RareDbContext _context, UserDataEntity newUser) =>
{
    string firstName = newUser.FirstName.Split(' ')[0];
    string lastName = newUser.FirstName.Split(' ')[1];
    newUser.FirstName = firstName;
    newUser.LastName = lastName;
    newUser.CreatedOn = DateTime.Now;
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
        Title = "test",
        PublicationDate = Post.PublicationDate,
        ImageUrl = "test",
        Content = Post.Content,
    };

    newPost.User = await _context.Users.FirstOrDefaultAsync(x => x.Id == Post.UserId);


    newPost.Category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == Post.CategoryId);

    if (Post.Tags?.Count > 0)
    {
        foreach (var id in Post.Tags)
        {
            TagDataEntity newTag = await _context.Tags.FirstOrDefaultAsync(x => x.Id == id);

            newPost.Tags.Add(newTag);
        }
    }
    _context.Add(newPost);
    await _context.SaveChangesAsync();

    return Results.Ok();
});

app.MapGet("/post/{postId}", async (RareDbContext _context, int postId, int? userId) =>
{
    PostDataEntity postDataEntity = await _context.Posts.FirstOrDefaultAsync(x => x.Id == postId);

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

app.MapGet("/posts", async (RareDbContext _context, int? userId) =>
{
    List<PostDataEntity> posts = await _context.Posts.Include(x => x.User).ToListAsync();

    List<PostDto> postsInDto = new();

    foreach (var post in posts)
    {
        List<PostUserReactionDataEntity> reactions = await _context.PostReactions.Include(x => x.Reaction).Include(x => x.User).Where(x => x.Post.Id == post.Id).ToListAsync();

        PostUserReactionDataEntity userReactionDataEntity = null;
        if (userId != null || userId > 0)
        {
            userReactionDataEntity = reactions.Where(x => x.User.Id == userId).FirstOrDefault();
        }

        PostDto newPost = new()
        {
            Id = post.Id,
            Content = post.Content,
            Reactions = reactions,
            UserReaction = userReactionDataEntity,
            Author = post.User
        };

        postsInDto.Add(newPost);
    };

    return Results.Ok(postsInDto);

});

app.MapPost("/post/reaction", async (RareDbContext _context, CreateUserReactionDto userReaction) =>
{
    PostUserReactionDataEntity previousUserReaction = await _context.PostReactions.FirstOrDefaultAsync(x => x.Post.Id == userReaction.PostId && x.User.Id == userReaction.UserId);

    if (previousUserReaction != null)
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