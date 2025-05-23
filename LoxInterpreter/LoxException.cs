namespace LoxInterpreter;

public class LoxException(int line, string Message) : Exception(Message)
{
    public int Line { get; } = line;

    public override string ToString()
    {
        return $"[line {Line}] Error: {Message}";
    }
}