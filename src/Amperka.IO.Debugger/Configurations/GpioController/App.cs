using System.CommandLine;
using System.Device.Gpio;
using Amperka.IO.Devices;
using Amperka.IO.Extensions;

// ReSharper disable All

namespace Amperka.IO.Debugger.Configurations
{
    internal static partial class App
    {
        private static void ConfigureGpioControllerCommand(Command root)
        {
            Command gpioController = new Command("gpio-controller", "Checking the GPIO controller functions.");

            #region Options

            Option<int> delayOption = new Option<int>("--delay", () => 25);
            Option<int> readPinOption = new Option<int>("--read-pin", () => 0);
            Option<int> writePinOption = new Option<int>("--write-pin", () => 1);
            Option<bool> useBcmOption = new Option<bool>("--use-bcm", () => false);

            #endregion

            #region Convert command

            Command convertCommand = new Command("convert-pins", "Checking the convert pins.");

            convertCommand.SetHandler(ConvertPinsHandler, readPinOption, writePinOption, useBcmOption);

            #endregion

            #region Sync command

            Command syncCommand = new Command("sync-button-click", "Checking the sync button click.")
            {
                delayOption
            };

            syncCommand.SetHandler(SyncButtonClickHandler, readPinOption, writePinOption, delayOption, useBcmOption);

            #endregion

            #region Async command

            Command asyncCommand = new Command("async-button-click", "Checking the async button click.")
            {
                delayOption
            };

            asyncCommand.SetHandler(AsyncButtonClickHandler, readPinOption, writePinOption, delayOption, useBcmOption);

            #endregion

            gpioController.AddGlobalOption(useBcmOption);
            gpioController.AddGlobalOption(readPinOption);
            gpioController.AddGlobalOption(writePinOption);

            gpioController.AddCommand(convertCommand);
            gpioController.AddCommand(syncCommand);
            gpioController.AddCommand(asyncCommand);

            root.AddCommand(gpioController);
        }

        #region Handlers

        private static Task ConvertPinsHandler(int readPin, int writePin, bool useBcm)
        {
            if (useBcm)
            {
                Console.WriteLine("WiringPi read pin: {0}. BCM read pin: {1}", WiringPi.FromBcm(readPin), readPin);
                Console.WriteLine("WiringPi write pin: {0}. BCM write pin: {1}", WiringPi.FromBcm(writePin), writePin);
            }
            else
            {
                Console.WriteLine("BCM read pin: {0}. WiringPi read pin: {1}", WiringPi.ToBcm(readPin), readPin);
                Console.WriteLine("BCM write pin: {0}. WiringPi write pin: {1}", WiringPi.ToBcm(writePin), writePin);
            }

            return Task.CompletedTask;
        }

        private static async Task SyncButtonClickHandler(int readPin, int writePin, int delay, bool useBcm)
        {
            int bcmReadPin = useBcm ? readPin : WiringPi.FromBcm(readPin);
            int bcmWritePin = useBcm ? writePin : WiringPi.FromBcm(writePin);

            using (GpioController controller = new GpioController())
            {
                controller.OpenPin(bcmReadPin, PinMode.Input);
                controller.OpenPin(bcmWritePin, PinMode.Output);

                while (Exit())
                {
                    bool result = controller.TroykaButtonClick(bcmReadPin);

                    Console.WriteLine("Result: {0}", result);

                    controller.Write(bcmWritePin, result);

                    await Task.Delay(delay);
                }
            }
        }

        private static async Task AsyncButtonClickHandler(int readPin, int writePin, int delay, bool useBcm)
        {
            int bcmReadPin = useBcm ? readPin : WiringPi.FromBcm(readPin);
            int bcmWritePin = useBcm ? writePin : WiringPi.FromBcm(writePin);

            using (GpioController controller = new GpioController())
            {
                controller.OpenPin(bcmReadPin, PinMode.Input);
                controller.OpenPin(bcmWritePin, PinMode.Output);

                while (Exit())
                {
                    bool result = await controller.TroykaButtonClickAsync(bcmReadPin);

                    Console.WriteLine("Result: {0}", result);

                    controller.Write(bcmWritePin, result);

                    await Task.Delay(delay);
                }
            }
        }

        #endregion
    }
}
