// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace CoreBlog.Server.Services;

using System.Globalization;
using Microsoft.Extensions.Options;
using Options;
using Shared.Models;
using Shared.Storage;

public class BlogSyncBackgroundService(
  Blog blog,
  IBlogStorage storage,
  ILoggerFactory loggerFactory,
  IOptions<ServerOptions> options) : BackgroundService
{
  private readonly ILogger logger = loggerFactory.CreateLogger("CoreBlog");

  public override async Task StartAsync(CancellationToken cancellationToken)
  {
    await this.ImplAsync(cancellationToken).ConfigureAwait(false);

    await base.StartAsync(cancellationToken).ConfigureAwait(false);
  }

  protected override async Task ExecuteAsync(
    CancellationToken stoppingToken)
  {
    using var timer = new PeriodicTimer(TimeSpan.FromMinutes(options.Value.SyncPeriodMinutes));

    while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false))
    {
      try
      {
        await this.ImplAsync(stoppingToken).ConfigureAwait(false);
      }
      catch (OperationCanceledException)
      {
      }
    }
  }

  private async Task ImplAsync(CancellationToken cancellationToken)
  {
    Log.SyncingBlog(this.logger);

    var metadata = await storage.GetBlogAsync(cancellationToken).ConfigureAwait(false);

    if (metadata is not null)
    {
      blog.Metadata = metadata;
    }

    var posts = new List<PostMetadata>();

    await foreach (var post in storage.GetPostsAsync(cancellationToken).ConfigureAwait(false))
    {
      posts.Add(post);
    }

    blog.Posts = posts;

    blog.Culture = CultureInfo.GetCultureInfo(blog.Metadata.Language);

    blog.Tags = posts
      .Where(e => e is { Hidden: false, Published: true })
      .SelectMany(e => e.Tags)
      .GroupBy(e => e)
      .ToDictionary(e => e.Key, e => e.Count());

    Log.SyncedBlog(this.logger);
  }
}
