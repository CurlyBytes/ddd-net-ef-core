﻿using DDDEfCore.Core.Common.Models;
using System;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Products
{
    public class ProductId : IdentityBase
    {
        #region Constructors

        private ProductId(Guid id) : base(id) { }

        #endregion

        public static explicit operator ProductId(Guid id) => id == Guid.Empty ? null : new ProductId(id);
    }
}
