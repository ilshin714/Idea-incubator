/*
 * Copyright (C) 2023 IKTSolution
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
 * OR OTHER DEALINGS IN THE SOFTWARE.
 */

using IdeaIncubatorBlazor.Hubs;
using IdeaIncubatorBlazor.Models;
using IdeaIncubatorBlazor.Services.Ideas;
using IdeaIncubatorBlazor.Services.Users;
using IdeaIncubatorBlazor.Utils.Loggings;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using MudBlazor.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);
IConfiguration _config = new ConfigurationBuilder()
    .AddConfiguration(builder.Configuration)
    .AddEnvironmentVariables("IDEA_")
    .Build();
var dbConfig = _config.GetSection("DbConfig");

if (_config.GetValue<string>("UseMicrosoftIdentity") == "1")
{
    builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(_config.GetSection("AzureAd"));

    builder.Services.AddControllersWithViews()
        .AddMicrosoftIdentityUI();
}

builder.Services.AddDbContext<IdeaIncubatorDbContext>(options =>
{
    options.UseSqlServer(
        "Server=tcp:" + dbConfig.GetValue<string>("Server") + "," + dbConfig.GetValue<string>("ServerPort") + ";" +
        "Initial Catalog=IdeaIncubatorDB;Persist Security Info=False;" +
        "User ID=" + dbConfig.GetValue<string>("UserId") + ";" +
        "Password=" + dbConfig.GetValue<string>("Password") + ";" +
        "MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30"
        );
});

builder.Services.AddRazorPages(options =>
    { options.RootDirectory = "/Views/Pages"; });
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(options =>
    options.MimeTypes = ResponseCompressionDefaults
    .MimeTypes
    .Concat(new[] { "application/octet-stream" })
);
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddScoped<ILogger, Logger<LoggingIdeaIncubator>>();
builder.Services.AddScoped<ILoggingIdeaIncubator, LoggingIdeaIncubator>();
builder.Services.AddScoped<IIdeaService, IdeaService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IContentService, ContentService>(); 
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserIdeaRoleService, UserIdeaRoleService>();
builder.Services.AddScoped<IChatService, ChatService>();

var app = builder.Build();

app.UseResponseCompression();
app.UseAuthentication();
app.UseAuthorization();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapHub<ChatHub>("/chathub");
app.MapFallbackToPage("/_Host");
app.MapControllers();
app.Run();
