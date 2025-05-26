using LoxInterpreter;
using LoxInterpreter.Parser;

namespace LoxTests;

public class UnitTest1
{
    [Theory]
    [MemberData(nameof(AstPrinterTestData))]
    public void AstPrinterTest(Expr expr, string expected)
    {
        // Arrange


        // Act
        var result = new AstPrinter().Print(expr);

        // Assert
        Assert.Equal(expected, result);
    }

    public static IEnumerable<object[]> AstPrinterTestData()
    {
        yield return
        [
            new Binary
            {
                Op    = new Token(TokenType.PLUS, "+", null, 1),
                Left  = new Literal { Value = 1 },
                Right = new Literal { Value = 2 }
            },
            "(+ 1 2)"
        ];
        yield return
        [
            new Binary
            {
                Op    = new Token(TokenType.STAR, "*", null, 1),
                Left  = new Literal { Value = 3 },
                Right = new Literal { Value = 4 }
            },
            "(* 3 4)"
        ];
        yield return
        [
            new Unary
            {
                Op    = new Token(TokenType.MINUS, "-", null, 1),
                Right = new Literal { Value = 5 }
            },
            "(- 5)"
        ];
        yield return
        [
            new Binary()
            {
                Op = new Token(TokenType.STAR, "*", null, 1),
                Left = new Unary()
                       {
                           Op    = new Token(TokenType.MINUS, "-", null, 1),
                           Right = new Literal { Value = 123 }
                       },
                Right = new Grouping()
                        {
                            Expression = new Literal() { Value = 45.67 }
                        }
            },
            "(* (- 123) (group 45.67))"
        ];
    }
}