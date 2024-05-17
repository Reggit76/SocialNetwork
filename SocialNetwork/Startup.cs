using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork;
using SocialNetwork.Services;

namespace SocialNetwork
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<UserDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("Host=localhost;Port=5432;Database=SocialNetwork;Username=postgres;Password=11111111")));

            // Регистрация сервисов
            services.AddScoped<UserService>();
            services.AddScoped<PostService>();
            services.AddScoped<CommentService>();
            services.AddScoped<ChatService>();
            services.AddScoped<MessageService>();
            services.AddScoped<FriendshipService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

}
