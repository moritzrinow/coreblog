// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace CoreBlog.Shared.Models;

public record PostMetadata
{
  /// <summary>
  /// URL-friendly identifier of the blog (slug).
  /// </summary>
  public string Id { get; set; } = null!;

  /// <summary>
  /// Title of the post.
  /// </summary>
  public string Title { get; set; } = null!;

  /// <summary>
  /// Short summary of the post.
  /// </summary>
  public string? Summary { get; set; }

  /// <summary>
  /// Asset name of the thumbnail.
  /// </summary>
  public string? Thumbnail { get; set; }

  /// <summary>
  /// User-defined date, when this post was published. This does not have to reflect reality.
  /// </summary>
  public DateTime? Date { get; set; }

  /// <summary>
  /// Language the post is written in. This can be any two-letter ISO language code.
  /// </summary>
  public string Language { get; set; } = null!;

  /// <summary>
  /// Whether this post is unlisted. Unlisted posts do not appear in the posts overview.
  /// </summary>
  public bool Hidden { get; set; }

  /// <summary>
  /// Whether this post is published and available. Unpublished posts are unlisted implicitly.
  /// </summary>
  public bool Published { get; set; }

  /// <summary>
  /// List of tags associated with the post.
  /// </summary>
  public IList<string> Tags { get; set; } = new List<string>();
}
