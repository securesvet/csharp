using System;
using System.Linq.Expressions;

namespace Reflection.Differentiation
{
    public static class Algebra
    {
        public static Expression<Func<double, double>> Differentiate(Expression<Func<double, double>> function)
        {
            var expressionBody = DifferentiateExpression(function.Body);
            return Expression.Lambda<Func<double, double>>(expressionBody, function.Parameters);
        }

        private static Expression DifferentiateExpression(Expression expression)
        {
            switch (expression)
            {
                case ConstantExpression _:
                    return Expression.Constant(0.0);
                case ParameterExpression _:
                    return Expression.Constant(1.0);
                case BinaryExpression binaryExpression:
                    return DifferentiateBinaryExpression(binaryExpression);
                case MethodCallExpression methodCallExpression:
                    return DifferentiateMethodCallExpression(methodCallExpression);
                default:
                    throw new ArgumentException("String containing \"ToString\"");
            }
        }

        private static Expression DifferentiateBinaryExpression(BinaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Add:
                    return Expression.Add(DifferentiateExpression(expression.Left), DifferentiateExpression(expression.Right));
                case ExpressionType.Subtract:
                    return Expression.Subtract(DifferentiateExpression(expression.Left), DifferentiateExpression(expression.Right));
                case ExpressionType.Multiply:
                    var left = Expression.Multiply(DifferentiateExpression(expression.Left), expression.Right);
                    var right = Expression.Multiply(expression.Left, DifferentiateExpression(expression.Right));
                    return Expression.Add(left, right);
                default:
                    throw new ArgumentException($"Unsupported operator: {expression.NodeType}");
            }
        }

        private static Expression DifferentiateMethodCallExpression(MethodCallExpression expression)
        {
            switch (expression.Method.Name)
            {
                case "Sin":
                    var arg = expression.Arguments[0];
                    var cos = Expression.Call(typeof(Math).GetMethod("Cos", new[] 
                        { typeof(double) }) ?? throw new InvalidOperationException(), arg);
                    return Expression.Multiply(DifferentiateExpression(arg), cos);
                case "Cos":
                    arg = expression.Arguments[0];
                    var sin = Expression.Call(typeof(Math).GetMethod("Sin", new[] 
                        { typeof(double) }) ?? throw new InvalidOperationException(), arg);
                    return Expression.Negate(Expression.Multiply(DifferentiateExpression(arg), sin));
                default:
                    throw new ArgumentException($"Unsupported method call: {expression.Method.Name}");
            }
        }
    }
}