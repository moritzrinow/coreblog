// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace CoreBlog.Server.Services;

using System.Globalization;
using Shared.Models;

public class Blog : IBlog
{
  public BlogMetadata Metadata { get; set; } = new();

  public IEnumerable<PostMetadata> Posts { get; set; } = [];

  public IReadOnlyDictionary<string, int> Tags { get; set; } = new Dictionary<string, int>();

  public CultureInfo Culture { get; set; } = CultureInfo.GetCultureInfo("en");
}
