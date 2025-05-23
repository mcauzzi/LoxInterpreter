namespace LoxInterpreter;

public class Scanner
{
    private readonly string      _source;
    private readonly List<Token> _tokens = new List<Token>();
    private          int         _start;
    private          int         _current;
    private          int         _line;

    public Scanner(string source)
    {
        _source = source;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            _start = _current;
            ScanToken();
        }

        _tokens.Add(new Token(TokenType.EOF, "", null, _line));
        return _tokens;
    }

    private void ScanToken()
    {
        var c = Advance();
        switch (c)
        {
            case '(': AddToken(TokenType.LEFT_PAREN); break;
            case ')': AddToken(TokenType.RIGHT_PAREN); break;
            case '{': AddToken(TokenType.LEFT_BRACE); break;
            case '}': AddToken(TokenType.RIGHT_BRACE); break;
            case ',': AddToken(TokenType.COMMA); break;
            case '.': AddToken(TokenType.DOT); break;
            case '-': AddToken(TokenType.MINUS); break;
            case '+': AddToken(TokenType.PLUS); break;
            case ';': AddToken(TokenType.SEMICOLON); break;
            case '*': AddToken(TokenType.STAR); break;
            case '!':
                AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;
            case '/':
                if (Match('/'))
                {
                    while (Peek() != '\n' && !IsAtEnd())
                    {
                        Advance();
                    }
                }
                else
                {
                    AddToken(TokenType.SLASH);
                }

                break;
            case ' ':
            case '\r':
            case '\t':
                break;
            case '\n':
                _line++;
                break;
            case '"':
                ManageString();
                break;
            default:
            {
                if (char.IsAsciiDigit(c))
                {
                    ManageNumber();
                }
                else if (char.IsAsciiLetter(c))
                {
                    ManageIdentifier();
                }
                else
                {
                    throw new LoxException(_line, $"Unexpected character '{c}'");
                }

                break;
            }
        }
    }

    private void ManageIdentifier()
    {
        while (char.IsAsciiLetterOrDigit(Peek()))
        {
            Advance();
        }
        
        var text       = _source[_start.._current];
        var keyword = MatchKeyword(text);
        if (keyword != null)
        {
            AddToken(keyword.Value);
            return;
        }
        
        AddToken(TokenType.IDENTIFIER);
    }

    private static TokenType? MatchKeyword(string text) =>
        text switch
        {
            "and"    => TokenType.AND,
            "class"  => TokenType.CLASS,
            "else"   => TokenType.ELSE,
            "false"  => TokenType.FALSE,
            "for"    => TokenType.FOR,
            "fun"    => TokenType.FUN,
            "if"     => TokenType.IF,
            "nil"    => TokenType.NIL,
            "or"     => TokenType.OR,
            "print"  => TokenType.PRINT,
            "return" => TokenType.RETURN,
            "super"  => TokenType.SUPER,
            "this"   => TokenType.THIS,
            "true"   => TokenType.TRUE,
            "var"    => TokenType.VAR,
            "while"  => TokenType.WHILE,
            _        => null
        };

    private void ManageNumber()
    {
        while (char.IsAsciiDigit(Peek()))
        {
            Advance();
        }

        if (Peek() == '.' && char.IsAsciiDigit(PeekNext()))
        {
            Advance();
            while (char.IsAsciiDigit(Peek()))
            {
                Advance();
            }
        }

        var value = double.Parse(_source[_start.._current]);
        AddToken(TokenType.NUMBER, value);
    }

    private char PeekNext()
    {
        if (_current + 1 >= _source.Length)
        {
            return '\0';
        }

        return _source[_current + 1];
    }

    private void ManageString()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
            {
                _line++;
            }

            Advance();
        }

        if (IsAtEnd())
        {
            throw new LoxException(_line, "Unterminated string.");
        }

        Advance();
        var value = _source[(_start).._current];
        AddToken(TokenType.STRING, value);
    }

    private char Peek()
    {
        if (IsAtEnd())
        {
            return '\0';
        }

        return _source[_current];
    }

    private bool Match(char expected)
    {
        if (IsAtEnd())
        {
            return false;
        }

        if (_source[_current] != expected)
        {
            return false;
        }

        _current++;
        return true;
    }

    private void AddToken(TokenType tokenType)
    {
        AddToken(tokenType, null);
    }

    private void AddToken(TokenType tokenType, object literal)
    {
        var text = _source[_start.._current];
        _tokens.Add(new(tokenType, text, literal, _line));
    }

    private char Advance() => _source[_current++];

    private bool IsAtEnd() => _current >= _source.Length;
}