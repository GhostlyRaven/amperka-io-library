using Amperka.IO.Debugger.Configurations;

// ReSharper disable All

namespace Amperka.IO.Debugger
{
    internal static class Program
    {
        internal static async Task<int> Main(string[] args)
        {
            await App.Init();

#if DEBUG
            await App.InitExitDelay(ushort.MaxValue);

            int exitCode = await App.Run(args);

            while (App.Exit())
            {
                await Task.Delay(10);
            }

            return exitCode;
#else
            return await App.Run(args);
#endif
        }
    }
}
