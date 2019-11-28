﻿using AutoFixture;
using DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs;
using System.Threading.Tasks;
using DDDEfCore.ProductCatalog.Core.DomainModels.Categories;
using DDDEfCore.ProductCatalog.Core.DomainModels.Products;
using Xunit;

namespace DDDEfCore.ProductCatalog.WebApi.Tests.TestCatalogsController
{
    [Collection(nameof(SharedFixture))]
    public class TestCatalogsControllerFixture : SharedFixture
    {
        public string BaseUrl => @"api/catalogs";
        public Catalog Catalog { get; private set; }
        public Category Category { get; private set; }
        public Product Product { get; private set; }
        public CatalogCategory CatalogCategory { get; private set; }

        #region Overrides of SharedFixture

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            this.Category = Category.Create(this.AutoFixture.Create<string>());
            await this.SeedingData(this.Category);

            this.Product = Product.Create(this.AutoFixture.Create<string>());
            await this.SeedingData(this.Product);

            this.Catalog = Catalog.Create(this.AutoFixture.Create<string>());
            this.CatalogCategory = this.Catalog.AddCategory(this.Category.CategoryId, this.Category.DisplayName);
            this.CatalogCategory.CreateCatalogProduct(this.Product.ProductId, this.Product.Name);
            await this.SeedingData(this.Catalog);
        }

        #endregion
    }
}
