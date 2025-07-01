using Npgsql;
using Repositories.Implementation;
using Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Repositories.Interface;
using StackExchange.Redis;
using Repositories.Model;
using RabbitMQ.Client;
using TaskTrackPro.Core.Services.Email;
using Repositories.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Repositories.Implimentations;
using Repositories.Implimentation;
using Repositories.Implementations;
using API.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Services.Configure<EmailService>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSignalR();

// Redis (Singleton)
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisConnectionString = builder.Configuration.GetSection("Redis:ConnectionString").Value;
    if (string.IsNullOrEmpty(redisConnectionString))
    {
        throw new InvalidOperationException("Redis connection string is missing in configuration.");
    }
    return ConnectionMultiplexer.Connect(redisConnectionString);
});

// RabbitMQ (Singleton)
builder.Services.AddSingleton<IConnection>(sp =>
{
    var rabbitMqHost = builder.Configuration.GetSection("RabbitMQ").Value;
    if (string.IsNullOrEmpty(rabbitMqHost))
    {
        throw new InvalidOperationException("RabbitMQ connection string is missing in configuration.");
    }
    Console.WriteLine($"RabbitMQ Connection String: {rabbitMqHost}");
    return new ConnectionFactory() { Uri = new Uri(rabbitMqHost) }.CreateConnection();
});

// RabbitMQ Channel (Scoped)
builder.Services.AddScoped<IModel>(sp =>
{
    var connection = sp.GetRequiredService<IConnection>();
    return connection.CreateModel();
});

// Elasticsearch Configuration
var jobsConfig = builder.Configuration.GetSection("Elasticsearch:Jobs").Get<Dictionary<string, string>>();
var candidatesConfig = builder.Configuration.GetSection("Elasticsearch:Candidates").Get<Dictionary<string, string>>();

// Configure Jobs Elasticsearch Client
var jobsSettings = new ElasticsearchClientSettings(new Uri(jobsConfig["Uri"]))
    .Authentication(new BasicAuthentication(jobsConfig["Username"], jobsConfig["Password"]));
var jobsClient = new ElasticsearchClient(jobsSettings);

// Configure Candidates Elasticsearch Client
var candidatesSettings = new ElasticsearchClientSettings(new Uri(candidatesConfig["Uri"]))
    .Authentication(new BasicAuthentication(candidatesConfig["Username"], candidatesConfig["Password"]));
var candidatesClient = new ElasticsearchClient(candidatesSettings);


//Elasticsearch Services
builder.Services.AddScoped<JobSearchServices>(sp => 
    new JobSearchServices(jobsClient, jobsConfig["Index"]));

// Services
builder.Services.AddSingleton<EmailService>();
builder.Services.AddScoped<IAuthInterface, AuthRepository>();
builder.Services.AddSingleton<UserRegistrationConsumer>(); // Changed to Singleton
builder.Services.AddScoped<IJobPostInterface, JobPostRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddHostedService<NotificationConsumerService>();
builder.Services.AddScoped<IIndustryInterface, IndustryRepository>();
builder.Services.AddScoped<ICandidateInterface, CandidateRepository>();

builder.Services.AddScoped<IApplyjobInterface,ApplyJobRepository>();
// Cloudinary
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddSingleton<CloudinaryService>();
// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "YourSecureKeyHere")),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

// Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaskTrackPro API",
        Version = "v1",
        Description = "API documentation for TaskTrackPro"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer <your-token>' in the field below."
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
            Array.Empty<string>()
        }
    });
});
builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory
    {
        HostName = "localhost",
        Port = 5672,
        UserName = "guest",
        Password = "guest"
    };
    return factory.CreateConnection();
});

// Repositories
builder.Services.AddScoped<IUserProfileDetail, UserProfileDetail>();
builder.Services.AddScoped<IJobPreference, JobPreferenceRepository>();
builder.Services.AddScoped<IUserProjects, UserProjectsRepository>();
builder.Services.AddScoped<IUserCertificate, UserCertificateRepository>();
builder.Services.AddScoped<IUserResume, UserResumeRepository>();
builder.Services.AddScoped<IUserSkills, UserSkillsRepository>();
builder.Services.AddScoped<IEducation_Details, EducationRepository>();
builder.Services.AddScoped<IWorkExperience, WorkExperienceRepository>();
builder.Services.AddScoped<IPostedJobInterface, PostedJobRepository>();
builder.Services.AddScoped<IAdminInterface, AdminRepository>();
builder.Services.AddScoped<ICompanyInterface, CompanyRepository>();

// PostgreSQL
builder.Services.AddScoped<NpgsqlConnection>(sp => 
    new NpgsqlConnection(sp.GetRequiredService<IConfiguration>().GetConnectionString("pgcon")));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("corsapp", policy =>
    {
        policy.WithOrigins("*")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors("corsapp");
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

// Start RabbitMQ Consumer (now works because it's Singleton)
var consumer = app.Services.GetRequiredService<UserRegistrationConsumer>();
Task.Run(() => consumer.StartListening());

app.MapHub<NotificationHub>("/notificationHub");
app.MapControllers();
app.Run();