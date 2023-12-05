using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Example.FileInfoApp;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;

const string APP_TITLE_DEFAULT = "File Information App";
const long MAX_REQUEST_SIZE = 4L * 1024 * 1024 * 1024; // 4 GiB

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddOptions<AppConfiguration>()
    .Configure<IConfiguration>((options, configuration) =>
    {
        configuration.GetSection(AppConfiguration.Name).Bind(options);
    });


builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = MAX_REQUEST_SIZE;
});



builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
    });

builder.Services
    //Add Api versioning
    .AddApiVersioning(options =>
    {
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
    })
    .AddApiExplorer(options =>
    {
        //Add VersionedApi Explorer
        //The format of the version added to the route URL
        options.GroupNameFormat = "'v'VVV";
        //Tells swagger to replace the version in the controller route
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = MAX_REQUEST_SIZE;
});

// builder.WebHost
//     .ConfigureKestrel(options =>
//     {
//         options.Limits.MaxRequestBodySize = MAX_REQUEST_SIZE;
//         options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(15); // 15 Minutes
//     })
// #if !DEBUG
//     .UseIISIntegration()
// #endif
//     ;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Use swagger UI always
}
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
var appConfigurationAccessor = app.Services.GetRequiredService<IOptionsMonitor<AppConfiguration>>();
var appConfiguration = appConfigurationAccessor?.CurrentValue;

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DocumentTitle = appConfiguration?.Title ?? APP_TITLE_DEFAULT;

    // build a swagger endpoint for each discovered API version
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
    }

    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.MapControllers()
    .WithOpenApi();

app.Run();
