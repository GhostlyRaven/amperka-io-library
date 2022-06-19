﻿using System.CommandLine;

// ReSharper disable All

namespace Amperka.IO.Debugger.Configurations
{
    internal static partial class App
    {
#if DEBUG
        private static long _exitDelay;

        internal static Task InitExitDelay(long exitDelay)
        {
            _exitDelay = exitDelay > 0 ? exitDelay : 0;

            return Task.CompletedTask;
        }
#endif

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

        internal static bool Exit()
        {
#if DEBUG
            bool exit = _exitDelay > 0;

            _exitDelay--;

            return exit;
#else
            return !(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape);
#endif
        }

        private static void DefaultHandler()
        {
            Console.WriteLine("Call help to find out the capabilities of the application.");
        }
    }
}
