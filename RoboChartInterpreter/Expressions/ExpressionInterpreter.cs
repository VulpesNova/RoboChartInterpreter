

using System.Collections;
using System.Reflection.Metadata.Ecma335;
using Antlr4.Runtime;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace RoboChartInterpreter.Expressions
{
    public class ExpressionInterpreter : AbstractParseTreeVisitor<object>, IRoboChartExpressionVisitor<object>
    {
        public Dictionary<string, object> variables = new();
        public Dictionary<string, int> clocks = new();

        public object Interpret(string input)
        {
            ICharStream inputStream = CharStreams.fromString(input);
            RoboChartExpressionLexer RoboChartExpressionLexer = new(inputStream);
            CommonTokenStream commonTokenStream = new(RoboChartExpressionLexer);
            RoboChartExpressionParser RoboChartExpressionParser = new(commonTokenStream);

            RoboChartExpressionParser.ExpressionContext expressionContext = RoboChartExpressionParser.expression();

            return Visit(expressionContext);
        }

        public object VisitBinaryExpr([NotNull] RoboChartExpressionParser.BinaryExprContext context)
        {
            object left = context.expression(0).Accept(this);
            object right = context.expression(1).Accept(this);
            switch (context.op.Text)
            {
                case "==":
                    return Equal(left, right);
                case "!=":
                    return !Equal(left, right);
                case ">":
                    return Greater(left, right);
                case ">=":
                    return Greater(left, right) | Equal(left, right);
                case "<":
                    return Greater(right, left);
                case "<=":
                    return Greater(right, left) | Equal(left, right);
                case "*":
                case "/":
                case "+":
                case "-":
                    return Arithmetic(left, right, context.op.Text);
                default:
                    throw new InvalidOperationRCException($"Operation {context.op.Text} does not exist.");
            }
        }

        object Arithmetic(object left, object right, string op)
        {
            if (left.GetType() == typeof(int) && right.GetType() == typeof(int))
            {
                switch (op)
                {
                    case "*":
                        return (int)left * (int)right;
                    case "/":
                        return (int)left / (int)right;
                    case "+":
                        return (int)left + (int)right;
                    case "-":
                        return (int)left - (int)right;
                }
            }
            if ((left.GetType() == typeof(double) && right.GetType() == typeof(int)) ||
                (left.GetType() == typeof(int) && right.GetType() == typeof(double)) ||
                (left.GetType() == typeof(double) && right.GetType() == typeof(double)))
            {
                switch (op)
                {
                    case "*":
                        return Convert.ToDouble(left) * Convert.ToDouble(right);
                    case "/":
                        return Convert.ToDouble(left) / Convert.ToDouble(right);
                    case "+":
                        return Convert.ToDouble(left) + Convert.ToDouble(right);
                    case "-":
                        return Convert.ToDouble(left) - Convert.ToDouble(right);
                }
            }

            throw new InvalidTypeRCException($"Operation {op} not valid for types '{left.GetType()}' and '{right.GetType()}'.");
        }

        bool Equal(object left, object right)
        {
            if (left.GetType() == typeof(double) && right.GetType() == typeof(int) ||
                left.GetType() == typeof(int) && right.GetType() == typeof(double))
                return Math.Abs(Convert.ToDouble(left) - Convert.ToDouble(right)) < 0.000001;

            if (left.GetType() != right.GetType()) return false;
            if (left.GetType() == typeof(string)) return (string)left == (string)right;
            if (left.GetType() == typeof(int)) return (int)left == (int)right;
            if (left.GetType() == typeof(double)) return Math.Abs((double)left - (double)right) < 0.000001;
            return false;
        }

        bool Greater(object left, object right)
        {
            if (left.GetType() == typeof(double) && right.GetType() == typeof(int) ||
                left.GetType() == typeof(int) && right.GetType() == typeof(double))
                return Convert.ToDouble(left) > Convert.ToDouble(right);

            if (left.GetType() != right.GetType()) return false;
            if (left.GetType() == typeof(string)) return false;
            if (left.GetType() == typeof(int)) return (int)left > (int)right;
            if (left.GetType() == typeof(double)) return (double)left > (double)right;
            return false;
        }

        public object VisitLiteralExpr([NotNull] RoboChartExpressionParser.LiteralExprContext context)
        {
            return context.literal().Accept(this);
        }

        public object VisitVariableExpr([NotNull] RoboChartExpressionParser.VariableExprContext context)
        {
            return variables[context.NAME().GetText()];
        }

        public object VisitIntLiteral([NotNull] RoboChartExpressionParser.IntLiteralContext context)
        {
            return int.Parse(context.INT().Symbol.Text);
        }

        public object VisitStringLiteral([NotNull] RoboChartExpressionParser.StringLiteralContext context)
        {
            return context.STRING().Symbol.Text[1..^1];
        }

        public object VisitRealLiteral([NotNull] RoboChartExpressionParser.RealLiteralContext context)
        {
            return double.Parse(context.REAL().Symbol.Text);
        }

        public object VisitParenthisedExpr([NotNull] RoboChartExpressionParser.ParenthisedExprContext context)
        {
            return context.expression().Accept(this);
        }

        public object VisitClockExpr([NotNull] RoboChartExpressionParser.ClockExprContext context)
        {
            Console.WriteLine($"{context.NAME().Symbol.Text}: {clocks[context.NAME().Symbol.Text]}");
            return clocks[context.NAME().Symbol.Text];
        }
    }
}
