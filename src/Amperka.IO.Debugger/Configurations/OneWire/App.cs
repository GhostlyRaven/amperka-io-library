using UnitsNet;
using UnitsNet.Units;
using System.CommandLine;
using Iot.Device.OneWire;

// ReSharper disable All

namespace Amperka.IO.Debugger.Configurations
{
    internal static partial class App
    {
        private static void ConfigureOneWireCommand(Command root)
        {
            Command oneWireCommand = new Command("one-wire", "Checking the OneWire functions.");

            #region Options

            Option<int> delayOption = new Option<int>("--delay", () => 25);
            Option<TemperatureUnit> temperatureUnitOption = new Option<TemperatureUnit>("--temperature-unit", () => TemperatureUnit.DegreeCelsius);

            #endregion

            #region Thermometer command

            Command thermometerCommand = new Command("thermometer", "Checking the thermometer.")
            {
                temperatureUnitOption
            };

            thermometerCommand.SetHandler(ThermometerHandler, temperatureUnitOption, delayOption);

            #endregion

            oneWireCommand.AddGlobalOption(delayOption);

            oneWireCommand.AddCommand(thermometerCommand);

            root.AddCommand(oneWireCommand);
        }

        #region Handlers

        private static async Task ThermometerHandler(TemperatureUnit unit, int delay)
        {
            OneWireThermometerDevice[] thermometers = OneWireThermometerDevice.EnumerateDevices().ToArray();

            while (Exit())
            {
                foreach (OneWireThermometerDevice thermometer in thermometers)
                {
                    Temperature result = (await thermometer.ReadTemperatureAsync()).ToUnit(unit);

                    Console.WriteLine("Result ({0} | {1}): {2} {3}", thermometer.BusId, thermometer.DeviceId, result.Value, unit);
                }

                await Task.Delay(delay);
            }
        }

        #endregion
    }
}
