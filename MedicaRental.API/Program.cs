using MedicaRental.API.DataSeeding;
using MedicaRental.BLL.Managers;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };
    });
#endregion

#region Authorization Services
builder.Services
    .AddAuthorization(options =>
    {
        //options.AddPolicy("", policy => policy
        //    .RequireClaim(ClaimTypes.Role, "", "", ....));
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
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
#endregion

app.MapGet("/", () => "Hello World");
app.MapGet("/Hi", () => "GitHub Acctions Works !!");
app.Run();

string GetRDSConnectionString()
{
    var appConfig = System.Configuration.ConfigurationManager.AppSettings;

    string dbname = Environment.GetEnvironmentVariable("RDS_DB_NAME")??string.Empty;

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
