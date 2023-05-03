using FluentValidation;
using Inventory.Application.Domain;
using MediatR;
using Persistence.GenericRepository.UnitOfWork;

namespace Inventory.Application.Features.Products.Commands
{
    public record CreateProductCommand(string Name, decimal Price) : IRequest<int>;

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotNull()
                .NotEmpty();

            RuleFor(p => p.Price)
                .NotNull()
                .NotEmpty();
        }
    }

    public class CreateProduct : IRequestHandler<CreateProductCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateProduct(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var productRepository = _unitOfWork.GetRepository<Product, int>();
            var newProduct = new Product(request.Name, request.Price);
            await productRepository.CreateAndSaveAsync(newProduct);
            return newProduct.Id;
        }
    }
}
