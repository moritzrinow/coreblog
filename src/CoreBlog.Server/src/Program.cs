// Copyright (c) 2024 Moritz Rinow. All rights reserved.

using System.Globalization;
using System.Text;
using CoreBlog.Server.Components;
using CoreBlog.Server.Options;
using CoreBlog.Server.Services;
using CoreBlog.Shared.Storage;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.Sources.Clear();

builder.Configuration.AddCommandLine(args);

var configPath = builder.Configuration["Config"] ?? "coreblog.yaml";

builder.Configuration.AddYamlFile(configPath, true, false);

if (builder.Environment.IsDevelopment())
{
  builder.Configuration.AddYamlFile("coreblog.local.yaml", true, false);
}

builder.Configuration.AddEnvironmentVariables("CB_");

builder.Configuration.AddCommandLine(args);

builder.Logging.ClearProviders();

builder.Logging.AddConsole();

builder.Services.Configure<ServerOptions>(builder.Configuration);

builder.Services.Configure<S3BlogStorageOptions>(builder.Configuration.GetSection(nameof(ServerOptions.S3)));

builder.Services.AddOptions<ConsoleLoggerOptions>()
  .Configure<IOptions<ServerOptions>>((console, options) =>
  {
    console.FormatterName = options.Value.SimpleLogFormatter ? ConsoleFormatterNames.Simple : ConsoleFormatterNames.Json;
  });

builder.Services.AddRazorComponents();

builder.Services.AddRadzenComponents();

builder.Services.AddLocalization();

builder.Services.AddS3BlogStorage(options =>
{
});

builder.Services.AddMemoryCache();

builder.Services.AddSingleton<IPostContentService, PostContentService>();

builder.Services.AddSingleton<IRssFeedProvider, RssFeedProvider>();

builder.Services.AddHostedService<BlogSyncBackgroundService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton(new Blog());

builder.Services.AddSingleton<IBlog>(provider => provider.GetRequiredService<Blog>());

builder.Services.AddSingleton<AssetLinkRewriter>();

var app = builder.Build();

var pathBase = builder.Configuration.GetValue<string?>(nameof(ServerOptions.PathBase));

if (!string.IsNullOrEmpty(pathBase))
{
  app.UsePathBase(pathBase);
}

if (!app.Environment.IsDevelopment())
{
}

app.MapStaticAssets();

app.Use((ctx, next) =>
{
  var blog = ctx.RequestServices.GetRequiredService<IBlog>();

  CultureInfo.CurrentCulture = blog.Culture;

  CultureInfo.CurrentUICulture = blog.Culture;

  return next();
});

app.UseAntiforgery();

app.MapGet("/feed/{tag?}", async (
  string? tag,
  HttpContext ctx,
  [FromServices] IRssFeedProvider provider,
  CancellationToken cancellationToken) =>
{
  var uri = UriHelper.BuildAbsolute(
    ctx.Request.Scheme,
    ctx.Request.Host,
    ctx.Request.PathBase);

  var feed = provider.CreateFeed(uri, tag);

  await ctx.Response.WriteAsync(feed, Encoding.UTF8, cancellationToken).ConfigureAwait(false);
});

app.MapRazorComponents<App>();

app.MapFallback(ctx =>
{
  ctx.Response.Redirect("/");

  return Task.CompletedTask;
});

app.Run();
