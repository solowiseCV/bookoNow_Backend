using BookNow.Application.Extensions;
using BookNow.Infrastructure.Extensions;
using BookNow.Infrastructure.Identity;
using BookNow.Presentation.Extensions;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplication().AddCorsConfiguration().AddDatabase(builder.Configuration).AddIdentityConfiguration(builder.Configuration).AddJwtAuthentication(builder.Configuration).AddRateLimiting().AddPersistence();
builder.Services.AddPaymentServices(builder.Configuration);
builder.Services.AddControllers()
    .AddJsonOptions(options => 
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddSwaggerDocumentation();
builder.Services.Configure<GoogleAuthOptions>(
    builder.Configuration.GetSection("GoogleAuthSettings"));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookNow API v1");
    c.RoutePrefix = "swagger";
});
app.UseGlobalMiddlewares();
app.MapGet("/ping", () => "api is working");
app.MapControllers();
app.Run();
