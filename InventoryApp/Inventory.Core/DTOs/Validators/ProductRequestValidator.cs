using Inventory.Core.DTOs.Requests;
using FluentValidation;
using Inventory.Core.Entities;

namespace Inventory.Core.DTOs.Validators;


public class ProductrequestValidator : AbstractValidator<ProductRequestDTO>
{
    public ProductrequestValidator()
    {
        RuleFor(product => product.Name)
        .NotEmpty().WithMessage("Product name cannot be Empty")
        .MinimumLength(3).WithMessage("Name must be atleast 3 characters")
        .MaximumLength(50).WithMessage("Name cannot exceed 50 characters");

        RuleFor(product => product.Price)
        .GreaterThan(0).WithMessage("Price must be greater thatn zero");

        RuleFor(product => product.Name)
        .Must(name => !name.Contains("Inventory"))
        .WithMessage("Name Contains Illegal Words");

        RuleFor(x=>x.Category)
        .IsEnumName(typeof(ProductCategory), caseSensitive: false)
        .WithMessage("Invalid Category");
    }
}