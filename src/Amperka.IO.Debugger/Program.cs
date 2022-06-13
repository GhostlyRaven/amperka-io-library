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
            int exitCode = await App.Run(args);

            await App.Close();

            return exitCode;
#else
            return await App.Run(args);
#endif
        }
    }
}
