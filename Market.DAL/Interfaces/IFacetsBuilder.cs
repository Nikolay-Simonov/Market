using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Market.DAL.Enums;

namespace Market.DAL.Interfaces
{
    public interface IFacetsBuilder<TEntity> where TEntity : class, new()
    {
        IReadOnlyDictionary<string, object> Parameters { get; }

        IEntityDescription<TEntity> EntityDescription { get; }

        void Clear();

        string GetParamName();

        IFacetsBuilder<TEntity> AppendText(string text);

        IFacetsBuilder<TEntity> AddParameter(string paramName, object value);

        IFacetsBuilder<TEntity> Condition(Expression<Func<TEntity, string>> property, string value, Op op = Op.Equal);

        IFacetsBuilder<TEntity> Condition<TProperty>(Expression<Func<TEntity, TProperty>> property,
            TProperty? value, Op op = Op.Equal) where TProperty : struct;

        IFacetsBuilder<TEntity> ConditionIn(Expression<Func<TEntity, string>> property, ISet<string> values);

        IFacetsBuilder<TEntity> ConditionIn<TProperty>(Expression<Func<TEntity, TProperty>> property,
            ISet<TProperty> values) where TProperty : struct;

        IFacetsBuilder<TEntity> Or(Expression<Func<TEntity, string>> property, string value, Op op = Op.Equal);

        IFacetsBuilder<TEntity> Or<TProperty>(Expression<Func<TEntity, TProperty>> property, TProperty? value,
            Op op = Op.Equal) where TProperty : struct;

        IFacetsBuilder<TEntity> And(Expression<Func<TEntity, string>> property, string value, Op op = Op.Equal);

        IFacetsBuilder<TEntity> And<TProperty>(Expression<Func<TEntity, TProperty>> property, TProperty? value,
            Op op = Op.Equal) where TProperty : struct;

        IFacetsBuilder<TEntity> OrIn(Expression<Func<TEntity, string>> property, ISet<string> values);

        IFacetsBuilder<TEntity> OrIn<TProperty>(Expression<Func<TEntity, TProperty>> property, ISet<TProperty> values)
            where TProperty : struct;

        IFacetsBuilder<TEntity> AndIn(Expression<Func<TEntity, string>> property, ISet<string> values);

        IFacetsBuilder<TEntity> AndIn<TProperty>(Expression<Func<TEntity, TProperty>> property,
            ISet<TProperty> values) where TProperty : struct;
    }
}