﻿using PetFoster.Domain.Shared;
using System.Linq.Expressions;

namespace PetFoster.Application.Extensions
{
    public static class QueryExtensions
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, 
            bool condition, 
            Expression<Func<T,bool>> predicate)
        {
            return condition ? source.Where(predicate) : source;
        }
    }
}
