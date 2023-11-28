using API;
using API.Middleware;
using Infastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddNpgsqlDataSource(Utilities.ProperlyFormattedConnectionString,
    dataSourceBuilder => dataSourceBuilder.EnableParameterLogging()); 
builder.Services.AddSingleton<AccountRepository>();
builder.Services.AddSingleton<ChatReposetory>();
builder.Services.AddSingleton<PasswordHashRepository>();
builder.Services.AddSingleton<PostRepository>();
builder.Services.AddSingleton<CommentRepository>();
builder.Services.AddSingleton<Service.AccountService>();
builder.Services.AddSingleton<Service.ChatService>();
builder.Services.AddSingleton<Service.PostService>();
builder.Services.AddSingleton<Service.CommentService>();
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<HttpClientService>();
builder.Services.AddControllers();
builder.Services.AddJwtService();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithBearerJWT();

var frontEndRelativePath = "../frontend/www";
builder.Services.AddSpaStaticFiles(conf => conf.RootPath = frontEndRelativePath);

var app = builder.Build();
app.UseRouting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options =>
{
    options.SetIsOriginAllowed(origin => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
});

app.UseSpaStaticFiles();
app.UseSpa(conf =>
{
    conf.Options.SourcePath = frontEndRelativePath;
});

app.UseSecurityHeaders();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<JwtBearerHandler>();
app.Run();