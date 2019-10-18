﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DDDEfCore.Core.Common;
using DDDEfCore.Infrastructures.EfCore.Common.Extensions;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.RemoveCatalogCategory
{
    public class RemoveCatalogCategoryCommandValidator : AbstractValidator<RemoveCatalogCategoryCommand>
    {
        public RemoveCatalogCategoryCommandValidator(IRepositoryFactory repositoryFactory)
        {
            RuleFor(x => x.CatalogId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .Must(x => x.IsNotEmpty);

            RuleFor(x => x.CatalogCategoryId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .Must(x => x.IsNotEmpty);

            When(x => x.CatalogId.IsNotEmpty && x.CatalogCategoryId.IsNotEmpty, () =>
            {
                RuleFor(x => x).Custom((command, context) =>
                {
                    var repository = repositoryFactory.CreateRepository<Catalog>();
                    var catalog = repository
                        .FindOneWithIncludeAsync(x => x.CatalogId == command.CatalogId,
                            x => x.Include(c => c.Categories)).GetAwaiter().GetResult();
                    if (catalog == null)
                    {
                        context.AddFailure(nameof(command.CatalogId), $"Could not found Catalog#{command.CatalogId}");
                    }
                    else
                    {
                        if (catalog.Categories.All(x => x.CatalogCategoryId != command.CatalogCategoryId))
                        {
                            context.AddFailure(nameof(command.CatalogCategoryId), $"Could not found CatalogCategory#{command.CatalogCategoryId} in Catalog#{command.CatalogId}");
                        }
                    }
                });
            });
        }
    }
}
