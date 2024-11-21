# CoreBlog

CoreBlog is a super simple blog engine, that does not require a database.
All you need is an S3 bucket.

Demo: https://coreblog-demo.moritzrinow.com

- ✅ Multiple themes including dark and white modes
- ✅ Supports multiple languages including `en`, `de`, `es`, `it`, `fr`, `uk`, `ru`
- ✅ SEO friendly
- ✅ Low resource requirements (100MiB RAM)
- ✅ No database required
- ✅ Multiple fonts available
- ✅ Allows embedding assets in posts
- ✅ Docker image
- ✅ Mobile friendly
- ❌ Sadly no UI to edit the blog yet (probably coming in the future)
  - I've been using DataGrip + Big Data Tools (Plugin) to edit the S3 bucket
  - Maybe a simple CLI could also come in handy
- ❌ No real-time blog/post updates (data is pulled in intervals)

## Configuration
`coreblog.yaml`:
```yaml
s3:
  endpoint: 'https://fsn1.your-objectstorage.com'
  region: 'fsn1'
  bucketName: 'my-blog' # Required, should be private
  assetBucketName: 'my-blog-assets' # Optional, must be publicly accessible
  accessKey: 'HDY...'
  secretKey: 'r78...'
syncPeriodMinutes: 60 # Every hour blog and post metadata is pulled from S3
postContentCacheTtlMinutes: 60 # Post content is cached in-memory
```

## Install with Docker

Create a `coreblog.yaml` file with your configurations.

Run the docker container:
```text
docker run -p 5000:5000 -v /my/coreblog.yaml:/coreblog/coreblog.yaml devmojo/coreblog
```

You can specify a different path for the config file:
```text
docker run devmojo/coreblog --config /path/to/config.yaml
```

Or specify everything with environment variables:
- CB_S3__Endpoint
- CB_S3__Region
- CB_S3__BucketName
- CB_S3__AssetBucketName
- CB_S3__AccessKey
- CB_S3__SecretKey
- CB_SyncPeriodMinutes
- CB_PostContentCacheTtlMinutes

## S3 bucket layout and file formats
```text
blog.json
posts/
    post-1.json
    post-1.md
    post-2.json
    post-2.md
```
The S3 bucket stores 3 different kind of files:
- 1 blog manifest file (`blog.json`)
  - Contains metadata about the blog
- N post manifest files (`posts/post-id.json`)
  - Contains metadata about a post
- N post content files (`posts/post-id.md`)
  - Contains the post content written in Markdown

### Blog manifest (`blog.json`):
```json5
{
  "title": "CoreBlog Demo",
  "author": "CoreBlog",
  "description": "Welcome to the CoreBlog demo!",
  "homepage": "https://github.com/moritzrinow/coreblog", // Your personal homepage
  "theme": "standard",
  "language": "en", // Controls language of UI elements
  "fontFamily": "Roboto Mono",
  "additionalPageMeta": {
    // You can use this to perform things like Google site verifications
  }
}
```

If the property `homepage` is specified, a link named 'About Me' will be displayed in the header.

Supported themes:
- `material`
- `material-dark`
- `standard`
- `standard-dark` (used in demo)
- `default`
- `dark`
- `humanistic`
- `humanistic-dark`
- `software`
- `software-dark`

Supported fonts:
- `Roboto Mono`
- `Roboto`
- `sans-serif`

Supported languages:
- `en` (English)
- `de` (German)
- `es` (Spanish)
- `it` (Italian)
- `fr` (French)
- `uk` (Ukrainian)
- `ru` (Russian)

If you want to display a **favicon**, you have to put a file named `favicon.ico` in the asset bucket.  

### Post manifest (`post-id.json`):
```json5
{
  "id": "example-1", // URL slug, must be same as file prefix
  "title": "How to host your own managed Kubernetes cluster in the cloud",
  "language": "en", // Language of the actual content
  "summary": "In this post I will show you how to host your own Kubernetes cluster in the cloud.",
  "published": true,
  "hidden": false, // Can be used to have unlisted posts
  "date": "2024-11-21",
  "thumbnail": "welcome.jpg", // Image stored in asset bucket
  "tags": [
    "kubernetes",
    "cloud"
  ]
}
```

### Embedding images (assets)

If you want to make use of static assets, such as images, an optional second S3 bucket comes into play.

It **must** be publicly accessible to serve the assets directly.

To include assets in your blog-post Markdown, use relative links prefixed with `assets/`:

```markdown
The following is an image from my assets bucket:
[My Image](assets/my-image.png)
```

Links starting with `assets/` will be rewritten to point to the S3 asset-bucket directly.
