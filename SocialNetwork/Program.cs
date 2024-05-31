using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Services;
using SocialNetwork.Services.Interfaces;
using SocialNetwork.Hubs;

var builder = WebApplication.CreateBuilder(args);

// ���������� �������� � ��������� ������������.
builder.Services.AddControllersWithViews();

// ����������� ��������� ���� ������
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseLazyLoadingProxies());

// ����������� �������� � �� �����������
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFriendshipService, FriendshipService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMessageService, MessageService>();

// ����������� Identity � Cookie Authentication
builder.Services.AddAuthentication("MyCookieAuthenticationScheme")
    .AddCookie("MyCookieAuthenticationScheme", options =>
    {
        options.Cookie.Name = "MyCookie";
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// ����������� SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// ������������ HTTP-��������
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/chathub");

app.Run();
