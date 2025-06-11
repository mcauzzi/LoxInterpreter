using LoxInterpreter.Parser;

namespace LoxInterpreter;

public class LoxRunner
{
    public void RunFile(string path)
    {
        var content = File.ReadAllText(path);
        Run(content);
    }

    public void RunPrompt()
    {
        Console.WriteLine("Lux Interpreter");
        Console.WriteLine("Type 'exit' to quit.");
        do
        {
            Console.Write("> ");
            var line = Console.ReadLine();
            if (line != null && line.Trim().ToLower() == "exit")
                break;
            if (line != null)
            {
                try
                {
                    Run(line);
                }
                catch (LoxException e)
                {
                    Console.WriteLine(e);
                }
            }
        } while (true);
    }

    void Run(string content)
    {
        var scanner = new Scanner(content);
        var tokens = scanner.ScanTokens();
        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
        var parser = new LoxParser(tokens);
        var expr = parser.Parse();
        if (expr != null)
        {
            Console.WriteLine("Parsing successful. Expression tree:");
            Console.WriteLine(new AstPrinter().Print(expr));
            var interpreter = new Interpreter.LoxInterpreter();
            interpreter.Interpret(expr);
        }
        else
        {
            Console.WriteLine("Parsing failed.");
        }
    }
}