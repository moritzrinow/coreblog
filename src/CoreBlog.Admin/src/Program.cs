// Copyright (c) 2024 Moritz Rinow. All rights reserved.

using CoreBlog.Admin.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
  .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
}

app.MapStaticAssets();

app.MapRazorComponents<App>()
  .AddInteractiveServerRenderMode();

app.Run();
