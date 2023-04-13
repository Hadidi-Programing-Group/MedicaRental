using MedicaRental.BLL.Managers;
using MedicaRental.DAL.Context;
using MedicaRental.DAL.Repositories.Accounts;
using MedicaRental.DAL.Repositories.Admins;
using MedicaRental.DAL.Repositories.Categories;
using MedicaRental.DAL.Repositories.Clients;
using MedicaRental.DAL.Repositories.Items;
using MedicaRental.DAL.Repositories.Messages;
using MedicaRental.DAL.Repositories.Reports;
using MedicaRental.DAL.Repositories.Reviews;
using MedicaRental.DAL.Repositories.SubCategories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


#region Main Services
builder.Services.AddDbContext<MedicaRentalDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MedicaRentalDbConn")));
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

#region Repository Services
builder.Services.AddScoped<IItemsRepo, ItemsRepo>();
builder.Services.AddScoped<ICategoriesRepo, CategoriesRepo>();
builder.Services.AddScoped<ISubCategoriesRepo, SubCategoriesRepo>();
builder.Services.AddScoped<IClientsRepo, ClientsRepo>();
builder.Services.AddScoped<IAdminsRepo, AdminsRepo>();
builder.Services.AddScoped<IMessagesRepo, MessagesRepo>();
builder.Services.AddScoped<IReportsRepo, ReportsRepo>();
builder.Services.AddScoped<IReviewsRepo, ReviewsRepo>();
builder.Services.AddScoped<IAccountsRepo, AccountsRepo>();
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
builder.Services.AddScoped<ISubCategoriesManager, SubCategoriesManager>();
#endregion



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


# region Middelwares
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
#endregion

app.Run();