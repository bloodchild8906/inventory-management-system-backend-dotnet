using Inventory.Application.Infrastructure.Persistence;
using Inventory.Application.Interfaces.Service;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Features.Settings.Queries
{
    public record GetCompanyLogoQuery() : IRequest<GetCompanyLogoResponse>;

    public class GetCompanyLogo : IRequestHandler<GetCompanyLogoQuery, GetCompanyLogoResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IPathComposer _pathComposer;

        public GetCompanyLogo(ApplicationDbContext context, IPathComposer pathComposer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _pathComposer = pathComposer ?? throw new ArgumentNullException(nameof(pathComposer));
        }

        public async Task<GetCompanyLogoResponse> Handle(GetCompanyLogoQuery request, CancellationToken cancellationToken)
        {
            var companyInfo = await _context.CompanyInfo.Include(c => c.Logo).FirstOrDefaultAsync();
            var filePath = _pathComposer.ComposeCompanyLogoPath(companyInfo.Logo.Filename);
            var contentType = companyInfo.Logo.FileType;
            return new GetCompanyLogoResponse
            {
                FilePath = filePath,
                ContentType = contentType
            };
        }
    }

    public class GetCompanyLogoResponse
    {
        public string FilePath { get; set; } = default!;
        public string ContentType { get; set; } = default!;
    }
}
