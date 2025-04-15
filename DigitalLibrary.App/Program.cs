using DigitalLibrary.App;
using DigitalLibrary.Core;

var provider = new DefaultDirectoryProvider();
var db = new BookDatabase(provider);
var processor = new CommandProcessor(db);

Console.WriteLine("Welcome to Digital Library CLI!");
Console.WriteLine("Type 'help' to see available commands. Type 'exit' to quit.");

while (true)
{
    Console.Write("> ");
    var line = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(line))
        continue;

    if (line.Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    var command = line.Split(' ');
    processor.Execute(command);
}
