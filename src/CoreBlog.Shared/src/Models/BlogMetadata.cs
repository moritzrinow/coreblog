// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace CoreBlog.Shared.Models;

public record BlogMetadata
{
  /// <summary>
  /// Title of the blog.
  /// </summary>
  public string Title { get; set; } = "Blog";

  /// <summary>
  /// Name of the blog author.
  /// </summary>
  public string? Author { get; set; }

  /// <summary>
  /// Description of the blog.
  /// </summary>
  public string? Description { get; set; }

  /// <summary>
  /// Language of the blog. Default is 'en'.
  /// </summary>
  public string Language { get; set; } = "en";

  /// <summary>
  /// Theme of the blog.
  /// </summary>
  public string Theme { get; set; } = "standard";

  /// <summary>
  /// Homepage of the blog owner.
  /// </summary>
  public string? Homepage { get; set; }

  /// <summary>
  /// Font-family for every text.
  /// </summary>
  public string FontFamily { get; set; } = "Roboto, sans-serif";

  /// <summary>
  /// Line-height for every text.
  /// </summary>
  public double LineHeight { get; set; } = 2.0;

  /// <summary>
  /// Additional meta elements added to the HTML `head`.
  /// </summary>
  public Dictionary<string, string> AdditionalPageMeta { get; set; } = [];
}
