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
            Command i2cHubCommand = new Command("i2c-hub", "Checking the I2C hub functions.");

            #region Options

            Option<int> delayOption = new Option<int>("--delay", () => 25);
            Option<int> busIdOption = new Option<int>("--bus-id", () => 1);
            Option<int> channelOption = new Option<int>("--channel", () => 0);
            Option<bool> useIndexOption = new Option<bool>("--use-index", () => false);
            Option<bool> useAsyncMethodOption = new Option<bool>("--use-async-method", () => false);
            Option<int> deviceAddressOption = new Option<int>("--device-address", () => I2CHub.DefaultAddress);

            #endregion

            #region Set channel

            Command setChannelCommand = new Command("set-channel", "Checking the set channel.")
            {
                channelOption,
                delayOption
            };

            setChannelCommand.SetHandler(SetChannelHandler, busIdOption, deviceAddressOption, channelOption, delayOption);

            #endregion

            #region For Each

            Command forEachCommand = new Command("for-each", "Checking the for each set channel.")
            {
                delayOption,
                channelOption,
                useIndexOption,
                useAsyncMethodOption,
            };

            forEachCommand.SetHandler(ForEachHandler, busIdOption, deviceAddressOption, delayOption, useIndexOption, useAsyncMethodOption);

            #endregion

            #region For Each Async

            Command forEachAsyncCommand = new Command("for-each-async", "Checking the for each set channel.")
            {
                delayOption,
                channelOption,
                useIndexOption,
                useAsyncMethodOption,
            };

            forEachAsyncCommand.SetHandler(ForEachAsyncHandler, busIdOption, deviceAddressOption, delayOption, useIndexOption, useAsyncMethodOption);

            #endregion

            i2cHubCommand.AddGlobalOption(busIdOption);
            i2cHubCommand.AddGlobalOption(deviceAddressOption);

            i2cHubCommand.AddCommand(forEachCommand);
            i2cHubCommand.AddCommand(setChannelCommand);
            i2cHubCommand.AddCommand(forEachAsyncCommand);

            root.AddCommand(i2cHubCommand);
        }

        #region Handlers

        private static async Task SetChannelHandler(int busId, int deviceAddress, int channel, int delay)
        {
            await using (I2CHub hub = new I2CHub(I2cDevice.Create(new I2cConnectionSettings(busId, deviceAddress))))
            {
                Console.WriteLine("Checking the set channel ({0}).", channel);

                await hub.SetChannelAsync(channel);

                while (Exit())
                {
                    await Task.Delay(delay);
                }
            }
        }

        private static async Task ForEachHandler(int busId, int deviceAddress, int delay, bool useIndex, bool useAsyncMethod)
        {
            await using (I2CHub hub = new I2CHub(I2cDevice.Create(new I2cConnectionSettings(busId, deviceAddress))))
            {
                if (useIndex)
                {
                    if (useAsyncMethod)
                    {
                        hub.ForEach(async (index, cancellationToken) =>
                        {
                            Console.WriteLine("Channel {0} delay started with async method.", index);

                            await Task.Delay(delay, cancellationToken);
                        });
                    }
                    else
                    {
                        hub.ForEach(index =>
                        {
                            Console.WriteLine("Channel {0} delay started with method.", index);

                            Task.Delay(delay).Wait();
                        });
                    }
                }
                else
                {
                    if (useAsyncMethod)
                    {
                        hub.ForEach(async cancellationToken =>
                        {
                            Console.WriteLine("Channel delay started with async method.");

                            await Task.Delay(delay, cancellationToken);
                        });
                    }
                    else
                    {
                        hub.ForEach(() =>
                        {
                            Console.WriteLine("Channel delay started with method.");

                            Task.Delay(delay).Wait();
                        });
                    }
                }
            }
        }

        private static async Task ForEachAsyncHandler(int busId, int deviceAddress, int delay, bool useIndex, bool useAsyncMethod)
        {
            await using (I2CHub hub = new I2CHub(I2cDevice.Create(new I2cConnectionSettings(busId, deviceAddress))))
            {
                if (useIndex)
                {
                    if (useAsyncMethod)
                    {
                        await hub.ForEachAsync(async (index, cancellationToken) =>
                        {
                            Console.WriteLine("Async channel {0} delay started with async method.", index);

                            await Task.Delay(delay, cancellationToken);
                        });
                    }
                    else
                    {
                        await hub.ForEachAsync(index =>
                        {
                            Console.WriteLine("Async channel {0} delay started with method.", index);

                            Task.Delay(delay).Wait();
                        });
                    }
                }
                else
                {
                    if (useAsyncMethod)
                    {
                        await hub.ForEachAsync(async cancellationToken =>
                        {
                            Console.WriteLine("Async channel delay started with async method.");

                            await Task.Delay(delay, cancellationToken);
                        });
                    }
                    else
                    {
                        await hub.ForEachAsync(() =>
                        {
                            Console.WriteLine("Async channel delay started with method.");

                            Task.Delay(delay).Wait();
                        });
                    }
                }
            }
        }

        #endregion
    }
}
