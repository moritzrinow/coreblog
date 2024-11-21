// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace CoreBlog.Shared.Storage;

public record S3BlogStorageOptions
{
  /// <summary>
  /// S3 endpoint.
  /// </summary>
  public string Endpoint { get; set; } = null!;

  /// <summary>
  /// S3 region.
  /// </summary>
  public string Region { get; set; } = null!;

  /// <summary>
  /// Name of the bucket storing assets.
  /// This bucket must be publicly accessible when specified.
  /// </summary>
  public string? AssetBucketName { get; set; }

  /// <summary>
  /// Name of the bucket storing blog and posts.
  /// This bucket is required and should be private.
  /// </summary>
  public string BucketName { get; set; } = null!;

  /// <summary>
  /// S3 access-key.
  /// </summary>
  public string AccessKey { get; set; } = null!;

  /// <summary>
  /// S3 private-key.
  /// </summary>
  public string SecretKey { get; set; } = null!;
}
