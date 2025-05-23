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
        foreach (var token in scanner.ScanTokens())
        {
            Console.WriteLine(token);
        }
    }
}