// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace CoreBlog.Server.Services;

public interface IRssFeedProvider
{
  string CreateFeed(string baseUrl, string? tag = null);
}
