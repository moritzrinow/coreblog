// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace CoreBlog.Server.Services;

using Microsoft.AspNetCore.Components;

public interface IPostContentService
{
  ValueTask<MarkupString?> GetPostContentAsync(
    string id, CancellationToken cancellationToken = default);
}
