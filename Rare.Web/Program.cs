using Microsoft.EntityFrameworkCore;
using Rare.Web;
using Rare.Web.Data;
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


app.Run();