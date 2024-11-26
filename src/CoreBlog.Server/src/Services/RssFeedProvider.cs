// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace CoreBlog.Server.Services;

using System.Text;
using WilderMinds.RssSyndication;

public class RssFeedProvider(IBlog blog) : IRssFeedProvider
{
  private static readonly SerializeOption serializeOption = new()
  {
    Encoding = Encoding.UTF8
  };

  public string CreateFeed(string baseUrl, string? tag = null)
  {
    var feedUri = new Uri(baseUrl);

    var feed = new Feed
    {
      Title = blog.Metadata.Title,
      Description = blog.Metadata.Description,
      Link = feedUri,
      Language = blog.Metadata.Language
    };

    var posts = blog.Posts.Where(e => e.Published);

    if (!string.IsNullOrEmpty(tag))
    {
      posts = posts.Where(e => e.Tags.Contains(tag));
    }

    foreach (var post in posts)
    {
      if (post.Hidden)
      {
        continue;
      }

      var postUri = new Uri(feedUri, post.Id);

      var item = new Item
      {
        Guid = postUri.ToString(),
        Title = post.Title,
        Author = blog.Metadata.Author is { } author
          ? new Author
          {
            Name = author
          }
          : null,
        PublishDate = post.Date ?? DateTime.Now.Date,
        Link = postUri,
        Permalink = postUri.ToString(),
        Categories = post.Tags,
        Body = post.Summary
      };

      feed.Items.Add(item);
    }

    return feed.Serialize(serializeOption);
  }
}
