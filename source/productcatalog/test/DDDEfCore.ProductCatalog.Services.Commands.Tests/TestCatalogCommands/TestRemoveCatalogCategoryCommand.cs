﻿using AutoFixture;
using DDDEfCore.Core.Common;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.Infrastructures.EfCore.Common.Repositories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Services.Commands.CatalogCommands.RemoveCatalogCategory;
using FluentValidation;
using FluentValidation.TestHelper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests.TestCatalogCommands
{
    public class TestRemoveCatalogCategoryCommand : UnitTestBase<Catalog>
    {
        private readonly Mock<DbContext> _mockDbContext;
        private readonly IRepository<Catalog> _repository;
        private readonly RemoveCatalogCategoryCommandValidator _validator;

        public TestRemoveCatalogCategoryCommand() : base()
        {
            this._mockDbContext = new Mock<DbContext>();
            this._repository = new DefaultRepositoryAsync<Catalog>(this._mockDbContext.Object);
            this._validator = new RemoveCatalogCategoryCommandValidator(this.MockRepositoryFactory.Object);
            this.MockRepositoryFactory
                .Setup(x => x.CreateRepository<Catalog>())
                .Returns(this._repository);
        }

        [Fact(DisplayName = "Remove CatalogCategory Successfully")]
        public async Task Remove_CatalogCategory_Successfully()
        {
            var catalog = Catalog.Create(this.Fixture.Create<string>());
            var catalogs = new List<Catalog> {catalog};

            this._mockDbContext
                .Setup(x => x.Set<Catalog>())
                .Returns(catalogs.AsQueryable().BuildMockDbSet().Object);

            var categoryId = IdentityFactory.Create<CategoryId>();
            var catalogCategory = catalog.AddCategory(categoryId, this.Fixture.Create<string>());

            var command = new RemoveCatalogCategoryCommand
            {
                CatalogId = catalog.CatalogId,
                CatalogCategoryId = catalogCategory.CatalogCategoryId
            };
                
            IRequestHandler<RemoveCatalogCategoryCommand> handler
                = new CommandHandler(this.MockRepositoryFactory.Object, this._validator);

            await handler.Handle(command, this.CancellationToken);

            this._mockDbContext.Verify(x => x.Update(catalog), Times.Once);
        }

        [Fact(DisplayName = "Invalid Command Should Throw Exception")]
        public async Task Invalid_Command_ShouldThrowException()
        {
            var command = new RemoveCatalogCategoryCommand
            {
                CatalogId = new CatalogId(Guid.Empty),
                CatalogCategoryId = new CatalogCategoryId(Guid.Empty)
            };

            IRequestHandler<RemoveCatalogCategoryCommand> handler = new CommandHandler(this.MockRepositoryFactory.Object, this._validator);

            await Should.ThrowAsync<ValidationException>(async () =>
                await handler.Handle(command, this.CancellationToken));

        }

        [Fact(DisplayName = "Validate: Guid.Empty Ids Should Be Invalid")]
        public void GuidEmpty_For_Ids_ShouldBeInvalid()
        {
            var command = new RemoveCatalogCategoryCommand
            {
                CatalogId = new CatalogId(Guid.Empty),
                CatalogCategoryId = new CatalogCategoryId(Guid.Empty)
            };

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldHaveValidationErrorFor(x => x.CatalogCategoryId);
        }

        [Fact(DisplayName = "Validate: Catalog Not Found Should Be Invalid")]
        public void Catalog_NotFound_ShouldBeInvalid()
        {
            var command = new RemoveCatalogCategoryCommand
            {
                CatalogId = new CatalogId(Guid.Empty),
                CatalogCategoryId = IdentityFactory.Create<CatalogCategoryId>()
            };

            var result = this._validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CatalogId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogCategoryId);
        }

        [Fact(DisplayName = "Validate: CatalogCategory Not Found Should Fail Validation")]
        public void CatalogCategory_NotFound_ShouldBeInvalid()
        {
            var catalog = Catalog.Create(this.Fixture.Create<string>());
            var catalogs = new List<Catalog> { catalog };
            this._mockDbContext.Setup(x => x.Set<Catalog>())
                .Returns(catalogs.AsQueryable().BuildMockDbSet().Object);

            var command = new RemoveCatalogCategoryCommand
            {
                CatalogId = catalog.CatalogId,
                CatalogCategoryId = new CatalogCategoryId(Guid.Empty)
            };

            var result = this._validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.CatalogCategoryId);
            result.ShouldNotHaveValidationErrorFor(x => x.CatalogId);
        }
    }
}
