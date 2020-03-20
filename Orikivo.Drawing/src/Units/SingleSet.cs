using System;
using System.Linq.Expressions;

namespace Orikivo.Drawing
{
    /// <summary>
    /// Represents a function specified by a single-precision floating-point number.
    /// </summary>
    public struct SingleSet
    {
        // y = f(x) = value;
        private readonly Expression<Func<float, float>> _expressionSource;
        public readonly Func<float, float> Expression;

        private static Expression<Func<float, float>> Combine(Expression<Func<float, float>> a, 
            Expression<Func<float, float>> b, Operator operation)
        {
            ParameterExpression x = a.Parameters[0];
            if (ReferenceEquals(x, b))
            {
                return System.Linq.Expressions.Expression.Lambda<Func<float, float>>(CombineExact(a, b, operation));
            }

            return System.Linq.Expressions.Expression.Lambda<Func<float, float>>(CombineSimilar(a, b, operation), x);
        }

        private static BinaryExpression CombineExact(Expression<Func<float, float>> a, Expression<Func<float, float>> b, Operator operation)
            => operation switch
            {
                Operator.Add => System.Linq.Expressions.Expression.Add(a.Body, b.Body),
                Operator.Subtract => System.Linq.Expressions.Expression.Subtract(a.Body, b.Body),
                Operator.Multiply => System.Linq.Expressions.Expression.Multiply(a.Body, b.Body),
                Operator.Divide => System.Linq.Expressions.Expression.Divide(a.Body, b.Body),
                Operator.Power => System.Linq.Expressions.Expression.Power(a.Body, b.Body),
                Operator.Modulo => System.Linq.Expressions.Expression.Modulo(a.Body, b.Body)
            };

        private static BinaryExpression CombineSimilar(Expression<Func<float, float>> a, Expression<Func<float, float>> b, Operator combineMethod)
        {
            ParameterExpression x = a.Parameters[0];

            return combineMethod switch
            {
                Operator.Add => System.Linq.Expressions.Expression.Add(a.Body, System.Linq.Expressions.Expression.Invoke(b, x)),
                Operator.Subtract => System.Linq.Expressions.Expression.Subtract(a.Body, System.Linq.Expressions.Expression.Invoke(b, x)),
                Operator.Multiply => System.Linq.Expressions.Expression.Multiply(a.Body, System.Linq.Expressions.Expression.Invoke(b, x)),
                Operator.Divide => System.Linq.Expressions.Expression.Divide(a.Body, System.Linq.Expressions.Expression.Invoke(b, x)),
                Operator.Power => System.Linq.Expressions.Expression.Power(a.Body, System.Linq.Expressions.Expression.Invoke(b, x)),
                Operator.Modulo => System.Linq.Expressions.Expression.Modulo(a.Body, System.Linq.Expressions.Expression.Invoke(b, x))
            };
        }

        // merge two SingleSet functions together.
        public static SingleSet MergeOperations(SingleSet a, SingleSet b, Operator operation)
        {
            var expressionSource = Combine(a._expressionSource, b._expressionSource, operation);
            return new SingleSet(expressionSource);
        }

        public static float PointwiseAdd(SingleSet a, SingleSet b, float x)
            => a[x] + b[x];

        public static float PointwiseSubtract(SingleSet a, SingleSet b, float x)
            => a[x] - b[x];

        public static float PointwiseMultiply(SingleSet a, SingleSet b, float x)
            => a[x] * b[x];

        public static float PointwiseDivide(SingleSet a, SingleSet b, float x)
            => a[x] / b[x];

        public static float PointwisePower(SingleSet a, SingleSet b, float x)
            => MathF.Pow(a[x], b[x]);

        public static float PointwiseModulo(SingleSet a, SingleSet b, float x)
            => a[x] % b[x];


        public SingleSet(Expression<Func<float, float>> expressionSource)
        {
            _expressionSource = expressionSource;
            Expression = _expressionSource.Compile();
            X = 0;

        }

        /*
            y = 3x * 2
            x = 3y * 2
            x / 2 = 3y
            x / 2 / 3 = y
            x / 6 = y
             
             
             */

        public float X { get; set; }
        public float Y => Expression.Invoke(X);

        // determines if this expression is eligible to combine based on the given
        // expression.

        public Vector2 Point => new Vector2(X, Y);

        public float this[float x]
            => Expression.Invoke(x);

        public static SingleSet operator +(SingleSet a, SingleSet b)
            => MergeOperations(a, b, Operator.Add);

        public static SingleSet operator -(SingleSet a, SingleSet b)
            => MergeOperations(a, b, Operator.Subtract);

        public static SingleSet operator *(SingleSet a, SingleSet b)
            => MergeOperations(a, b, Operator.Multiply);

        public static SingleSet operator /(SingleSet a, SingleSet b)
            => MergeOperations(a, b, Operator.Divide);

        public static SingleSet operator %(SingleSet a, SingleSet b)
            => MergeOperations(a, b, Operator.Modulo);
    }
}
