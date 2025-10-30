using TaskManager.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// CORS: Allow frontend running at http://localhost:3000
const string CorsPolicyName = "FrontendCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddSingleton<ITaskService, TaskService>();

var app = builder.Build();

app.UseCors(CorsPolicyName);
app.MapControllers();

app.Run();


