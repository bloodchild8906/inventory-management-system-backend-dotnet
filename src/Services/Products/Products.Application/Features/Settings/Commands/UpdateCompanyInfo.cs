using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Products.Application.Domain.Settings;
using Products.Application.Infrastructure.Persistence;
using Products.Application.Interfaces.Service;
using Products.Application.Shared.Constants;

namespace Products.Application.Features.Settings.Commands
{
    public class UpdateCompanyInfoCommand : IRequest<string>
    {
        public string Name { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Country { get; set; } = default!;
        public string State { get; set; } = default!;
        public string City { get; set; } = default!;
        public string Zip { get; set; } = default!;
        public string Line1 { get; set; } = default!;
        public string? Line2 { get; set; } = default!;
        public IFormFile? Photo { get; set; } = default!;
    }

    public class UpdateCompanyInfoValidator : AbstractValidator<UpdateCompanyInfoCommand>
    {
        public UpdateCompanyInfoValidator() 
        {
            RuleFor(c => c.Name).NotEmpty().MaximumLength(50);
            RuleFor(c => c.Phone).NotEmpty().MaximumLength(20);
            RuleFor(c => c.Country).NotEmpty().MaximumLength(50);
            RuleFor(c => c.State).NotEmpty().MaximumLength(50);
            RuleFor(c => c.City).NotEmpty().MaximumLength(50);
            RuleFor(c => c.Line1).NotEmpty().MaximumLength(50);
            RuleFor(c => c.Line2).MaximumLength(50);
            RuleFor(c => c.Photo).Must(m => ValidLength(m)).WithMessage("Maximum Photo size exceeded");
            RuleFor(c => c.Photo).Must(m => ValidContentType(m)).WithMessage("Only png & jpeg images are allowed.");
        }

        private HashSet<string> ValidMimeTypes()
        {
            return new HashSet<string>
            {
                "image/jpeg",
                "image/png"
            };
        }

        private bool ValidContentType(IFormFile? photo)
        {
            if (photo == null)
            {
                return true;
            }
            return ValidMimeTypes().Contains(photo.ContentType);
        }

        private bool ValidLength(IFormFile? photo)
        {
            if (photo == null)
                return true;
            // 2MB
            return photo.Length <= 2097152;
        }
    }

    public class UpdateCompanyInfo : IRequestHandler<UpdateCompanyInfoCommand, string>
    {
        private readonly ApplicationDbContext _context;
        private readonly IPathComposer _pathComposer;

        public UpdateCompanyInfo(ApplicationDbContext context, IPathComposer pathComposer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _pathComposer = pathComposer ?? throw new ArgumentNullException(nameof(pathComposer));
        }

        private void UpdateInfo(UpdateCompanyInfoCommand command, CompanyInfo info)
        {
            info.Name = command.Name;
            info.Phone = command.Phone;
            info.Country = command.Country;
            info.State = command.State;
            info.City = command.City;
            info.Zip = command.Zip;
            info.Line1 = command.Line1;
            if (command.Line2 != null)
            {
                info.Line2 = command.Line2;
            }
        }

        public async Task<string> Handle(UpdateCompanyInfoCommand request, CancellationToken cancellationToken)
        {
            var companyInfo = await _context.CompanyInfo.Include(p => p.Logo).FirstOrDefaultAsync();
            UpdateInfo(request, companyInfo!);

            if (request.Photo != null)
            {
                var logo = await CreateLogo(request.Photo);
                companyInfo!.Logo!.Filename = logo.Filename;
                companyInfo!.Logo!.FileType = logo.FileType;
            }

            _context.Entry(companyInfo!).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return companyInfo!.Id;
        }

        public async Task<CompanyInfoLogo> CreateLogo(IFormFile photo)
        {
            var path = _pathComposer.ComposeCompanyLogoDirectory();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fileName = Path.GetFileName(photo.FileName);
            var filePath = _pathComposer.ComposeCompanyLogoPath(fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            return new CompanyInfoLogo
            {
                CompanyInfoId = SettingsConstants.CompanyInfoId,
                Filename = fileName,
                FileType = photo.ContentType
            };
        }
    }
}
