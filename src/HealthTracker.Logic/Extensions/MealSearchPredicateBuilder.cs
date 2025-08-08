using System.Linq.Expressions;
using HealthTracker.Entities.Food;

namespace HealthTracker.Logic.Extensions
{
    public static class MealSearchPredicateBuilder
    {
        /// <summary>
        /// Given a set of meal search criteria, build an expression for retrieving matching entries
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static Expression<Func<Meal, bool>> BuildPredicate(this MealSearchCriteria criteria)
        {
            Expression<Func<Meal, bool>> predicate = x => true;

            // Add a clause for the  meal food source
            if (criteria.FoodSourceId != null)
            {
                predicate = predicate.And(m => m.FoodSourceId == criteria.FoodSourceId);
            }

            // Add a clause for the associated food item category
            if (criteria.FoodCategoryId != null)
            {
                predicate = predicate.And(m => m.MealFoodItems
                                    .Select(x => x.FoodItem.FoodCategoryId)
                                    .ToList()
                                    .Contains(criteria.FoodCategoryId.Value));
            }

            // Add a clause for the meal name substring search
            if (!string.IsNullOrEmpty(criteria.MealName))
            {
                predicate = predicate.And(m => m.Name.ToLower().Contains(criteria.MealName.ToLower()));
            }

            // Add a clause for the associated food item name substring search
            if (!string.IsNullOrEmpty(criteria.FoodItemName))
            {
                predicate = predicate.And(m =>
                                    m.MealFoodItems.Any(x =>
                                        x.FoodItem.Name != null &&
                                        x.FoodItem.Name.ToLower().Contains(criteria.FoodItemName.ToLower())));
            }

            return predicate;
        }

        /// <summary>
        /// Helper to AND together two expressions to return the combined predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1"></param>
        /// <param name="expr2"></param>
        /// <returns></returns>
        private static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var param = Expression.Parameter(typeof(T));

            var body = Expression.AndAlso(
                Expression.Invoke(expr1, param),
                Expression.Invoke(expr2, param));

            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }
}