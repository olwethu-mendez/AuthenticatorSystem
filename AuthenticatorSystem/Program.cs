using AuthenticatorSystem.Infrastructure;
using BusinessLogicLayer.Hubs;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Middleware;
using BusinessLogicLayer.Services;
using DataAccessLayer.Data;
using DataAccessLayer.Infrastructure;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<ContextAccessorService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<R2Service>();

builder.Services.AddSingleton<ISmsService, TwilioSmsService>();
builder.Services.AddSingleton<IEmailService, MailtrapService>();
builder.Services.AddHostedService<TokenCleanupService>();
builder.Services.AddHttpClient();
builder.Services.AddLogging();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Authentication System API", Version = "1.1.0", Description = "An API for Authentication Upskilling." });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            }, new string[]{}
        }
    });
    c.OperationFilter<DocumentationOperationFilter>();
    c.OperationFilter<AuthorizationRolesOperationFilter>(); //The order here matters as it determines how the Summary (and Description) concatinates
});
#region Auth Stuff
var tokenValidationParameters = new TokenValidationParameters()
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]!)),

    ValidateIssuer = true,
    ValidIssuer = builder.Configuration["JWT:Issuer"],

    ValidateAudience = true,
    ValidAudience = builder.Configuration["JWT:Audience"],

    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero, // remove delay of token when expire
    NameClaimType = ClaimTypes.NameIdentifier // 👈 important for SignalR

};

builder.Services.AddSingleton(tokenValidationParameters); //Injected in AuthController for generating tokens

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 10;
    options.Lockout.AllowedForNewUsers = false;   // Disable for new users
    //options.User.RequireUniqueEmail = true; already do in RegisterUser method
    //below are required for generation otp tokens (as well as AddDefaultTokenProviders())
    options.Tokens.ChangePhoneNumberTokenProvider = "Phone";
    //options.Tokens.ChangeEmailTokenProvider = "Email";
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider; //ensure it generates a 6-digit numeric code
    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    // This affects the long Base64 tokens
    options.TokenLifespan = TimeSpan.FromMinutes(15);
});

builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<RoleManager<IdentityRole>>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = tokenValidationParameters;
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/api/user-hub"))
                {// If the request is for our hub, grab the token from the query string
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(UserPolicies.MustHaveProfile, policy =>
        policy.RequireClaim("HasProfile", "true"));
    options.AddPolicy(UserPolicies.MustBeActivated, policy =>
    policy.RequireClaim("IsActivated", "true"));
});
#endregion

#region Otp Setup
var twilioAccountSid = builder.Configuration["Twilio:TestAccountSID"] ?? "";
var twilioAuthToken = builder.Configuration["Twilio:TestAuthToken"] ?? "";
var twilioPhoneNumber = builder.Configuration["Twilio:TwilioPhoneNumber"] ?? "";
#endregion

#region DB Stuff

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

#endregion



var cultureInfo = new CultureInfo("en-ZA")
{
    NumberFormat = { NumberDecimalSeparator = "." } // Ensure dot is the decimal separator
};
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var app = builder.Build();

//app.UseCors(options =>
//options.WithOrigins(
//        "https://localhost:3000",
//        "http://localhost:3000",
//        "http://10.0.2.2:5233",         // For Android Emulator (if your backend is also on 5233, usually just 10.0.2.2)
//        "http://127.0.0.1:5233"        // For iOS Simulator
//)
//.AllowAnyMethod()
//.AllowAnyHeader()
//.AllowCredentials()
//); // Also relevant for SignalR Hubs


app.UseCors(options =>
options.SetIsOriginAllowed(origin => true)
.AllowAnyMethod()
.AllowAnyHeader()
.AllowCredentials()
); // Also relevant for SignalR Hubs

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() ||
    app.Environment.IsEnvironment("Debug") ||
    app.Environment.IsEnvironment("Test"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<TokenValidationMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<UserHub>("/api/user-hub"); // The endpoint your Flutter app will connect to

// Replace: ApplicationDbInitializer.SeedRoles(app).Wait();
// Replace: ApplicationDbInitializer.SeedAdmin(app).Wait();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
    var services = scope.ServiceProvider;
    try
    {
        // We pass the 'app' or the 'serviceProvider' depending on your method signature
        // Since your methods use IApplicationBuilder, we can call them like this:
        await ApplicationDbInitializer.SeedRoles(app);
        await ApplicationDbInitializer.SeedAdmin(app);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
