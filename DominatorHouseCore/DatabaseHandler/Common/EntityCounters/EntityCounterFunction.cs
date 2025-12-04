#region

using System;
using System.Linq.Expressions;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.DatabaseHandler.Common.EntityCounters
{
    // ReSharper disable once UnusedTypeParameter
    public interface IEntityCounterFunction<T> where T : class, new()
    {
        EntityCounter GetCounter(string accountId, SocialNetworks networks, ActivityType? activityType);
    }

    public class EntityCounterFunction<T> : IEntityCounterFunction<T> where T : class, new()
    {
        private readonly IFilterPredicate<T> _datePredicate;
        private readonly IFilterPredicate<T> _activityTypePredicate;


        public EntityCounterFunction(IFilterPredicate<T> datePredicate)
        {
            _datePredicate = datePredicate;
        }

        public EntityCounterFunction(IFilterPredicate<T> datePredicate, IFilterPredicate<T> activityTypePredicate) :
            this(datePredicate)
        {
            _activityTypePredicate = activityTypePredicate;
        }

        public EntityCounter GetCounter(string accountId, SocialNetworks networks, ActivityType? activityType)
        {
            var dbOperations = InstanceProvider.ResolveAccountDbOperations(accountId, networks);
            var dateProvider = InstanceProvider.GetInstance<IDateProvider>();
            var now = dateProvider.Now();
            var startOfWeek = now.GetStartOfWeek();
            var today = now.Date;

            var lastHour = today.AddHours(now.Hour);
            var countLastWeek =
                dbOperations.Count(
                    BuildExpressionFor(_datePredicate, _activityTypePredicate, startOfWeek, activityType));
            var countLastDay =
                dbOperations.Count(BuildExpressionFor(_datePredicate, _activityTypePredicate, today, activityType));
            var countLastHour =
                dbOperations.Count(BuildExpressionFor(_datePredicate, _activityTypePredicate, lastHour, activityType));

            return new EntityCounter(countLastWeek, countLastDay, countLastHour);
        }

        private Expression<Func<T, bool>> BuildExpressionFor(IFilterPredicate<T> dateFilter,
            IFilterPredicate<T> activityFilter, DateTime filter, ActivityType? activityType)
        {
            var dateExpression = dateFilter.GetFilterExpression(filter);
            if (activityFilter == null) return dateExpression;

            if (!activityType.HasValue)
                throw new InvalidOperationException(
                    $"Filter by activity type is set, but no activity type provided. EntityCounter:{GetType()}");

            var activityExpression = activityFilter.GetFilterExpression(activityType.Value);
            var paramExpr = Expression.Parameter(typeof(T));
            var exprBody = Expression.AndAlso(dateExpression.Body, activityExpression.Body);
            exprBody = (BinaryExpression) new ParameterReplacer(paramExpr).Visit(exprBody);
            return Expression.Lambda<Func<T, bool>>(exprBody, paramExpr);
        }

        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _parameter;

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return base.VisitParameter(_parameter);
            }

            internal ParameterReplacer(ParameterExpression parameter)
            {
                _parameter = parameter;
            }
        }
    }
}