using API;
using API.Middleware;
using Infastructure;
using Service;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddNpgsqlDataSource(Utilities.ProperlyFormattedConnectionString,
    dataSourceBuilder => dataSourceBuilder.EnableParameterLogging()); 
builder.Services.AddSingleton<AccountRepository>();
builder.Services.AddSingleton<ChatReposetory>();
builder.Services.AddSingleton<PasswordHashRepository>();
builder.Services.AddSingleton<PostRepository>();
builder.Services.AddSingleton<CommentRepository>();
builder.Services.AddSingleton<AccountService>();
builder.Services.AddSingleton<ChatService>();
builder.Services.AddSingleton<PostService>();
builder.Services.AddSingleton<CommentService>();
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<HttpClientService>();
builder.Services.AddSingleton<MailService>();
builder.Services.AddAvatarBlobService();
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

var frontendOrigin = app.Services.GetService<IConfiguration>()!["FrontendOrigin"];

app.UseCors(policy =>

    policy.SetIsOriginAllowed(origin => origin == frontendOrigin)
        .AllowAnyMethod()
        .AllowAnyHeader()
);

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
app.UseMiddleware<GlobalExeptionHandeler>();
app.Run();