// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace CoreBlog.Shared.Storage;

using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.Runtime.Endpoints;
using Amazon.S3;
using Amazon.S3.Internal;
using Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddS3BlogStorage(
    this IServiceCollection services, Action<S3BlogStorageOptions> config)
  {
    services.Configure(config);

    services.AddHttpClient("AWS");

    services.AddDefaultAWSOptions(provider =>
    {
      var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();

      var storageOptions = provider.GetRequiredService<IOptions<S3BlogStorageOptions>>().Value;

      return new AWSOptions
      {
        DefaultClientConfig =
        {
          HttpClientFactory = new AWSHttpClientFactoryWrapper(httpClientFactory),
          EndpointDiscoveryEnabled = true,
          ServiceURL = storageOptions.Endpoint
        },
        Credentials = new BasicAWSCredentials(storageOptions.AccessKey, storageOptions.SecretKey),
        Region = RegionEndpoint.GetBySystemName(storageOptions.Region)
      };
    });

    services.AddAWSService<IAmazonS3>();

    services.TryAddSingleton<IBlogStorage, S3BlogStorage>();

    return services;
  }
}
