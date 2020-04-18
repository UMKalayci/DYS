using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities.Concrete;
using FluentValidation;

namespace Business.ValidationRules.FluentValidation
{
    public class ProductValidator:AbstractValidator<User>
    {
        public ProductValidator()
        {
            RuleFor(p => p.Email).NotEmpty();
            //RuleFor(p => p.ProductName).Length(2, 30);
            //RuleFor(p => p.UnitPrice).NotEmpty();
            //RuleFor(p => p.UnitPrice).GreaterThanOrEqualTo(1);
            //RuleFor(p => p.UnitPrice).GreaterThanOrEqualTo(10).When(p => p.CategoryId == 1);
            //RuleFor(p => p.ProductName).Must(StartWithWithA);
        }

        //private bool StartWithWithA(string arg)
        //{
        //    return arg.StartsWith("A");
        //}
    }
}
