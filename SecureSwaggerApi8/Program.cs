// Program.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Secure API",
        Version = "v1",
        Description = "A secure API with Swagger authentication",
        Contact = new OpenApiContact
        {
            Name = "API Support",
            Email = "support@example.com"
        }
    });

    options.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Basic Authorization header."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add authentication
builder.Services.AddAuthentication(options => 
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "BasicAuthentication";
})
    .AddCookie("Cookies")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>
    ("BasicAuthentication", null);

builder.Services.AddAuthorization();

// Configure rate limiting


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Add the authentication check middleware BEFORE Swagger
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/swagger") && 
        !context.User.Identity?.IsAuthenticated == true)
    {
        context.Response.Redirect("/auth/login");
        return;
    }
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((swagger, httpReq) =>
        {
            swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
        });
    });
    
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Secure API v1");
        c.RoutePrefix = "swagger";
        c.ConfigObject.AdditionalItems["persistAuthorization"] = true;
    });
}

app.MapControllers();

app.Run();
