using UnitsNet;
using UnitsNet.Units;
using Iot.Device.Sht3x;
using System.Device.I2c;
using System.CommandLine;

// ReSharper disable All

namespace Amperka.IO.Debugger.Configurations
{
    internal static partial class App
    {
        private static void ConfigureSHT3XCommand(Command root)
        {
            Command sht3XCommand = new Command("sht3x", "Checking the SHT3X functions.");

            #region Options

            Option<int> delayOption = new Option<int>("--delay", () => 25);
            Option<int> busIdOption = new Option<int>("--bus-id", () => 1);
            Option<bool> heaterOption = new Option<bool>("--heater", () => false);
            Option<int> deviceAddressOption = new Option<int>("--device-address", () => Convert.ToInt32(I2cAddress.AddrLow));
            Option<TemperatureUnit> temperatureUnitOption = new Option<TemperatureUnit>("--temperature-unit", () => TemperatureUnit.DegreeCelsius);
            Option<RelativeHumidityUnit> relativeHumidityUnitOption = new Option<RelativeHumidityUnit>("--relative-humidity-unit", () => RelativeHumidityUnit.Percent);

            #endregion

            #region Thermometer command

            Command getDataCommand = new Command("get-data", "Checking the get data.")
            {
                heaterOption,
                temperatureUnitOption,
                relativeHumidityUnitOption
            };

            getDataCommand.SetHandler(SHT3XHHandler, busIdOption, deviceAddressOption, temperatureUnitOption, relativeHumidityUnitOption, heaterOption, delayOption);

            #endregion

            sht3XCommand.AddGlobalOption(delayOption);
            sht3XCommand.AddGlobalOption(busIdOption);
            sht3XCommand.AddGlobalOption(deviceAddressOption);

            sht3XCommand.AddCommand(getDataCommand);

            root.AddCommand(sht3XCommand);
        }

        #region Handlers

        private static async Task SHT3XHHandler(int busId, int deviceAddress, TemperatureUnit temperatureUnit, RelativeHumidityUnit relativeHumidityUnit, bool heater, int delay)
        {
            using (Sht3x sht3X = new Sht3x(I2cDevice.Create(new I2cConnectionSettings(busId, deviceAddress))))
            {
                sht3X.Heater = heater;

                while (Exit())
                {
                    Temperature temperatureResult = sht3X.Temperature.ToUnit(temperatureUnit);

                    RelativeHumidity relativeHumidityResult = sht3X.Humidity.ToUnit(relativeHumidityUnit);

                    Console.WriteLine("Result: {0} {1} | {2} {3}", temperatureResult.Value, temperatureUnit, relativeHumidityResult.Value, relativeHumidityUnit);

                    await Task.Delay(delay);
                }
            }
        }

        #endregion
    }
}
