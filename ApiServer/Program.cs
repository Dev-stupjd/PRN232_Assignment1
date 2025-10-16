using Repositories.Interface;
using Repositories.Repository;
using Services.Interface;
using Services.Service;
using BussinessObjects;
using DataAccessObjects.DAO;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<FunewsManagementContext>();

// DAOs
builder.Services.AddScoped<AccountDAO>();
builder.Services.AddScoped<CategoryDAO>();
builder.Services.AddScoped<NewsArticleDAO>();
builder.Services.AddScoped<TagDAO>();

// Repositories
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

// Services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<INewsArticleService, NewsArticleService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IReportService, ReportService>();


// Configure OData
var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<BussinessObjects.Models.SystemAccount>("SystemAccount");
modelBuilder.EntitySet<BussinessObjects.Models.Category>("Category");
modelBuilder.EntitySet<BussinessObjects.Models.NewsArticle>("NewsArticle");
modelBuilder.EntitySet<BussinessObjects.Models.Tag>("Tag");

builder.Services.AddControllers()
    .AddOData(options => options
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .Count()
        .SetMaxTop(100)
        .AddRouteComponents("odata", modelBuilder.GetEdmModel()))
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "FU News Management API",
        Version = "v1",
        Description = "FU News Management API with both REST and OData endpoints"
    });

    // Exclude OData controllers from Swagger to avoid conflicts
    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        return !apiDesc.RelativePath?.StartsWith("odata") == true;
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FU News Management API v1");
        c.DocumentTitle = "FU News Management API";
        c.RoutePrefix = "swagger";
        
        // Add custom CSS to show OData info
        c.InjectStylesheet("/swagger-ui/custom.css");
    });
}

app.UseHttpsRedirection();

// Serve static files (for custom CSS)
app.UseStaticFiles();

// Use CORS
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
