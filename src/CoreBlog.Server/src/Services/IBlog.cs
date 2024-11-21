// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace CoreBlog.Server.Services;

using System.Globalization;
using Shared.Models;

public interface IBlog
{
  BlogMetadata Metadata { get; }

  IEnumerable<PostMetadata> Posts { get; }

  IReadOnlyDictionary<string, int> Tags { get; }

  CultureInfo Culture { get; }
}
