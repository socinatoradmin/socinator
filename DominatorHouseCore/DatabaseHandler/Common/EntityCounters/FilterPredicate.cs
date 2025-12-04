#region

using System;
using System.Linq.Expressions;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.DatabaseHandler.Common.EntityCounters
{
    public interface IFilterPredicate<TSource> where TSource : class, new()
    {
        Expression<Func<TSource, bool>> GetFilterExpression(object filterInput);
    }

    public abstract class FilterPredicate<TSource, TInput, TValue> : IFilterPredicate<TSource>
        where TSource : class, new()
    {
        private readonly Expression<Func<TSource, TValue>> _filterExpression;
        private readonly Func<TInput, TValue> _valueConversionFunc;

        protected FilterPredicate(Expression<Func<TSource, TValue>> filterExpression,
            Func<TInput, TValue> valueConversionFunc)
        {
            _filterExpression = filterExpression;
            _valueConversionFunc = valueConversionFunc;
        }

        public Expression<Func<TSource, bool>> GetFilterExpression(object filterInput)
        {
            var value = _valueConversionFunc((TInput) filterInput);
            return CreateExpression(_filterExpression, value);
        }

        protected virtual Expression<Func<TSource, bool>> CreateExpression(
            Expression<Func<TSource, TValue>> filterExpression, TValue value)
        {
            return Expression.Lambda<Func<TSource, bool>>(
                Expression.GreaterThanOrEqual(
                    filterExpression.Body,
                    Expression.Constant(value, typeof(TValue))
                ), filterExpression.Parameters);
        }
    }

    public class DateFilterPredicate<TSource> : FilterPredicate<TSource, DateTime, DateTime>
        where TSource : class, new()
    {
        public DateFilterPredicate(Expression<Func<TSource, DateTime>> filterExpression)
            : base(filterExpression, a => a)
        {
        }
    }

    public class DateEpochFilterPredicate<TSource> : FilterPredicate<TSource, DateTime, int>
        where TSource : class, new()
    {
        public DateEpochFilterPredicate(Expression<Func<TSource, int>> filterExpression)
            : base(filterExpression, a => a.ConvertToEpoch())
        {
        }
    }

    public class ActivityTypeFilterPredicate<TSource, TValue> : FilterPredicate<TSource, ActivityType, TValue>
        where TSource : class, new()
    {
        public ActivityTypeFilterPredicate(Expression<Func<TSource, TValue>> filterExpression,
            Func<ActivityType, TValue> valueConversionFunc)
            : base(filterExpression, valueConversionFunc)
        {
        }

        protected override Expression<Func<TSource, bool>> CreateExpression(
            Expression<Func<TSource, TValue>> filterExpression, TValue value)
        {
            return Expression.Lambda<Func<TSource, bool>>(
                Expression.Equal(
                    filterExpression.Body,
                    Expression.Constant(value, typeof(TValue))
                ), filterExpression.Parameters);
        }
    }

    public class ActivityTypeAsStringFilterPredicate<TSource> : ActivityTypeFilterPredicate<TSource, string>
        where TSource : class, new()
    {
        public ActivityTypeAsStringFilterPredicate(Expression<Func<TSource, string>> filterExpression)
            : base(filterExpression, a => a.ToString())
        {
        }
    }

    public class ActivityTypeFilterPredicate<TSource> : ActivityTypeFilterPredicate<TSource, ActivityType>
        where TSource : class, new()
    {
        public ActivityTypeFilterPredicate(Expression<Func<TSource, ActivityType>> filterExpression)
            : base(filterExpression, a => a)
        {
        }
    }
}