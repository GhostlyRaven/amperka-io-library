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

            Command port = new Command("port", "Checking the port signal.")
            {
                delayOption
            };

            port.SetHandler(PortHandler, busIdOption, deviceAddressOption, delayOption);

            #endregion

            #region Digital command

            Command digital = new Command("digital", "Checking the digital signal.")
            {
                delayOption,
                readPinOption,
                writePinOption
            };

            digital.SetHandler(DigitalHandler, busIdOption, deviceAddressOption, delayOption, writePinOption, readPinOption);

            #endregion

            #region Analog command

            Command anlog = new Command("analog", "Checking the analog signal.")
            {
                delayOption,
                readPinOption,
                writePinOption,
                useReadonlyOption
            };

            anlog.SetHandler(AnalogHandler, busIdOption, deviceAddressOption, delayOption, writePinOption, readPinOption, useReadonlyOption);

            #endregion

            #region Pwm random command

            Command pwmRandom = new Command("pwm-random".ToLower(), "Checking the PWM signal.")
            {
                freqOption,
                delayOption,
                writePinOption
            };

            pwmRandom.SetHandler(PwmRandomHandler, busIdOption, deviceAddressOption, delayOption, writePinOption, freqOption);

            #endregion

            #region Pwm command

            Command pwm = new Command("pwm", "Checking the PWM signal.")
            {
                pwmOption,
                freqOption,
                delayOption,
                writePinOption
            };

            pwm.SetHandler(PwmHandler, busIdOption, deviceAddressOption, delayOption, writePinOption, freqOption, pwmOption);

            #endregion

            #region Adc command

            Command adc = new Command("adc", "Checking the ADC speed.")
            {
                adcSpeedOption
            };

            adc.SetHandler(AdcHandler, busIdOption, deviceAddressOption, adcSpeedOption);

            #endregion

            #region Change address command

            Command changeChipAddress = new Command("change-chip-address", "Checking the chip change address.")
            {
                newDeviceAddressOption
            };

            changeChipAddress.SetHandler(ChangeChipAddressHandler, busIdOption, deviceAddressOption, newDeviceAddressOption);

            #endregion

            #region Save address command

            Command saveChipAddress = new Command("save-chip-address", "Checking the chip save address.");

            saveChipAddress.SetHandler(SaveAddressHandler, busIdOption, deviceAddressOption);

            #endregion

            #region Reset chip command

            Command resetChip = new Command("reset-chip", "Checking the chip reset.");

            resetChip.SetHandler(ResetChipHandler, busIdOption, deviceAddressOption);

            #endregion

            gpioExpander.AddGlobalOption(busIdOption);
            gpioExpander.AddGlobalOption(deviceAddressOption);

            gpioExpander.AddCommand(adc);
            gpioExpander.AddCommand(pwm);
            gpioExpander.AddCommand(port);
            gpioExpander.AddCommand(anlog);
            gpioExpander.AddCommand(digital);
            gpioExpander.AddCommand(pwmRandom);
            gpioExpander.AddCommand(resetChip);
            gpioExpander.AddCommand(saveChipAddress);
            gpioExpander.AddCommand(changeChipAddress);

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
                        double result = await expander.AnalogReadAsync(readPin) / 4095.0;

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
