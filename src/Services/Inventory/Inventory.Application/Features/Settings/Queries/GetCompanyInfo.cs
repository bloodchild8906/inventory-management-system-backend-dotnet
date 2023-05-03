using Application.Shared.Exceptions;
using AutoMapper;
using Inventory.Application.Domain.Settings;
using Inventory.Application.Infrastructure.Persistence;
using Inventory.Application.Interfaces.Service;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Features.Settings.Queries
{
    public record GetCompanyInfoQuery() : IRequest<GetCompanyInfoResponse>;
    public class GetCompanyInfo : IRequestHandler<GetCompanyInfoQuery, GetCompanyInfoResponse>
    {
        private readonly ApplicationDbContext _conext;
        private readonly IUriComposer _uriComposer;
        private readonly IMapper _mapper;

        public GetCompanyInfo(ApplicationDbContext conext, IUriComposer uriComposer, IMapper mapper)
        {
            _uriComposer = uriComposer ?? throw new ArgumentNullException(nameof(uriComposer));
            _conext = conext ?? throw new ArgumentNullException(nameof(conext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<GetCompanyInfoResponse> Handle(GetCompanyInfoQuery request, CancellationToken cancellationToken)
        {
            var companyInfo = await _conext.CompanyInfo.Include(m => m.Logo).FirstOrDefaultAsync();
            if (companyInfo == null)
            {
                throw new NotFoundException("Company Info was not found");
            }
            var response = _mapper.Map<GetCompanyInfoResponse>(companyInfo);
            if (companyInfo.Logo?.Filename != null)
            {
                response.LogoUri = _uriComposer.ComposeLogoUri();
            }
            return response;
        }
    }

    public class GetCompanyInfoResponse
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Country { get; set; } = default!;
        public string State { get; set; } = default!;
        public string City { get; set; } = default!;
        public string Zip { get; set; } = default!;
        public string Line1 { get; set; } = default!;
        public string Line2 { get; set; } = default!;
        public string? LogoUri { get; set; } = default!;
    }

    public class GetCompanyInfoMapper : Profile
    {
        public GetCompanyInfoMapper()
        {
            CreateMap<CompanyInfo, GetCompanyInfoResponse>()
                .ForMember(dest => dest.LogoUri, opt => opt.Ignore());
        }
    }
}
