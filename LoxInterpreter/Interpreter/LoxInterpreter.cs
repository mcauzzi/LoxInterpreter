using System.Globalization;
using LoxInterpreter.Parser;

namespace LoxInterpreter.Interpreter;

public class LoxInterpreter : IVisitor<object>
{
    public void Interpret(Expr expression)
    {
        try
        {
            var result = Evaluate(expression);
            Console.WriteLine(Stringify(result));
        }
        catch (RuntimeLoxException e)
        {
            Console.WriteLine($"Runtime error at {e.Token.Line}: {e.Message}");
        }
    }

    private string Stringify(object? result)
    {
        if (result is null) return "nil";
        if (result is double d)
        {
            var text = d.ToString(CultureInfo.InvariantCulture);
            return text.Contains('.') ? text : $"{text}.0";
        }

        return result.ToString() ?? "nil";
    }

    public object VisitBinary(Binary binary)
    {
        var left  = Evaluate(binary.Left);
        var right = Evaluate(binary.Right);

        return binary.Op.Type switch
               {
                   TokenType.MINUS => CheckNumberOperand(binary.Op, left) - CheckNumberOperand(binary.Op, right),
                   TokenType.PLUS when left is double l && right is double r => l + r,
                   TokenType.PLUS when left is string sl && right is string sr => sl[..^1] + sr[1..],
                   TokenType.PLUS => throw new RuntimeLoxException(binary.Op, "Operands must be numbers or strings."),
                   TokenType.STAR => CheckNumberOperand(binary.Op, left) * CheckNumberOperand(binary.Op, right),
                   TokenType.SLASH => CheckNumberOperand(binary.Op, left) / CheckNumberOperand(binary.Op, right),
                   TokenType.GREATER => CheckNumberOperand(binary.Op, left) > CheckNumberOperand(binary.Op, right),
                   TokenType.GREATER_EQUAL => CheckNumberOperand(binary.Op, left)
                                           >= CheckNumberOperand(binary.Op, right),
                   TokenType.LESS       => CheckNumberOperand(binary.Op, left) < CheckNumberOperand(binary.Op,  right),
                   TokenType.LESS_EQUAL => CheckNumberOperand(binary.Op, left) <= CheckNumberOperand(binary.Op, right),
                   TokenType.BANG_EQUAL => !IsEqual(left, right),
                   TokenType.EQUAL      => IsEqual(left, right),
                   _                    => throw new LoxException(0, $"Unknown binary operator: {binary.Op.Type}")
               };
    }

    private double CheckNumberOperand(Token op, object left)
    {
        if (left is double d)
        {
            return d;
        }

        throw new RuntimeLoxException(op, $"Operand must be a number, got {left.GetType().Name} instead.");
    }


    private bool IsEqual(object? left, object? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;

        return left.Equals(right);
    }

    public object VisitGrouping(Grouping grouping)
    {
        return Evaluate(grouping.Expression);
    }

    private object Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }

    public object VisitLiteral(Literal literal)
    {
        return literal.Value;
    }

    public object VisitUnary(Unary unary)
    {
        var right = Evaluate(unary.Right);
        return unary.Op.Type switch
               {
                   TokenType.MINUS => -(double)CheckNumberOperand(unary.Op, right),
                   TokenType.BANG  => !IsTruthy(right),
                   _               => throw new LoxException(0, $"Unknown unary operator: {unary.Op.Type}")
               };
    }

    private bool IsTruthy(object? right) =>
        right switch
        {
            null   => false,
            bool b => b,
            _      => true
        };
}

internal class RuntimeLoxException : Exception
{
    public RuntimeLoxException(Token token, string message) : base(message)
    {
        Token = token;
    }

    public Token Token { get; set; }
}