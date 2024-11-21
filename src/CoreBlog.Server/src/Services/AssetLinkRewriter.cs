// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace CoreBlog.Server.Services;

using Markdig;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Shared.Storage;

public class AssetLinkRewriter(IBlogStorage storage) : IMarkdownExtension
{
  public void Setup(MarkdownPipelineBuilder pipeline)
  {
    pipeline.DocumentProcessed += this.RewriteAssetLinks;
  }

  public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
  {
  }

  private void RewriteAssetLinks(MarkdownDocument document)
  {
    foreach (var link in document.Descendants<LinkInline>())
    {
      if (!link.IsImage || link.Url?.StartsWith("assets/") is not true)
      {
        continue;
      }

      var assetName = link.Url.Split('/').LastOrDefault();

      if (assetName is null)
      {
        continue;
      }

      if (storage.GetAssetUrl(assetName) is not { } assetUrl)
      {
        continue;
      }

      link.Url = assetUrl;
    }
  }
}
