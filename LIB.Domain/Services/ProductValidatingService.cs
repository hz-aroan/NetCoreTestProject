using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.SQL.Main;
using LIB.Domain.Exceptions;
using LIB.Domain.Services.DTO;
using Product = Infrastructure.SQL.Main.Product;

namespace LIB.Domain.Services;

public class ProductValidatingService
{
    private const Decimal MAX_FEE_AMOUNT = 100;



    public void ValidateNewOne(Product product, IList<Currency> availableCurrencies)
    {
        if (string.IsNullOrEmpty(product.Name))
            throw new DomainException("The product name should not be empty!");

        if (product.FeeAmount.HasValue && String.IsNullOrEmpty(product.FeeCurrency))
            throw new DomainException("When the fee currency amount is set - the currency should also be set!");

        if (product.FeeAmount.HasValue && product.FeeAmount < 0)
            throw new DomainException($"Invalid fee amount '{product.FeeAmount}' - should be a positive value!");

        if (product.FeeAmount.HasValue && product.FeeAmount > MAX_FEE_AMOUNT)
            throw new DomainException($"Invalid fee amount '{product.FeeAmount}' - should not be greater the the maximum allowed value {MAX_FEE_AMOUNT}!");

        if (string.IsNullOrEmpty(product.FeeCurrency) && availableCurrencies.Any(p => p.Id == product.FeeCurrency) == false)
            throw new DomainException($"The fee currency {product.FeeCurrency} is unknown!");
    }
}