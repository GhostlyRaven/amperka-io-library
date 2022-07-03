using Amperka.IO.Debugger.Configurations;

// ReSharper disable All

namespace Amperka.IO.Debugger
{
    //Debug configurations. Console mode use only externalTerminal or integratedTerminal.

    internal static class Program
    {
        internal static async Task<int> Main(string[] args)
        {
            await App.Init();

            return await App.Run(args);
        }
    }
}
