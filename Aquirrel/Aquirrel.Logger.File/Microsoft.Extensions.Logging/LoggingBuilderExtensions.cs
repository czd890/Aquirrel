using Aquirrel.Logger.File;
using Aquirrel.Logger.File.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Logging
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddSingleton<FileLoggerSettings>(new FileLoggerSettings(configuration));
            builder.Services.AddSingleton<IFileFormatProvider, FileFormatProvider>();
            builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();

            return builder;
        }
    }
}
