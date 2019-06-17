﻿using System.Reflection;
using Microsoft.Extensions.FileProviders;

namespace NToastNotify.Helpers
{
    public static class Utils
    {
        private static readonly Assembly ThisAssembly = typeof(NToastNotifyViewComponent).Assembly;

        public static EmbeddedFileProvider GetEmbeddedFileProvider()
        {
            return new EmbeddedFileProvider(ThisAssembly, "NToastNotify");
        }

        public static ILibrary GetLibraryDetails<T>(NToastNotifyOption nToastNotifyOptions, ILibraryOptions defaultOptions) where T : class, ILibrary, new()
        {
            var library = new T();
            if (nToastNotifyOptions != null)
            {
                if (!string.IsNullOrWhiteSpace(nToastNotifyOptions.ScriptSrc))
                {
                    library.ScriptSrc = nToastNotifyOptions.ScriptSrc;
                }
                if (!string.IsNullOrWhiteSpace(nToastNotifyOptions.StyleHref))
                {
                    library.StyleHref = nToastNotifyOptions.StyleHref;
                }
            }

            if (defaultOptions != null)
            {
                library.Options = defaultOptions;
            }
            return library;
        }
    }
}
