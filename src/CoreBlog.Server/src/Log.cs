// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace CoreBlog.Server;

public static partial class Log
{
  [LoggerMessage(Level = LogLevel.Information, Message = "Syncing blog with external storage.")]
  public static partial void SyncingBlog(ILogger logger);

  [LoggerMessage(Level = LogLevel.Information, Message = "Successfully synced blog with external storage.")]
  public static partial void SyncedBlog(ILogger logger);
}
