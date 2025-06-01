using System.Text.RegularExpressions;

namespace LoxInterpreter.Parser;

public class LoxParser
{
    private List<Token> _tokens;
    
    private int _current;

    public LoxParser(List<Token> tokens)
    {
        _tokens= tokens;
    }

    public Expr? Parse()
    {
        try
        {
            return Expression();
        }
        catch (LoxException e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }
    private Expr Expression()
    {
        return Equality();
    }

    private Expr Equality()
    {
        Expr expr = Comparison();

        while (Match(TokenType.EQUAL_EQUAL, TokenType.BANG_EQUAL))
        {
            Token op    =Previous();
            Expr  right = Comparison();
            expr = new Binary()
                   {
                       Left = expr,Op=op,Right = right
                       
                   };
        }
        
        return expr;
    }

    private Expr Comparison()
    {
        Expr expr = Term();
        while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
        {
            Token op    = Previous();
            Expr  right = Term();
            expr = new Binary()
                   {
                       Left = expr,Op=op,Right = right
                   };
        }

        return expr;
    }

    private Expr Term()
    {
        Expr expr = Factor();
        while (Match(TokenType.MINUS, TokenType.PLUS))
        {
            Token op    = Previous();
            Expr  right = Factor();
            expr = new Binary()
                   {
                       Left = expr,Op=op,Right = right
                   };
        }

        return expr;
    }

    private Expr Factor()
    {
        Expr expr= Unary();
        while (Match(TokenType.SLASH, TokenType.STAR))
        {
            var op= Previous();
            var right = Unary();
            expr = new Binary()
                   {
                       Left = expr,Op=op,Right = right
                   };
        }

        return expr;
    }
    
    private Expr Unary()
    {
        if (Match(TokenType.BANG, TokenType.MINUS))
        {
            Token op = Previous();
            Expr  right = Unary();
            return new Unary()
                   {
                       Op = op,Right = right
                   };
        }

        return Primary();
    }

    private Expr Primary()
    {
        if (Match(TokenType.FALSE))
        {
            return new Literal() { Value = false };
        }

        if (Match(TokenType.TRUE))
        {
            return new Literal() { Value = true };
        }

        if (Match(TokenType.NIL))
        {
            return new Literal() { Value = null };
        }

        if (Match(TokenType.NUMBER, TokenType.STRING))
        {
            return new Literal() { Value = Previous().Literal };
        }

        if (Match(TokenType.LEFT_PAREN))
        {
            Expr expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
            return new Grouping() { Expression = expr };
        }
        
        throw new LoxException(Peek().Line, $"Unexpected token {Peek().Lexeme}.");
    }

    private Token Consume(TokenType token, string errorMessage)
    {
        if (Check(token))
        {
            return Advance();
        }
        throw new LoxException(Peek().Line, errorMessage);
    }

    private void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous().Type == TokenType.SEMICOLON)
            {
                return;
            }

            switch (Peek().Type)
            {
                case TokenType.CLASS:
                case TokenType.FUN:
                case TokenType.VAR:
                case TokenType.FOR:
                case TokenType.IF:
                case TokenType.WHILE:
                case TokenType.PRINT:
                case TokenType.RETURN:
                    return;
            }

            Advance();
        }
    }
    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        return false;
    }

    private Token Advance()
    {
        if (!IsAtEnd())
        {
            _current++;
        }

        return Previous();
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd())
        {
            return false;
        }

        return Peek().Type == type;
    }

    private bool IsAtEnd()
    {
        return Peek().Type==TokenType.EOF;
    }
    
    private Token Peek()
    {
        return _tokens[_current];
    }
    private Token Previous()
    {
        return _tokens[_current - 1];
    }
}