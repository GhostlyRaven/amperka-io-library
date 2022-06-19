using System.Device.I2c;
using System.CommandLine;
using Amperka.IO.Devices;
using Amperka.IO.Extensions;
using Amperka.IO.Devices.GpioExpander;

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
            Option<int> busIdOption = new Option<int>("--bus-id", () => 1);
            Option<int> delayOption = new Option<int>("--delay", () => 5000);
            Option<int> readPinOption = new Option<int>("--read-pin", () => 0);
            Option<int> adcSpeedOption = new Option<int>("--adc-speed", () => 7);
            Option<int> writePinOption = new Option<int>("--write-pin", () => 1);
            Option<int> deviceAddressOption = new Option<int>("--address", () => 42);
            Option<bool> readonlyOption = new Option<bool>("--readonly", () => false);
            Option<int> newAddressOption = new Option<int>("--new-address", () => 42);

            #endregion

            #region Port command

            Command port = new Command("port", "Checking the port signal.")
            {
                delayOption
            };

            port.SetHandler(PortHandler, busIdOption, deviceAddressOption, delayOption);

            #endregion

            #region Digital command

            Command digital = new Command("digital", "Checking the digital signal.")
            {
                readPinOption,
                writePinOption
            };

            digital.SetHandler(DigitalHandler, busIdOption, deviceAddressOption, readPinOption, writePinOption);

            #endregion

            #region Analog command

            Command anlog = new Command("analog", "Checking the analog signal.")
            {
                readPinOption,
                writePinOption,
                readonlyOption
            };

            anlog.SetHandler(AnalogHandler, busIdOption, deviceAddressOption, readPinOption, writePinOption, readonlyOption);

            #endregion

            #region Pwm random command

            Command pwmRandom = new Command("pwm-random".ToLower(), "Checking the PWM signal.")
            {
                freqOption,
                writePinOption,
                delayOption
            };

            pwmRandom.SetHandler(PwmRandomHandler, busIdOption, deviceAddressOption, freqOption, writePinOption, delayOption);

            #endregion

            #region Pwm command

            Command pwm = new Command("pwm", "Checking the PWM signal.")
            {
                freqOption,
                writePinOption,
                pwmOption,
                delayOption
            };

            pwm.SetHandler(PwmHandler, busIdOption, deviceAddressOption, freqOption, writePinOption, pwmOption, delayOption);

            #endregion

            #region Adc command

            Command adc = new Command("adc", "Checking the ADC speed.")
            {
                adcSpeedOption
            };

            adc.SetHandler(AdcHandler, busIdOption, deviceAddressOption, adcSpeedOption);

            #endregion

            #region Adress command

            Command address = new Command("address", "Checking the chip change and save address.")
            {
                newAddressOption
            };

            address.SetHandler(AddressHandler, busIdOption, deviceAddressOption, newAddressOption);

            #endregion

            #region Reset command 

            Command reset = new Command("reset", "Checking the chip reset.");

            reset.SetHandler(ResetHandler, busIdOption, deviceAddressOption);

            #endregion

            gpioExpander.AddGlobalOption(busIdOption);
            gpioExpander.AddGlobalOption(deviceAddressOption);

            gpioExpander.AddCommand(adc);
            gpioExpander.AddCommand(pwm);
            gpioExpander.AddCommand(port);
            gpioExpander.AddCommand(anlog);
            gpioExpander.AddCommand(reset);
            gpioExpander.AddCommand(address);
            gpioExpander.AddCommand(digital);
            gpioExpander.AddCommand(pwmRandom);

            root.AddCommand(gpioExpander);
        }

        #region Handlers

        private static async Task PortHandler(int busId, int deviceAddress, int delay)
        {
            await using (IGpioExpander expander = await AmperkaDevices.CreateGpioExpanderAsync(new I2cConnectionSettings(busId, deviceAddress)))
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

        private static async Task DigitalHandler(int busId, int deviceAddress, int readPin, int writePin)
        {
            await using (IGpioExpander expander = await AmperkaDevices.CreateGpioExpanderAsync(new I2cConnectionSettings(busId, deviceAddress)))
            {
                Console.WriteLine("Checking the digital signal.");

                await expander.PinModeAsync(readPin, PinMode.Input);
                await expander.PinModeAsync(writePin, PinMode.Output);

                while (Exit())
                {
                    bool result = await expander.TroykaButtonClickAsync(readPin);

                    Console.WriteLine("Result: {0}", result);

                    expander.DigitalWrite(writePin, result);
                }
            }
        }

        private static async Task AnalogHandler(int busId, int deviceAddress, int readPin, int writePin, bool @readonly)
        {
            await using (IGpioExpander expander = await AmperkaDevices.CreateGpioExpanderAsync(new I2cConnectionSettings(busId, deviceAddress)))
            {
                Console.WriteLine("Checking the analog signal.");

                if (@readonly)
                {
                    while (Exit())
                    {
                        double result = await expander.AnalogReadAsync(readPin) / 4095.0;

                        Console.WriteLine("Result: {0}", result);
                    }
                }
                else
                {
                    while (Exit())
                    {
                        int result = await expander.AnalogReadAsync(readPin);

                        Console.WriteLine("Result: {0}", result);

                        await expander.AnalogWriteAsync(writePin, result, ScaleMode.ADC);
                    }
                }
            }
        }

        private static async Task PwmRandomHandler(int busId, int deviceAddress, int freq, int writePin, int delay)
        {
            await using (IGpioExpander expander = await AmperkaDevices.CreateGpioExpanderAsync(new I2cConnectionSettings(busId, deviceAddress)))
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

        private static async Task PwmHandler(int busId, int deviceAddress, int freq, int writePin, int pwm, int delay)
        {
            await using (IGpioExpander expander = await AmperkaDevices.CreateGpioExpanderAsync(new I2cConnectionSettings(busId, deviceAddress)))
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
            await using (IGpioExpander expander = await AmperkaDevices.CreateGpioExpanderAsync(new I2cConnectionSettings(busId, deviceAddress)))
            {
                Console.WriteLine("Checking the ADC speed ({0}).", adcSpeed);

                await expander.AdcSpeedAsync(adcSpeed);
            }
        }

        private static async Task AddressHandler(int busId, int deviceAddress, int newAddress)
        {
            await using (IGpioExpander expander = await AmperkaDevices.CreateGpioExpanderAsync(new I2cConnectionSettings(busId, deviceAddress)))
            {
                Console.WriteLine("Checking the chip change and save address ({0}).", newAddress);

                await expander.ChangeAddressAsync(newAddress);
                await expander.SaveAddressAsync();
            }
        }

        private static async Task ResetHandler(int busId, int deviceAddress)
        {
            await using (IGpioExpander expander = await AmperkaDevices.CreateGpioExpanderAsync(new I2cConnectionSettings(busId, deviceAddress)))
            {
                Console.WriteLine("Checking the chip reset.");

                await expander.ResetAsync();
            }
        }

        #endregion
    }
}
