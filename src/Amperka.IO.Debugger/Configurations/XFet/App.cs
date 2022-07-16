using System.Device.Spi;
using Amperka.IO.Devices;
using System.CommandLine;

// ReSharper disable All

namespace Amperka.IO.Debugger.Configurations
{
    internal static partial class App
    {
        private static void ConfigureXFetCommand(Command root)
        {
            Command xFet = new Command("x-fet", "Checking the X-Fet functions.");

            #region Options

            Option<int> countOption = new Option<int>("--count", () => 1);
            Option<int> delayOption = new Option<int>("--delay", () => 25);
            Option<int> busIdOption = new Option<int>("--bus-id", () => 0);
            Option<int> chipSelectLineOption = new Option<int>("--chip-select-line", () => 0);

            #endregion

            #region Digital write pins command

            Command digitalWritePinsCommand = new Command("digital-write-pins", "Checking the digital write pins.");

            digitalWritePinsCommand.SetHandler(DigitalWritePins, busIdOption, chipSelectLineOption, countOption, delayOption);

            #endregion

            #region Digital write all pins command

            Command digitalWriteAllPinsCommand = new Command("digital-write-all-pins", "Checking the digital write all pins.");

            digitalWriteAllPinsCommand.SetHandler(DigitalWriteAllPins, busIdOption, chipSelectLineOption, countOption, delayOption);

            #endregion

            #region Digital write many pins command

            Command digitalWriteManyPinsCommand = new Command("digital-write-many-pins", "Checking the digital write pins for devices.");

            digitalWriteManyPinsCommand.SetHandler(DigitalWriteManyPins, busIdOption, chipSelectLineOption, countOption, delayOption);

            #endregion

            #region Digital write many all pins command

            Command digitalWriteManyAllPinsCommand = new Command("digital-write-many-all-pins", "Checking the digital write all pins for devices.");

            digitalWriteManyAllPinsCommand.SetHandler(DigitalWriteManyAllPins, busIdOption, chipSelectLineOption, countOption, delayOption);

            #endregion

            #region Digital write many all pins and devices command

            Command digitalWriteManyAllPinsAndDevicesCommand = new Command("digital-write-many-all-pins-and-devices", "Checking the digital write all pins for all devices.");

            digitalWriteManyAllPinsAndDevicesCommand.SetHandler(DigitalWriteManyAllPinsAndDevices, busIdOption, chipSelectLineOption, countOption, delayOption);

            #endregion

            xFet.AddGlobalOption(countOption);
            xFet.AddGlobalOption(delayOption);
            xFet.AddGlobalOption(busIdOption);
            xFet.AddGlobalOption(chipSelectLineOption);

            xFet.AddCommand(digitalWritePinsCommand);
            xFet.AddCommand(digitalWriteAllPinsCommand);
            xFet.AddCommand(digitalWriteManyPinsCommand);
            xFet.AddCommand(digitalWriteManyAllPinsCommand);
            xFet.AddCommand(digitalWriteManyAllPinsAndDevicesCommand);

            root.AddCommand(xFet);
        }

        #region Handlers

        private static async Task DigitalWritePins(int busId, int chipSelectLine, int count, int delay)
        {
            await using (XFet fet = new XFet(SpiDevice.Create(new SpiConnectionSettings(busId, chipSelectLine)), count))
            {
                while (Exit())
                {
                    for (int pin = 0; pin < XFet.PinCount; pin++)
                    {
                        await fet.DigitalWriteAsync(pin, true);

                        Console.WriteLine("Pin signal level ({0}): {1}", pin, await fet.DigitalReadAsync(pin));

                        await Task.Delay(delay);

                        await fet.DigitalWriteAsync(pin, false);

                        Console.WriteLine("Pin signal level ({0}): {1}", pin, await fet.DigitalReadAsync(pin));

                        await Task.Delay(delay);
                    }
                }
            }
        }

        private static async Task DigitalWriteAllPins(int busId, int chipSelectLine, int count, int delay)
        {
            await using (XFet fet = new XFet(SpiDevice.Create(new SpiConnectionSettings(busId, chipSelectLine)), count))
            {
                while (Exit())
                {
                    await fet.DigitalWriteAsync(true);

                    Console.WriteLine("Pin signal level: {0}", await fet.DigitalReadAsync());

                    await Task.Delay(delay);

                    await fet.DigitalWriteAsync(false);

                    Console.WriteLine("Pin signal level: {0}", await fet.DigitalReadAsync());

                    await Task.Delay(delay);
                }
            }
        }

        private static async Task DigitalWriteManyPins(int busId, int chipSelectLine, int count, int delay)
        {
            await using (XFet fet = new XFet(SpiDevice.Create(new SpiConnectionSettings(busId, chipSelectLine)), count))
            {
                while (Exit())
                {
                    for (int device = 0; device < fet.DeviceCount; device++)
                    {
                        for (int pin = 0; pin < XFet.PinCount; pin++)
                        {
                            await fet.DigitalWriteManyAsync(device, pin, true);

                            Console.WriteLine("Pin signal level ({0} | {1}): {2}", device, pin, await fet.DigitalReadManyAsync(device, pin));

                            await Task.Delay(delay);

                            await fet.DigitalWriteManyAsync(device, pin, false);

                            Console.WriteLine("Pin signal level ({0} | {1}): {2}", device, pin, await fet.DigitalReadManyAsync(device, pin));

                            await Task.Delay(delay);
                        }
                    }
                }
            }
        }

        private static async Task DigitalWriteManyAllPins(int busId, int chipSelectLine, int count, int delay)
        {
            await using (XFet fet = new XFet(SpiDevice.Create(new SpiConnectionSettings(busId, chipSelectLine)), count))
            {
                while (Exit())
                {
                    for (int device = 0; device < fet.DeviceCount; device++)
                    {
                        await fet.DigitalWriteManyAsync(device, true);

                        Console.WriteLine("Pin signal level ({0}): {1}", device, await fet.DigitalReadManyAsync(device));

                        await Task.Delay(delay);

                        await fet.DigitalWriteManyAsync(device, false);

                        Console.WriteLine("Pin signal level ({0}): {1}", device, await fet.DigitalReadManyAsync(device));

                        await Task.Delay(delay);
                    }
                }
            }
        }

        private static async Task DigitalWriteManyAllPinsAndDevices(int busId, int chipSelectLine, int count, int delay)
        {
            await using (XFet fet = new XFet(SpiDevice.Create(new SpiConnectionSettings(busId, chipSelectLine)), count))
            {
                while (Exit())
                {
                    await fet.DigitalWriteManyAsync(true);

                    Console.WriteLine("Pin signal level: {0}", await fet.DigitalReadManyAsync());

                    await Task.Delay(delay);

                    await fet.DigitalWriteManyAsync(false);

                    Console.WriteLine("Pin signal level: {0}", await fet.DigitalReadManyAsync());

                    await Task.Delay(delay);
                }
            }
        }

        #endregion
    }
}
