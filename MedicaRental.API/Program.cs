using MedicaRental.API;
using MedicaRental.API.DataSeeding;
using MedicaRental.API.Services;
using MedicaRental.BLL.Dtos.Admin;
using MedicaRental.BLL.Managers;
using MedicaRental.BLL.Managers.Authentication;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Repositories;
using MedicaRental.DAL.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Stripe;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
StripeConfiguration.ApiKey = "sk_test_51Mq0DEDRs2d2XncX3l5gLODG0on2gtdtEiPEXSsyB2m2TUfGwZwlanLbn5ZBZGP3LJbOjDXlsx1f5j0eTcKbKKJI00mPVX4uAc";


#region Main Services
builder.Services.AddDbContext<MedicaRentalDbContext>(options => options.UseSqlServer(GetRDSConnectionString()));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        Name = "Authorization",
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });

});
#endregion

#region Identity Services
builder.Services
    .AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.Password.RequiredUniqueChars = 3;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;

        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<MedicaRentalDbContext>();
#endregion

#region RefreshToken 
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
#endregion

#region Authentication Services
builder.Services
    .AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opt =>
    {
        //used by christine I don't know why
        //opt.SaveToken = true;
        //opt.RequireHttpsMetadata = false;
        opt.TokenValidationParameters = new()
        {
            //ValidIssuer = builder.Configuration["JWT:Issuer"],
            //ValidAudience = builder.Configuration["JWT:Audience"],
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
            ValidateLifetime = true, // Checks expiry date.
            ClockSkew = TimeSpan.Zero // Matches time.
        };
        opt.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // If the request is for our hub...
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                {

                    // Read the token out of the query string
                    Console.WriteLine($"Access Token : {accessToken}");
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
#endregion

#region Authorization Services

var _adminPolicy = new AuthorizationPolicyBuilder()
    .RequireClaim(ClaimTypes.Role, UserRoles.Admin.ToString())
    .Build();

var _moderatorPolicy = new AuthorizationPolicyBuilder()
    .RequireClaim(ClaimTypes.Role, UserRoles.Moderator.ToString())
    .Build();

var _clientPolicy = new AuthorizationPolicyBuilder()
    .RequireClaim(ClaimTypes.Role, UserRoles.Client.ToString())
    .Build();


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(ClaimRequirement.AdminPolicy, _adminPolicy);
    options.AddPolicy(ClaimRequirement.ModeratorPolicy, _moderatorPolicy);
    options.AddPolicy(ClaimRequirement.ClientPolicy, _clientPolicy);
});
#endregion

#region Unit Of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
#endregion

#region Managers Services
builder.Services.AddScoped<IAccountsManager, AccountsManager>();
builder.Services.AddScoped<IAdminsManager, AdminsManager>();
builder.Services.AddScoped<ICategoriesManager, CategoriesManager>();
builder.Services.AddScoped<IClientsManager, ClientsManager>();
builder.Services.AddScoped<IItemsManager, ItemsManager>();
builder.Services.AddScoped<IMessagesManager, MessagesManager>();
builder.Services.AddScoped<IReportsManager, ReportsManager>();
builder.Services.AddScoped<IReviewsManager, ReviewsManager>();
builder.Services.AddScoped<IBrandsManager, BrandsManager>();
builder.Services.AddScoped<ISubCategoriesManager, SubCategoriesManager>();
builder.Services.AddScoped<IRentOperationsManager, RentOperationsManager>();
builder.Services.AddScoped<IReportActionManager, ReportActionManager>();
builder.Services.AddScoped<ICartItemsManager, CartItemsManager>();

builder.Services.AddScoped<IAuthManger, AuthManger>();
builder.Services.AddScoped<ITransactionsManager, TransactionsManager>();
builder.Services.AddScoped<ITransactionItemsManager, TransactionItemsManager>();

#endregion

#region CORS Services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
});
#endregion


builder.Services.AddHostedService<DailyRatingCalculationService>();
builder.Services.AddHostedService<DailyClearTokenService>();

#region SignalR
builder.Services.AddSignalR().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.PropertyNamingPolicy = null;
});
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region SeedingIdentity
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DataSeeder.SeedAsync(services);
}
#endregion


# region Middelwares
app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();

app.UseAuthentication();

app.Use(async (context, next) =>
{
    if (context.User.Identity.IsAuthenticated)
    {
        // Set the user identity on the SignalR hub context
        var claimsIdentity = (ClaimsIdentity)context.User.Identity;
        var claims = claimsIdentity.Claims;

        var user = new ClaimsPrincipal(claimsIdentity);

        context.Items["User"] = user;
    }
    await next();

});

app.UseAuthorization();

app.MapControllers();
#endregion

app.MapHub<ChatHub>("/chatHub");

app.MapGet("/", () => "Hello World");
app.MapGet("/Hi", () => "GitHub Acctions Works !!");
app.Run();

string GetRDSConnectionString()
{
    var appConfig = System.Configuration.ConfigurationManager.AppSettings;

    string dbname = Environment.GetEnvironmentVariable("RDS_DB_NAME") ?? string.Empty;

    if (string.IsNullOrEmpty(dbname)) return builder.Configuration.GetConnectionString("MedicaRentalDbConn");

    string username = Environment.GetEnvironmentVariable("RDS_USERNAME") ?? string.Empty;
    string password = Environment.GetEnvironmentVariable("RDS_PASSWORD") ?? string.Empty;
    string hostname = Environment.GetEnvironmentVariable("RDS_HOSTNAME") ?? string.Empty;
    string port = Environment.GetEnvironmentVariable("RDS_PORT") ?? string.Empty;
    string cs =
        $"Initial Catalog={dbname};" +
        $"Data Source={hostname};" +
        $"User ID={username};" +
        $"Password={password};" +
        "Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;";
    return cs;
}
