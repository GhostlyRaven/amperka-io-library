using UnitsNet;
using UnitsNet.Units;
using Iot.Device.Common;
using Iot.Device.Lps25h;
using System.Device.I2c;
using System.CommandLine;

// ReSharper disable All

namespace Amperka.IO.Debugger.Configurations
{
    internal static partial class App
    {
        private static void ConfigureLPS25HCommand(Command root)
        {
            Command lps25HCommand = new Command("lps25h", "Checking the LPS25H functions.");

            #region Options

            Option<int> delayOption = new Option<int>("--delay", () => 25);
            Option<int> busIdOption = new Option<int>("--bus-id", () => 1);
            Option<int> deviceAddressOption = new Option<int>("--device-address", () => 92);
            Option<PressureUnit> pressureUnitOption = new Option<PressureUnit>("--pressure-unit", () => PressureUnit.MillimeterOfMercury);
            Option<TemperatureUnit> temperatureUnitOption = new Option<TemperatureUnit>("--temperature-unit", () => TemperatureUnit.DegreeCelsius);

            #endregion

            #region Thermometer command

            Command getDataCommand = new Command("get-data", "Checking the get data.")
            {
                pressureUnitOption,
                temperatureUnitOption
            };

            getDataCommand.SetHandler(LPS25HHandler, busIdOption, deviceAddressOption, temperatureUnitOption, pressureUnitOption, delayOption);

            #endregion

            lps25HCommand.AddGlobalOption(delayOption);
            lps25HCommand.AddGlobalOption(busIdOption);
            lps25HCommand.AddGlobalOption(deviceAddressOption);

            lps25HCommand.AddCommand(getDataCommand);

            root.AddCommand(lps25HCommand);
        }

        #region Handlers

        private static async Task LPS25HHandler(int busId, int deviceAddress, TemperatureUnit temperatureUnit, PressureUnit pressureUnit, int delay)
        {
            using (Lps25h lps25H = new Lps25h(I2cDevice.Create(new I2cConnectionSettings(busId, deviceAddress))))
            {
                while (Exit())
                {
                    Temperature temperatureResult = lps25H.Temperature.ToUnit(temperatureUnit);

                    Pressure pressureResult = lps25H.Pressure.ToUnit(pressureUnit);

                    Length altitudeResult = WeatherHelper.CalculateAltitude(pressureResult, WeatherHelper.MeanSeaLevel, temperatureResult).ToUnit(LengthUnit.Meter);

                    Console.WriteLine("Result: {0} {1} | {2} {3} | {4} m", temperatureResult.Value, temperatureUnit, pressureResult.Value, pressureUnit, altitudeResult.Value);

                    await Task.Delay(delay);
                }
            }
        }

        #endregion
    }
}
