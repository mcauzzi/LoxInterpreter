using LoxInterpreter;
if (args.Length > 1)
{
    Console.WriteLine("Usage: <path to .lux file>");
    return 64;
}

var interpreter = new LoxRunner();
if (args.Length == 1)
{
    interpreter.RunFile(args[0]);
}
else
{
    interpreter.RunPrompt();
}

return 0;