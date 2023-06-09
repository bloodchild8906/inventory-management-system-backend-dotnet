﻿using Inventory.Application.Interfaces.Service;
using Microsoft.Extensions.Options;

namespace Inventory.Application.Infrastructure.Service
{
    public class ApplicationUri
    {
        public string BaseUri { get; set; } = default!;
    }

    public class UriComposer : IUriComposer
    {
        private readonly ApplicationUri _applicationUri;

        public UriComposer(IOptions<ApplicationUri> options)
        {
            _applicationUri = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public string ComposeLogoUri()
        {
            return $"{_applicationUri.BaseUri}/Settings/CompanyInfo/Logo";
        }
    }
}
