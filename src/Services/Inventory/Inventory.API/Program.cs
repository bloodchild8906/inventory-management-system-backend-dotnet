using Application.Shared.Middlewares;
using Inventory.API.Extensions;
using Inventory.Application.Domain.Identity;
using Inventory.Application.Infrastructure.Persistence;
using Inventory.Application.Infrastructure.Persistence.Seeders;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var myCors = "AppCors";

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(myCors, options=> { options.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod(); });
});
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddPipes();
builder.Services.AddUtils();
builder.Services.AddServices(builder.Configuration);
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddAuthrorizationPolicies();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(myCors);

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("app");
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        logger.LogInformation("Seeding default Users and Roles");
        await ModulesSeeder.CreateModulesWithPermissions(context);
        await RolesSeeder.CreateRoles(roleManager, context);
        await UsersSeeder.CreateUsers(userManager);
        await CompanyInfoSeeder.CreateDefaultCompanyInfo(context);
        logger.LogInformation("Finished Seeding Default Data");
        logger.LogInformation("Application Starting");
    }
    catch (Exception exception)
    {
        logger.LogInformation(exception, "An error occurred seeding the DB");
    }
}


app.Run();
