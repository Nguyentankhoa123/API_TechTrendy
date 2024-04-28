using API.Data;
using API.Helpers;
using API.Hubs;
using API.Model.Entity;
using API.Repositories;
using API.Repository;
using API.Repository.Implement;
using API.Services.Common;
using API.Services.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter your token with this format: ''Bearer YOUR_TOKEN''",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "Bearer",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        var routeTemplate = apiDesc.RelativePath;
        var hiddenRoutes = new List<string> { "login", "register", "refresh", "confirmEmail", "resendConfirmationEmail", "forgotPassword", "resetPassword", "manage/2fa", "manage/info" };
        return !hiddenRoutes.Contains(routeTemplate);
    });
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCors", build =>
    {
        build.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
        build.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
});


var configuration = builder.Configuration;


// Configure SQL Server

builder.Services.AddDbContext<StoreContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Ecommerce")));


// Configure Redis

builder.Services.AddStackExchangeRedisCache(redisOptions =>
{
    redisOptions.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")));



builder.Services.AddExceptionHandler<AppExceptionHandler>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<StoreContext>()
    //.AddSignInManager()
    .AddRoles<IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Google:ClientSecret"];
});

builder.Services.AddAuthentication().AddFacebook(facebookOptions =>
{
    facebookOptions.ClientId = builder.Configuration["Facebook:AppId"];
    facebookOptions.ClientSecret = builder.Configuration["Facebook:AppSecret"];
});

// Time Token ResetPassword
builder.Services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromHours(1));
// Email Configs
var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfig>();

builder.Services.AddSingleton(emailConfig);
builder.Services.AddScoped<IEmailSenderRepository, EmailSenderRepository>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ILaptopRepository, LaptopRepository>();
builder.Services.AddScoped<ITabletRepository, TabletRepository>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IRedisRepository, RedisRepository>();
builder.Services.AddScoped<ICloudinaryRepository, CloudinaryRepository>();

builder.Services.AddSingleton<SharedDb>();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(_ => { });

app.MapIdentityApi<ApplicationUser>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseCors("MyCors");

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/Chat");

app.Run();
