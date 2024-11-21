// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace CoreBlog.Shared.Storage;

using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Models;

public class S3BlogStorage(
  IOptions<S3BlogStorageOptions> options,
  IAmazonS3 client) : IBlogStorage
{
  public const string BlogFile = "blog.json";

  public const string PostsPrefix = "posts";

  private static readonly JsonSerializerOptions jsonOptions = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
  };

  private readonly string bucketName = options.Value.BucketName;

  public string? GetAssetUrl(string id)
    => options.Value.AssetBucketName is { } assetBucketName ? new Uri(new Uri(options.Value.Endpoint), $"{assetBucketName}/{id}").ToString() : null;

  public async Task<BlogMetadata?> GetBlogAsync(
    CancellationToken cancellationToken = default)
  {
    try
    {
      using var response = await client.GetObjectAsync(
        this.bucketName, BlogFile, cancellationToken).ConfigureAwait(false);

      if (response.HttpStatusCode is not HttpStatusCode.OK)
      {
        return null;
      }

      var blog = await JsonSerializer.DeserializeAsync<BlogMetadata>(
        response.ResponseStream, jsonOptions, cancellationToken: cancellationToken).ConfigureAwait(false);

      return blog;
    }
    catch (AmazonS3Exception ex) when (ex is { StatusCode: HttpStatusCode.NotFound })
    {
      return null;
    }
  }

  public async Task SetBlogAsync(
    BlogMetadata blog,
    CancellationToken cancellationToken = default)
  {
    var stream = new MemoryStream();

    JsonSerializer.Serialize(stream, blog, jsonOptions);

    stream.Position = 0;

    await client.PutObjectAsync(new PutObjectRequest
    {
      BucketName = this.bucketName,
      Key = BlogFile,
      InputStream = stream,
      ContentType = "application/json"
    }, cancellationToken).ConfigureAwait(false);
  }

  public async IAsyncEnumerable<PostMetadata> GetPostsAsync(
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    Task<ListObjectsV2Response> task = client.ListObjectsV2Async(new ListObjectsV2Request
    {
      BucketName = this.bucketName,
      Prefix = $"{PostsPrefix}/"
    }, cancellationToken);

    ListObjectsV2Response response;

    do
    {
      response = await task.ConfigureAwait(false);

      foreach (var item in response.S3Objects.Where(item => item.Key.EndsWith(".json")))
      {
        PostMetadata? post = null;

        try
        {
          await using var stream = await client.GetObjectStreamAsync(
            this.bucketName, item.Key, null, cancellationToken: cancellationToken).ConfigureAwait(false);

          post = await JsonSerializer.DeserializeAsync<PostMetadata>(
            stream, jsonOptions, cancellationToken).ConfigureAwait(false);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode is HttpStatusCode.NotFound)
        {
        }

        if (post is not null)
        {
          yield return post;
        }
      }

      if (response.IsTruncated)
      {
        task = client.ListObjectsV2Async(new ListObjectsV2Request
        {
          BucketName = this.bucketName,
          Prefix = $"{PostsPrefix}/",
          ContinuationToken = response.NextContinuationToken
        }, cancellationToken);
      }
    } while (response.IsTruncated);
  }

  public async Task<PostMetadata?> GetPostAsync(
    string id,
    CancellationToken cancellationToken = default)
  {
    try
    {
      using var response = await client.GetObjectAsync(
        this.bucketName, $"{PostsPrefix}/{id}.json", cancellationToken: cancellationToken).ConfigureAwait(false);

      if (response.HttpStatusCode is not HttpStatusCode.OK)
      {
        return null;
      }

      var blog = await JsonSerializer.DeserializeAsync<PostMetadata>(
        response.ResponseStream, jsonOptions, cancellationToken).ConfigureAwait(false);

      return blog;
    }
    catch (AmazonS3Exception ex) when (ex is { StatusCode: HttpStatusCode.NotFound })
    {
      return null;
    }
  }

  public async Task<Stream?> GetPostContentAsync(
    string id,
    CancellationToken cancellationToken = default)
  {
    try
    {
      var response = await client.GetObjectAsync(
        this.bucketName, $"{PostsPrefix}/{id}.md", cancellationToken: cancellationToken).ConfigureAwait(false);

      return response.HttpStatusCode is not HttpStatusCode.OK ? null : response.ResponseStream;
    }
    catch (AmazonS3Exception ex) when (ex is { StatusCode: HttpStatusCode.NotFound })
    {
      return null;
    }
  }
}
