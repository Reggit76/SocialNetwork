using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Services;
using SocialNetwork.Services.Interfaces;
using SocialNetwork.Models.Entity;
using SocialNetwork.Models.ViewModels;
using SocialNetwork.Models;
using Microsoft.Extensions.Logging;
using SocialNetwork.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigureServices(builder.Services, builder.Configuration, builder.Logging);

var app = builder.Build();

// Configure the HTTP request pipeline.
Configure(app, app.Environment);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
{
    services.AddControllersWithViews();

    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
    );

    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IFriendshipService, FriendshipService>();
    services.AddScoped<IPostService, PostService>();
    services.AddScoped<IChatService, ChatService>();
    services.AddScoped<IMessageService, MessageService>();
    services.AddScoped<ICommentService, CommentService>();

    services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.ExpireTimeSpan = TimeSpan.FromDays(7);
            options.SlidingExpiration = true;
        });

    services.AddHttpContextAccessor();
    services.AddSignalR();  // Добавление SignalR

    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
}

void Configure(WebApplication app, IWebHostEnvironment env)
{
    if (!env.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseBanCheck(); 
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.MapHub<ChatHub>("/chathub");  // Маршрут для SignalR

    EnsureDatabaseCreated(app.Services).Wait();
}

async Task EnsureDatabaseCreated(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var scopedServices = scope.ServiceProvider;
    var context = scopedServices.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();

    await CreateAdminUserAsync(scopedServices);
}

async Task CreateAdminUserAsync(IServiceProvider serviceProvider)
{
    var userService = serviceProvider.GetRequiredService<IUserService>();

    string adminFullName = "Admin";
    string adminUsername = "KingFrog";
    string adminEmail = "KingFrog@admin.com";
    string adminPassword = "Admin123!";

    var adminUser = await userService.AuthenticateUserAsync(adminEmail, adminPassword);
    if (adminUser == null)
    {
        var model = new RegisterViewModel
        {
            FullName = adminFullName,
            Username = adminUsername,
            Email = adminEmail,
            Password = adminPassword,
            Role = Role.Administrator
        };

        var result = await userService.RegisterUserAsync(model);
        if (result)
        {
            adminUser = await userService.AuthenticateUserAsync(adminEmail, adminPassword);
            if (adminUser != null)
            {
                await userService.ChangeUserRoleAsync(adminUser.Id, Role.Administrator.ToString());
            }
        }
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class BanCheckMiddlewareExtensions
{
    public static IApplicationBuilder UseBanCheck(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<BanCheckMiddleware>();
    }
}

public class BanCheckMiddleware
{
    private readonly RequestDelegate _next;

    public BanCheckMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity.IsAuthenticated)
        {
            var userService = context.RequestServices.GetRequiredService<IUserService>();
            var userId = await userService.GetUserIdAsync(context.User.Identity.Name);
            var user = await userService.GetUserProfileAsync(userId);
            if (user != null && user.IsBanned)
            {
                context.Response.Redirect("/Account/Banned");
                return;
            }
        }

        await _next(context);
    }
}
