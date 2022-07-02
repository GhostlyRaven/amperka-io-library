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
            Option<bool> bcmOption = new Option<bool>("--bcm", () => false);
            Option<int> readPinOption = new Option<int>("--read-pin", () => 0);
            Option<int> writePinOption = new Option<int>("--write-pin", () => 1);

            #endregion

            #region Convert command

            Command convertCommand = new Command("convert-pins", "Checking the convert pins.");

            convertCommand.SetHandler(ConvertPinsHandler, readPinOption, writePinOption, delayOption, bcmOption);

            #endregion

            #region Sync command

            Command syncCommand = new Command("sync-button-click", "Checking the sync button click.");

            syncCommand.SetHandler(SyncButtonClickHandler, readPinOption, writePinOption, delayOption, bcmOption);

            #endregion

            #region Async command

            Command asyncCommand = new Command("async-button-click", "Checking the async button click.");

            asyncCommand.SetHandler(AsyncButtonClickHandler, readPinOption, writePinOption, delayOption, bcmOption);

            #endregion

            root.AddGlobalOption(bcmOption);
            root.AddGlobalOption(delayOption);
            root.AddGlobalOption(readPinOption);
            root.AddGlobalOption(writePinOption);

            gpioController.AddCommand(convertCommand);
            gpioController.AddCommand(syncCommand);
            gpioController.AddCommand(asyncCommand);

            root.AddCommand(gpioController);
        }

        #region Handlers

        private static async Task ConvertPinsHandler(int readPin, int writePin, int delay, bool bcm)
        {
            if (bcm)
            {
                Console.WriteLine("WiringPi read pin: {0}. BCM read pin: {1}", AmperkaDevices.BcmToWiringPi(readPin), readPin);
                Console.WriteLine("WiringPi write pin: {0}. BCM write pin: {1}", AmperkaDevices.BcmToWiringPi(writePin), writePin);
            }
            else
            {
                Console.WriteLine("BCM read pin: {0}. WiringPi read pin: {1}", AmperkaDevices.WiringPiToBcm(readPin), readPin);
                Console.WriteLine("BCM write pin: {0}. WiringPi write pin: {1}", AmperkaDevices.WiringPiToBcm(writePin), writePin);
            }

            while (Exit())
            {
                await Task.Delay(delay);
            }
        }

        private static async Task SyncButtonClickHandler(int readPin, int writePin, int delay, bool bcm)
        {
            int bcmReadPin = bcm ? readPin : AmperkaDevices.WiringPiToBcm(readPin);
            int bcmWritePin = bcm ? writePin : AmperkaDevices.WiringPiToBcm(writePin);

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

        private static async Task AsyncButtonClickHandler(int readPin, int writePin, int delay, bool bcm)
        {
            int bcmReadPin = bcm ? readPin : AmperkaDevices.WiringPiToBcm(readPin);
            int bcmWritePin = bcm ? writePin : AmperkaDevices.WiringPiToBcm(writePin);

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
