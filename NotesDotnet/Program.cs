using NotesDotnet.Models;
using NotesDotnet.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<NotesDatabaseSettings>(
    builder.Configuration.GetSection("NotesDatabase")
);

builder.Services.AddSingleton<IFolderService, FolderService>();
builder.Services.AddSingleton<INoteService, NoteService>();

var app = builder.Build();

app.MapOpenApi();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors(item => item.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
}

app.UseHttpsRedirection();
app.UseSwaggerUI(item => item.SwaggerEndpoint("/openapi/v1.json", "Swagger DEMO"));

app.UseAuthorization();

app.MapControllers();

app.Run();
