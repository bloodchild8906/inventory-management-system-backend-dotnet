using MediatR;
using Persistence.GenericRepository.UnitOfWork;
using Products.Application.Domain;

namespace Products.Application.Features.Products.Queries
{

    public record GetProductsQuery() : IRequest<IEnumerable<Product>>;

    public class GetProducts : IRequestHandler<GetProductsQuery, IEnumerable<Product>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetProducts(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<IEnumerable<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.GetRepository<Product, int>().GetAllAsync();
        }
    }
}
