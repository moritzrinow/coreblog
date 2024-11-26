// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace CoreBlog.Server.Options;

using Shared.Storage;

public record ServerOptions
{
  /// <summary>
  /// Time in minutes between blog synchronizations with storage.
  /// </summary>
  public int SyncPeriodMinutes { get; set; } = 60;

  /// <summary>
  /// Time in minutes the content of posts is cached.
  /// </summary>
  public int PostContentCacheTtlMinutes { get; set; } = 60;

  /// <summary>
  /// S3 configuration.
  /// </summary>
  public S3BlogStorageOptions S3 { get; set; } = new();

  /// <summary>
  /// Whether to use the simple log formatter instead of json.
  /// Useful for local development purposes.
  /// </summary>
  public bool SimpleLogFormatter { get; set; }

  /// <summary>
  /// Whether all existing posts are considered published.
  /// </summary>
  public bool AutoPublish { get; set; }

  /// <summary>
  /// Path-base for the blog when hosted on a different sub-path.
  /// </summary>
  public string? PathBase { get; set; }
}
