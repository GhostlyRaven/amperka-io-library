using System.CommandLine;

// ReSharper disable All

namespace Amperka.IO.Debugger.Configurations
{
    internal static partial class App
    {
        internal static Task<int> Run(string[] args)
        {
            RootCommand root = new RootCommand();

            ConfigureGpioControllerCommand(root);
            ConfigureGpioExpanderCommand(root);
            ConfigureI2CHubCommand(root);

            root.SetHandler(DefaultHandler);

            return root.InvokeAsync(args);
        }

        internal static Task Init()
        {
            if (Console.BackgroundColor == ConsoleColor.Green)
            {
                Console.BackgroundColor = ConsoleColor.Black;
            }

            Console.ForegroundColor = ConsoleColor.Green;

            return Task.CompletedTask;
        }

        private static bool Exit()
        {
            return !(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape);
        }

        private static Task DefaultHandler()
        {
            Console.WriteLine("Call help to find out the capabilities of the application.");

            return Task.CompletedTask;
        }
    }
}
