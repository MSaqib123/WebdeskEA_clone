using WebdeskEA.Core.Extension;
using WebdeskEA.Core.Middlware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();


// Configure middleware and endpoints
app.ConfigureMiddleware();
app.ConfigureEndpoints();

app.Run();
