// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace CoreBlog.Shared.Storage;

using Models;

public interface IBlogStorage
{
  string? GetAssetUrl(string id);

  Task<BlogMetadata?> GetBlogAsync(CancellationToken cancellationToken = default);

  Task SetBlogAsync(BlogMetadata blog, CancellationToken cancellationToken = default);

  IAsyncEnumerable<PostMetadata> GetPostsAsync(CancellationToken cancellationToken = default);

  Task<PostMetadata?> GetPostAsync(string id, CancellationToken cancellationToken = default);

  Task<Stream?> GetPostContentAsync(string id, CancellationToken cancellationToken = default);
}
