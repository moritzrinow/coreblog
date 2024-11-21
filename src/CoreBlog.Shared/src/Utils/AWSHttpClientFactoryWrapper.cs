// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace CoreBlog.Shared.Utils;

using Amazon.Runtime;

public class AWSHttpClientFactoryWrapper(IHttpClientFactory factory) : HttpClientFactory
{
  public override HttpClient CreateHttpClient(IClientConfig clientConfig)
  {
    var httpClient = factory.CreateClient("AWS");

    return httpClient;
  }

  public override bool DisposeHttpClientsAfterUse(IClientConfig clientConfig)
    => true;

  public override bool UseSDKHttpClientCaching(IClientConfig clientConfig)
    => false;
}
