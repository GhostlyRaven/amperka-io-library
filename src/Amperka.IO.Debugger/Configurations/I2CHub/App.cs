using System.Device.I2c;
using Amperka.IO.Devices;
using System.CommandLine;
using Amperka.IO.Extensions;

// ReSharper disable All

namespace Amperka.IO.Debugger.Configurations
{
    internal static partial class App
    {
        private static void ConfigureI2CHubCommand(Command root)
        {
            Command i2cHub = new Command("i2c-hub", "Checking the I2C Hub functions.");

            #region Options

            Option<int> busIdOption = new Option<int>("--bus-id", () => 1);
            Option<int> delayOption = new Option<int>("--delay", () => 5000);
            Option<int> channelOption = new Option<int>("--channel", () => 0);
            Option<int> deviceAddressOption = new Option<int>("--address", () => 112);

            #endregion

            #region Set channel

            Command setChannel = new Command("set-channel", "Checking the set channel.")
            {
                channelOption,
                delayOption
            };

            setChannel.SetHandler(SetChannelHandler, busIdOption, deviceAddressOption, channelOption, delayOption);

            #endregion

            #region For Each

            Command forEach = new Command("for-each", "Checking the for each set channel.")
            {
                channelOption,
                delayOption
            };

            forEach.SetHandler(ForEachHandler, busIdOption, deviceAddressOption, channelOption, delayOption);

            #endregion

            #region For Each Async

            Command forEachAsync = new Command("for-each-async", "Checking the for each set channel.")
            {
                channelOption,
                delayOption
            };

            forEachAsync.SetHandler(ForEachAsyncHandler, busIdOption, deviceAddressOption, channelOption, delayOption);

            #endregion

            i2cHub.AddGlobalOption(busIdOption);
            i2cHub.AddGlobalOption(deviceAddressOption);

            i2cHub.AddCommand(forEach);
            i2cHub.AddCommand(setChannel);
            i2cHub.AddCommand(forEachAsync);

            root.AddCommand(i2cHub);
        }

        #region Handlers

        private static async Task SetChannelHandler(int busId, int deviceAddress, int channel, int delay)
        {
            await using (II2CHub hub = await AmperkaDevices.CreateI2CHubAsync(new I2cConnectionSettings(busId, deviceAddress)))
            {
                Console.WriteLine("Checking the set channel ({0}).", channel);

                await hub.SetChannelAsync(channel);

                while (Exit())
                {
                    await Task.Delay(delay);
                }
            }
        }

        private static async Task ForEachHandler(int busId, int deviceAddress, int channel, int delay)
        {
            await using (II2CHub hub = await AmperkaDevices.CreateI2CHubAsync(new I2cConnectionSettings(busId, deviceAddress)))
            {
                hub.ForEach((number) =>
                {
                    Console.WriteLine("Channel {0} delay started.", number);

                    Task.Delay(delay).Wait();
                });

                Console.WriteLine();

                hub.ForEach(async (number) =>
                {
                    Console.WriteLine("Channel {0} delay started.", number);

                    await Task.Delay(delay);
                });
            }
        }

        private static async Task ForEachAsyncHandler(int busId, int deviceAddress, int channel, int delay)
        {
            await using (II2CHub hub = await AmperkaDevices.CreateI2CHubAsync(new I2cConnectionSettings(busId, deviceAddress)))
            {
                await hub.ForEachAsync((number) =>
                {
                    Console.WriteLine("Channel {0} delay started.", number);

                    Task.Delay(delay).Wait();
                });

                Console.WriteLine();

                await hub.ForEachAsync(async (number, cancellationToken) =>
                {
                    Console.WriteLine("Channel {0} delay started.", number);

                    await Task.Delay(delay, cancellationToken);
                });
            }
        }

        #endregion
    }
}
