using URL_ShortenerAPI.Providers;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

var services = builder.Services;

services.AddControllersWithViews();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddMSSQLServer(config);
services.AddIdentityModels();
services.AddJWTAuthentication(config);
services.AddHttpContextAccessor();
services.AddAccountServices();
services.AddURLShortenerServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

await app.InitializeDataAsync(config);

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();
app.MapControllers();

app.Run();
