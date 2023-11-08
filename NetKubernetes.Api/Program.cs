using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Netkubernetes.Repository.Interfaces;
using NetKubernetes.Middleware;
using NetKubernetes.Models;
using NetKubernetes.Persistence;
using NetKubernetes.Profiles;
using NetKubernetes.Repository.Classes;
using NetKubernetes.Repository.Interfaces;
using NetKubernetes.Token.Classes;
using NetKubernetes.Token.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt
        .LogTo(Console.WriteLine,
            new[] { DbLoggerCategory.Database.Command.Name },
            LogLevel.Information)
        .EnableSensitiveDataLogging();
    opt
        .UseSqlServer(builder.Configuration
        .GetConnectionString("SqlServerConnection")!);
});

builder.Services.AddScoped<IPropertiesRepository, PropertiesRepository>();
//! revisar si no va despues
builder.Services.AddScoped<IUsersRepository, UsersRepository>();

// Add services to the container.

builder.Services.AddControllers(opt =>
{
    var policy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

    opt.Filters.Add(new AuthorizeFilter(policy));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "NetKubernetes API", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Token Header <br/>
                        First Enter 'bearer' <br/>
                        Second [token] <br/>
                        Example <b>bearer 123456789</b>",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

var mapperConfig = new MapperConfiguration(opt =>
{
    opt.AddProfile(new PropertyProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

var builderSecurity = builder.Services.AddIdentityCore<User>();
var identityBuilder = new IdentityBuilder(builderSecurity.UserType, builder.Services);
identityBuilder.AddEntityFrameworkStores<AppDbContext>();
identityBuilder.AddSignInManager<SignInManager<User>>();

builder.Services.AddSingleton<ISystemClock, SystemClock>();

builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();

builder.Services.AddScoped<IUserSession, UserSession>();

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("My_s3cr3t_p455w03d_f0r_g3n3r4t3_jwt_1234$_p455w03d_3ncrypt3d_$_netcoreapi"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateAudience = false,
            ValidateIssuer = false
        };
    });

builder.Services.AddCors(opt => opt.AddPolicy("appcors", builder =>
{
    builder.WithOrigins("*")
    .AllowAnyMethod()
    .AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ManagerMiddleware>();

app.UseAuthentication();

app.UseCors("appcors");
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var environment = app.Services.CreateScope())
{
    var services = environment.ServiceProvider;

    try
    {
        var userManager = services.GetRequiredService<UserManager<User>>();
        var context = services.GetRequiredService<AppDbContext>();

        await context.Database.MigrateAsync();
        await LoadDatabase.InserData(context, userManager);
    }
    catch (Exception e)
    {
        var logging = services.GetRequiredService<ILogger<Program>>();
        logging.LogError(e, "migration failure");
    }
}

app.Run();
