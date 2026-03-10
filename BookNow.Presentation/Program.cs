using BookNow.Api.Extensions;
using BookNow.Application.Extensions;
using BookNow.Infrastructure.Extensions;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplication().AddCorsConfiguration().AddDatabase(builder.Configuration).AddIdentityConfiguration(builder.Configuration).AddJwtAuthentication(builder.Configuration).AddRateLimiting().AddPersistence(); ;
builder.Services.AddControllers()
    .AddJsonOptions(options => 
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddSwaggerDocumentation();


var app = builder.Build();
app.UseGlobalMiddlewares();

app.MapControllers();
app.Run();
