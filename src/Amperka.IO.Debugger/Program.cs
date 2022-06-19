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
#endif

            return await App.Run(args);
        }
    }
}
