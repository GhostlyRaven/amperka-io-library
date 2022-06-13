using System.Device.I2c;
using Amperka.IO.Devices;
using System.CommandLine;

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
            Option<int> channelOption = new Option<int>("--channel", () => 0);
            Option<int> deviceAddressOption = new Option<int>("--address", () => 112);

            #endregion

            #region Set channel

            Command setChannel = new Command("set-channel", "Checking the set channel.")
            {
                channelOption
            };

            setChannel.SetHandler(SetChannelHandler, busIdOption, deviceAddressOption, channelOption);

            #endregion

            i2cHub.AddGlobalOption(busIdOption);
            i2cHub.AddGlobalOption(deviceAddressOption);

            i2cHub.AddCommand(setChannel);

            root.AddCommand(i2cHub);
        }

        #region Handlers

        private static async Task SetChannelHandler(int busId, int deviceAddress, int channel)
        {
            await using (II2CHub hub = await AmperkaDevices.CreateI2CHubAsync(new I2cConnectionSettings(busId, deviceAddress)))
            {
                Console.WriteLine("Checking the set channel ({0}).", channel);

                await hub.SetChannelAsync(channel);
            }
        }

        #endregion
    }
}
