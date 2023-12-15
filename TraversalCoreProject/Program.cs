using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using BusinessLayer.Container;
using BusinessLayer.ValidationRules;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using DTOLayer.DTOs.AnnouncementDTOs;
using EntityLayer.Concrete;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Logging;
using TraversalCoreProje.CQRS.Handlers.DestinationHandlers;
using TraversalCoreProject.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging(x => {
    x.ClearProviders();
    x.SetMinimumLevel(LogLevel.Debug);
    x.AddDebug();

    });
builder.Services.AddDbContext<Context>();
builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<Context>().AddErrorDescriber<CustomIdentityValidator>().AddEntityFrameworkStores<Context>();

builder.Services.AddScoped<GetAllDestinationQueryHandler>();
builder.Services.AddScoped<GetDestinationByIDQueryHandler>();
builder.Services.AddScoped<CreateDestinationCommandHandler>();
builder.Services.AddScoped<RemoveDestinationCommandHandler>();
builder.Services.AddScoped<UpdateDestinationCommandHandler>();

builder.Services.AddMediatR(typeof(Program));

builder.Services.AddHttpClient();

builder.Services.ContainerDependencies();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddTransient<IValidator<AnnouncementAddDto>, AnnouncementValidator>();

builder.Services.AddControllersWithViews().AddFluentValidation();

builder.Services.AddControllersWithViews();
builder.Services.AddMvc(config=>
{
    var policy=new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
    .Build();
    config.Filters.Add(new AuthorizeFilter());
    
});

builder.Services.AddMvc();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login/SignIn/";
});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/ErrorPage/Error404", "?code={0}");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints=>
{
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
        );

});
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
        );

});
app.Run();
