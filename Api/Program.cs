
using Api.Extensions;
using ApplicationServices.Extensions;
using DB.Extensions;
using Serilog;
using Shared.Configuration;
using Utility.Extensions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var loggerFactory = LoggerFactory
        .Create(builder =>
        {
            builder.ClearProviders();
            builder.AddConsole();
        });
    var logger = loggerFactory.CreateLogger<Program>();

    // Start Builder
    var builder = WebApplication.CreateBuilder(args);

    // get Configuration
    IConfiguration configuration = builder.Configuration;
    var appConfig = configuration.GetSection(AppConfiguration.SectionLabel);
    var connectionConfig = configuration.GetSection(ConnectionStrings.SectionLabel);


    // Register Identity Storage
    builder.Services.RegisterCoreStorage(configuration);
    builder.Services.RegisterCoreInfraServices();

    // Add Authentication and Current user Services
    builder.Services.AddAppIdentity();
    builder.Services.AddJwtAuthentication(appConfig.Get<AppConfiguration>(), logger);

    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    // Bind Options for use in Services
    builder.Services.AddOptions();
    builder.Services.Configure<AppConfiguration>(appConfig);
    builder.Services.Configure<ConnectionStrings>(connectionConfig);
    builder.Services.Configure<MailConfiguration>(configuration.GetSection(MailConfiguration.SectionLabel));
    builder.Services.Configure<AWSS3Config>(configuration.GetSection(AWSS3Config.Label));

    builder.Services.AddHttpClient();

    // Add Core Services and Repository
    builder.Services.RegisterParentAppLayer();


    //[AA] => For External services like Redis Caching, Azure File Storage, Email service
    builder.Services.RegisterParentUtilsServices(connectionConfig.Get<ConnectionStrings>());


    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("Production", policy =>
        {
            policy.WithOrigins("http://localhost:800", "http://52.66.127.231")
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseCors("Production");
    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    if (ex.GetType().Name != "StopTheHostException")
    {
        Log.Fatal(ex, "Unhandled exception");
    }
}
finally
{

    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}

