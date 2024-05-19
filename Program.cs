using Microsoft.AspNetCore.Authorization.Infrastructure;
using TodoApi;
using System.Web.Http.Cors;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));
builder.Services.AddScoped<ToDoDbContext>();
builder.Services.AddDbContext<ToDoDbContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseCors("corsapp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});

app.MapGet("/todoitems", async (ToDoDbContext db) =>
    await db.Items.ToListAsync());

app.MapGet("/todoitems/complete", async (ToDoDbContext db) =>
    await db.Items.Where(t => t.IsComplete == true).ToListAsync());

app.MapGet("/todoitems/{id}", async (int id, ToDoDbContext db) =>
    await db.Items.FindAsync(id)
        is Item todo
            ? Results.Ok(todo)
            : Results.NotFound());


app.MapPost("/todoitems/{todo}", async (string todo, ToDoDbContext db) =>
{
    db.Items.Add(new Item(todo));
    await db.SaveChangesAsync();
});

app.MapPut("/todoitems/{id}/{isComplete}", async (int id, bool inputTodo, ToDoDbContext db) =>
{
    var todo = await db.Items.FindAsync(id);

    if (todo is null) return Results.NotFound();

    todo.IsComplete = inputTodo;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, ToDoDbContext db) =>
{
    if (await db.Items.FindAsync(id) is Item todo)
    {
        db.Items.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.Run();














/*
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton< ToDoDbContext>();
var app = builder.Build();

// Add a route handler that uses a service from DI 
app.MapGet("/", (ToDoDbContext service) => service.Items);
app.MapPost("/{Id}/{Name}/{IsComplete}", (ToDoDbContext service) => service.Items.Add());
app.MapPut("/", () => "This is a PUT");
app.MapDelete("/", () => "This is a DELETE");

app.MapGet("/", () => "Hello World!");


app.Run();*/

/*
await using var provider = new ServiceCollection()
            .AddScoped<ToDoDbContext>()
            .BuildServiceProvider();
using (var scope = provider.CreateScope())
        {
            var foo = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
        }
*/

