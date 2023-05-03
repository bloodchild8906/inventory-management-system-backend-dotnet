# Base API Rest for Admin Dashboard Projects
Base API Rest to make admin dashboard projects made with DotNet Core 6 and uses [Vertical Slice Architecture](https://code-maze.com/vertical-slice-architecture-aspnet-core/) and [CQRS]( https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs) using EF Core with the UnitOfWork Pattern.

## Features
This api has the following features:
- Sign In (JWT AUTH)
- Permission Based Authorization
- Edit Profile Info
- List Users
- Update Users
- Delete Users
- Enable/Disable Users
- List Roles
- Create Roles
- Update Roles 
- Delete Roles
- Enable Disable Roles


## Example of how a command/query works
```c#
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
    
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Authorize(Policy = Permissions.Products.Create)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
        {
            var productId = await _mediator.Send(command);
            return Ok();
        }
     }
```

## Libraries used
- [FluentValidation](https://docs.fluentvalidation.net/en/latest/) 
- [MediatR](https://github.com/jbogard/MediatR)
- [EF Core](https://learn.microsoft.com/en-us/ef/core/)

## Credits
Inspired by
- [Minimal API Vertical Slice Architecture](https://github.com/isaacOjeda/MinimalApiArchitecture) by Issac Ojeda
- [CQRS and MediatR in ASP.NET Core](https://code-maze.com/cqrs-mediatr-in-aspnet-core/) by CodeMaze
- [CQRS Validation Pipeline with MediatR and FluentValidation](https://code-maze.com/cqrs-mediatr-fluentvalidation/) by CodeMaze

