using System.CommandLine;

// ReSharper disable All

namespace Amperka.IO.Debugger.Configurations
{
    internal static partial class App
    {
        internal static Task<int> Run(string[] args)
        {
            RootCommand root = new RootCommand();

            ConfigureGpioExpanderCommand(root);
            ConfigureI2CHubCommand(root);

            root.SetHandler(DefaultHandler);

            return root.InvokeAsync(args);
        }

        internal static Task Init()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.Black;

            return Task.CompletedTask;
        }

        internal static async Task Close()
        {
            while (Exit())
            {
                await Task.Delay(10);
            }
        }

        private static void DefaultHandler()
        {
            Console.WriteLine("Call help to find out the capabilities of the application.");
        }

        private static bool Exit()
        {
            return !(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape);
        }
    }
}
