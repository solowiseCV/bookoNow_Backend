using BookNow.Api.Extensions;
using BookNow.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCorsConfiguration().AddDatabase(builder.Configuration).AddIdentityConfiguration().AddJwtAuthentication(builder.Configuration).AddRateLimiting().AddPersistence(); ;
builder.Services.AddControllers();
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

app.UseGlobalMiddlewares();

app.MapControllers();

app.Run();
