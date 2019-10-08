﻿using DDDEfCore.Core.Common.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DDDEfCore.Core.Common
{
    public interface IRepository<TAggregate> : IDisposable where TAggregate : AggregateRoot
    {
        IQueryable<TAggregate> AsQueryable();
        Task<TAggregate> FindOneAsync(Expression<Func<TAggregate, bool>> predicate);
        Task AddAsync(TAggregate aggregate);
        Task UpdateAsync(TAggregate aggregate);
        Task RemoveAsync(TAggregate aggregate);
    }
}
