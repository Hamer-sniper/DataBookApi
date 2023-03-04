using DataBookApi.Authentification;
using DataBookApi.Controllers;
using DataBookApi.DataContext;
using DataBookApi.Interfaces;
using DataBookApi.Roles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DataBookApi
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddMvc(mvcOtions => mvcOtions.EnableEndpointRouting = false);

            builder.Services.AddDbContext<DataBookContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddTransient<IDataBookData, DataBookData>();
            builder.Services.AddTransient<IAccount, Account>();

            builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<DataBookContext>()
    .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
            })
               .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtOptions =>
               {
                   jwtOptions.TokenValidationParameters = new TokenValidationParameters()
                   {
                       // ���������, ����� �� �������������� �������� ��� ��������� ������
                       ValidateIssuer = true,
                       // ������, �������������� ��������
                       ValidIssuer = AuthOptions.ISSUER,
                       // ����� �� �������������� ����������� ������
                       ValidateAudience = true,
                       // ��������� ����������� ������
                       ValidAudience = AuthOptions.AUDIENCE,
                       // ����� �� �������������� ����� �������������
                       ValidateLifetime = true,
                       // ��������� ����� ������������
                       IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                       // ��������� ����� ������������
                       ValidateIssuerSigningKey = true,
                   };
               });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<User>>();
                var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                await RoleInitializer.InitializeAsync(userManager, rolesManager);
            }

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMvc();

            app.MapGet("/", () => "Api �������!");

            app.Run();
        }
    }
}