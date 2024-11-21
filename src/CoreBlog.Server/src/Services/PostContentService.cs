// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace CoreBlog.Server.Services;

using System.Text;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Options;
using Shared.Storage;

public class PostContentService(
  IBlogStorage storage,
  IMemoryCache cache,
  AssetLinkRewriter assetLinkRewriter,
  IOptions<ServerOptions> options) : IPostContentService
{
  private readonly MarkdownPipeline markdownPipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .Use(assetLinkRewriter)
    .Build();

  public ValueTask<MarkupString?> GetPostContentAsync(
    string id,
    CancellationToken cancellationToken = default)
  {
    return cache.TryGetValue(id, out string? cached)
      ? ValueTask.FromResult<MarkupString?>(new MarkupString(cached ?? string.Empty))
      : new ValueTask<MarkupString?>(GetInternalAsync());

    async Task<MarkupString?> GetInternalAsync()
    {
      await using var stream = await storage.GetPostContentAsync(id, cancellationToken).ConfigureAwait(false);

      if (stream is null)
      {
        return null;
      }

      using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);

      var content = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);

      var html = Markdown.ToHtml(content, this.markdownPipeline);

      cache.Set(id, html, TimeSpan.FromMinutes(options.Value.PostContentCacheTtlMinutes));

      return new MarkupString(html);
    }
  }
}
