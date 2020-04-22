using Entities.Concrete;
using Entities.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.ValidationRules.FluentValidation
{
    public class FileForUploadDtoValidator:AbstractValidator<FileForUploadDto>
    {
        public FileForUploadDtoValidator()
        {
            RuleFor(p => p.FileName).NotEmpty().WithMessage("Dosya ismi boş geçilemez") ;
            RuleFor(p => p.File).NotEmpty().WithMessage("Dosya boş geçilemez"); ;
        }
    }
}
