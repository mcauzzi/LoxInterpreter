namespace LoxInterpreter.Parser;

public class AstPrinter:IVisitor<string>
{
    public string Print(Expr expr)
    {
        return expr.Accept(this);
    }
    public string VisitBinary(Binary binary)
    {
        return Parenthesize(binary.Op.Lexeme,binary.Left, binary.Right);
    }

    public string VisitGrouping(Grouping grouping)
    {
        return Parenthesize("group", grouping.Expression);
    }

    private string Parenthesize(string left,params Expr[] expressions)
    {
        var builder = new System.Text.StringBuilder();
        builder.Append('(').Append(left);
        foreach (var expr in expressions)
        {
            builder.Append(' ').Append(expr.Accept(this));
        }
        builder.Append(')');
        return builder.ToString();
    }

    public string VisitLiteral(Literal literal)
    {
        if (literal.Value == null)
        {
            return "nil";
        }
        return literal.Value.ToString();
    }

    public string VisitUnary(Unary unary)
    {
        return Parenthesize(unary.Op.Lexeme, unary.Right);
    }
}