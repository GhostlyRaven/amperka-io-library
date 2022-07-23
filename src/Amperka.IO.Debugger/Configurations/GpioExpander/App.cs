using System.Device.I2c;
using System.CommandLine;
using Amperka.IO.Devices;
using Amperka.IO.Extensions;
using Amperka.IO.Devices.Settings;

// ReSharper disable All

namespace Amperka.IO.Debugger.Configurations
{
    internal static partial class App
    {
        private static void ConfigureGpioExpanderCommand(Command root)
        {
            Command gpioExpander = new Command("gpio-expander", "Checking the GPIO expander functions.");

            #region Options

            Option<int> pwmOption = new Option<int>("--pwm", () => 255);
            Option<int> freqOption = new Option<int>("--freq", () => 100);
            Option<int> delayOption = new Option<int>("--delay", () => 25);
            Option<int> busIdOption = new Option<int>("--bus-id", () => 1);
            Option<int> readPinOption = new Option<int>("--read-pin", () => 0);
            Option<int> adcSpeedOption = new Option<int>("--adc-speed", () => 7);
            Option<int> writePinOption = new Option<int>("--write-pin", () => 1);
            Option<bool> useReadonlyOption = new Option<bool>("--use-readonly", () => false);
            Option<int> newDeviceAddressOption = new Option<int>("--new-device-address", () => 40);
            Option<int> deviceAddressOption = new Option<int>("--device-address", () => GpioExpander.DefaultAddress);

            #endregion

            #region Port command

            Command portCommand = new Command("port", "Checking the port signal.")
            {
                delayOption
            };

            portCommand.SetHandler(PortHandler, busIdOption, deviceAddressOption, delayOption);

            #endregion

            #region Digital command

            Command digitalCommand = new Command("digital", "Checking the digital signal.")
            {
                delayOption,
                readPinOption,
                writePinOption
            };

            digitalCommand.SetHandler(DigitalHandler, busIdOption, deviceAddressOption, delayOption, writePinOption, readPinOption);

            #endregion

            #region Analog command

            Command anlogCommand = new Command("analog", "Checking the analog signal.")
            {
                delayOption,
                readPinOption,
                writePinOption,
                useReadonlyOption
            };

            anlogCommand.SetHandler(AnalogHandler, busIdOption, deviceAddressOption, delayOption, writePinOption, readPinOption, useReadonlyOption);

            #endregion

            #region Pwm random command

            Command pwmRandomCommand = new Command("pwm-random".ToLower(), "Checking the PWM signal.")
            {
                freqOption,
                delayOption,
                writePinOption
            };

            pwmRandomCommand.SetHandler(PwmRandomHandler, busIdOption, deviceAddressOption, delayOption, writePinOption, freqOption);

            #endregion

            #region Pwm command

            Command pwmCommand = new Command("pwm", "Checking the PWM signal.")
            {
                pwmOption,
                freqOption,
                delayOption,
                writePinOption
            };

            pwmCommand.SetHandler(PwmHandler, busIdOption, deviceAddressOption, delayOption, writePinOption, freqOption, pwmOption);

            #endregion

            #region Adc command

            Command adcCommand = new Command("adc", "Checking the ADC speed.")
            {
                adcSpeedOption
            };

            adcCommand.SetHandler(AdcHandler, busIdOption, deviceAddressOption, adcSpeedOption);

            #endregion

            #region Get device id command

            Command getDeviceIdCommand = new Command("get-device-id", "Checking the get device id.");

            getDeviceIdCommand.SetHandler(GetDeviceIdHandler, busIdOption, deviceAddressOption);

            #endregion

            #region Change address command

            Command changeChipAddressCommand = new Command("change-chip-address", "Checking the chip change address.")
            {
                newDeviceAddressOption
            };

            changeChipAddressCommand.SetHandler(ChangeChipAddressHandler, busIdOption, deviceAddressOption, newDeviceAddressOption);

            #endregion

            #region Save address command

            Command saveChipAddressCommand = new Command("save-chip-address", "Checking the chip save address.");

            saveChipAddressCommand.SetHandler(SaveAddressHandler, busIdOption, deviceAddressOption);

            #endregion

            #region Reset chip command

            Command resetChipCommand = new Command("reset-chip", "Checking the chip reset.");

            resetChipCommand.SetHandler(ResetChipHandler, busIdOption, deviceAddressOption);

            #endregion

            gpioExpander.AddGlobalOption(busIdOption);
            gpioExpander.AddGlobalOption(deviceAddressOption);

            gpioExpander.AddCommand(adcCommand);
            gpioExpander.AddCommand(pwmCommand);
            gpioExpander.AddCommand(portCommand);
            gpioExpander.AddCommand(anlogCommand);
            gpioExpander.AddCommand(digitalCommand);
            gpioExpander.AddCommand(pwmRandomCommand);
            gpioExpander.AddCommand(resetChipCommand);
            gpioExpander.AddCommand(saveChipAddressCommand);
            gpioExpander.AddCommand(changeChipAddressCommand);

            root.AddCommand(gpioExpander);
        }

        #region Handlers

        private static async Task PortHandler(int busId, int deviceAddress, int delay)
        {
            await using (GpioExpander expander = new GpioExpander(I2cDevice.Create(new I2cConnectionSettings(busId, deviceAddress))))
            {
                Console.WriteLine("Checking the port signal.");

                while (Exit())
                {
                    Console.WriteLine("Result: {0}", await expander.DigitalReadPortAsync());

                    await expander.DigitalPortLowLevelAsync();

                    await Task.Delay(delay);

                    Console.WriteLine("Result: {0}", await expander.DigitalReadPortAsync());

                    await expander.DigitalWritePortAsync(31);

                    await Task.Delay(delay);

                    Console.WriteLine("Result: {0}", await expander.DigitalReadPortAsync());

                    await expander.DigitalPortHighLevelAsync();

                    await Task.Delay(delay);
                }
            }
        }

        private static async Task DigitalHandler(int busId, int deviceAddress, int delay, int writePin, int readPin)
        {
            await using (GpioExpander expander = new GpioExpander(I2cDevice.Create(new I2cConnectionSettings(busId, deviceAddress))))
            {
                Console.WriteLine("Checking the digital signal.");

                await expander.PinModeAsync(readPin, PinMode.Input);
                await expander.PinModeAsync(writePin, PinMode.Output);

                while (Exit())
                {
                    bool result = await expander.TroykaButtonClickAsync(readPin);

                    Console.WriteLine("Result: {0}", result);

                    expander.DigitalWrite(writePin, result);

                    await Task.Delay(delay);
                }
            }
        }

        private static async Task AnalogHandler(int busId, int deviceAddress, int delay, int writePin, int readPin, bool useReadonly)
        {
            await using (GpioExpander expander = new GpioExpander(I2cDevice.Create(new I2cConnectionSettings(busId, deviceAddress))))
            {
                Console.WriteLine("Checking the analog signal.");

                if (useReadonly)
                {
                    while (Exit())
                    {
                        double result = await expander.AnalogReadAsync(readPin) / GpioExpander.AdcBitrate;

                        Console.WriteLine("Result: {0}", result);

                        await Task.Delay(delay);
                    }
                }
                else
                {
                    while (Exit())
                    {
                        int result = await expander.AnalogReadAsync(readPin);

                        Console.WriteLine("Result: {0}", result);

                        await expander.AnalogWriteAsync(writePin, result, ScaleMode.ADC);

                        await Task.Delay(delay);
                    }
                }
            }
        }

        private static async Task PwmRandomHandler(int busId, int deviceAddress, int delay, int writePin, int freq)
        {
            await using (GpioExpander expander = new GpioExpander(I2cDevice.Create(new I2cConnectionSettings(busId, deviceAddress))))
            {
                Console.WriteLine("Checking the PWM signal ({0}).", freq);

                await expander.PwmFreqAsync(freq);

                while (Exit())
                {
                    int result = Random.Shared.Next(0, 255);

                    Console.WriteLine("Result: {0}", result);

                    await expander.AnalogWriteAsync(writePin, result);

                    await Task.Delay(delay);
                }
            }
        }

        private static async Task PwmHandler(int busId, int deviceAddress, int delay, int writePin, int freq, int pwm)
        {
            await using (GpioExpander expander = new GpioExpander(I2cDevice.Create(new I2cConnectionSettings(busId, deviceAddress))))
            {
                Console.WriteLine("Checking the PWM signal ({0} | {1}).", freq, pwm);

                await expander.PwmFreqAsync(freq);
                await expander.AnalogWriteAsync(writePin, pwm);

                while (Exit())
                {
                    await Task.Delay(delay);
                }
            }
        }

        private static async Task AdcHandler(int busId, int deviceAddress, int adcSpeed)
        {
            await using (GpioExpander expander = new GpioExpander(I2cDevice.Create(new I2cConnectionSettings(busId, deviceAddress))))
            {
                Console.WriteLine("Checking the ADC speed ({0}).", adcSpeed);

                await expander.AdcSpeedAsync(adcSpeed);
            }
        }

        private static async Task GetDeviceIdHandler(int busId, int deviceAddress)
        {
            Console.WriteLine("Checking the get device id.");

            await using (GpioExpander expander = new GpioExpander(I2cDevice.Create(new I2cConnectionSettings(busId, deviceAddress))))
            {
                Console.WriteLine("Device id: {0}.", await expander.GetDeviceIdAsync());
                Console.WriteLine("Device id: {0}.", (await expander.GetDeviceIdAsync()).ToString("X"));
            }
        }

        private static async Task ChangeChipAddressHandler(int busId, int deviceAddress, int newDeviceAddress)
        {
            Console.WriteLine("Checking the chip change address ({0} => {1}).", deviceAddress, newDeviceAddress);

            await using (GpioExpander expander = new GpioExpander(I2cDevice.Create(new I2cConnectionSettings(busId, deviceAddress))))
            {
                await expander.ChangeAddressAsync(newDeviceAddress);
            }
        }

        private static async Task SaveAddressHandler(int busId, int deviceAddress)
        {
            Console.WriteLine("Checking the chip save address ({0}).", deviceAddress);

            await using (GpioExpander expander = new GpioExpander(I2cDevice.Create(new I2cConnectionSettings(busId, deviceAddress))))
            {
                await expander.SaveAddressAsync();
            }
        }

        private static async Task ResetChipHandler(int busId, int deviceAddress)
        {
            Console.WriteLine("Checking the chip reset ({0}).", deviceAddress);

            await using (GpioExpander expander = new GpioExpander(I2cDevice.Create(new I2cConnectionSettings(busId, deviceAddress))))
            {
                await expander.ResetAsync();
            }
        }

        #endregion
    }
}
