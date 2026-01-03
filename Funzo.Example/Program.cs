namespace Funzo.Example;

internal class Program
{
    static async Task Main(string[] args)
    {
        var looping = true;
        while (looping)
        {
            Console.WriteLine("What do you want to do?");
            Console.WriteLine("1 - Add Todo item");
            Console.WriteLine("2 - Remove Todo item");
            Console.WriteLine("3 - List Todo items");

            Console.WriteLine("q to quit");
            var command = Console.ReadLine();

            var appCommand = AppCommand.From(command);

            appCommand.Switch(
                add => OnAdd(),
                delete => OnDelete(),
                list => OnList(),
                quit => { looping = false; },
                unknown => OnUnknown()
            );

        }
    }

    static void OnAdd() { }
    static void OnDelete() { }
    static void OnList() { }
    static void OnUnknown() { }
}



public record AddTodoCommand;
public record RemoveTodoCommand;
public record ListTodoCommand;
public record QuitCommand;
public record UnknownCommand;

[Union<AddTodoCommand, RemoveTodoCommand, ListTodoCommand, QuitCommand, UnknownCommand>]
public partial class AppCommand
{
    public static AppCommand From(string? command)
        => command?.Trim().ToLower() switch
        {
            "1" => new AddTodoCommand(),
            "2" => new RemoveTodoCommand(),
            "3" => new ListTodoCommand(),
            "q" => new QuitCommand(),

            _ => new UnknownCommand(),
        };
}