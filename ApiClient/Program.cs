namespace ApiClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddHttpContextAccessor();

            // Required for session state
            builder.Services.AddDistributedMemoryCache();

            // Session for simple role handling
            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromHours(1);
            });

            // HttpClient for API
            var apiBaseUrl = builder.Configuration["Api:BaseUrl"] ?? string.Empty;
            builder.Services.AddHttpClient("Api", client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });

            // Register API services
            builder.Services.AddScoped<ApiClient.Services.ICategoryApi, ApiClient.Services.CategoryApi>();
            builder.Services.AddScoped<ApiClient.Services.INewsApi, ApiClient.Services.NewsApi>();
            builder.Services.AddScoped<ApiClient.Services.IAccountApi, ApiClient.Services.AccountApi>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
