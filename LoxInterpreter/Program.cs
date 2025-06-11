using LoxInterpreter;
if (args.Length > 1)
{
    Console.WriteLine("Usage: <path to .lux file>");
    return 64;
}

var runner = new LoxRunner();
if (args.Length == 1)
{
    runner.RunFile(args[0]);
}
else
{
    runner.RunPrompt();
}

return 0;