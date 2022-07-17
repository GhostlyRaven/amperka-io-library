using System.Device.Gpio;
using Amperka.IO.Devices;
using System.CommandLine;
using Iot.Device.RotaryEncoder;

// ReSharper disable All

namespace Amperka.IO.Debugger.Configurations
{
    internal static partial class App
    {
        private static void ConfigureRotaryEncoderCommand(Command root)
        {
            Command rotaryEncoderCommand = new Command("rotary-encoder", "Checking the rotary encoder functions.");

            #region Options

            Option<int> delayOption = new Option<int>("--delay", () => 25);
            Option<int> readPinAOption = new Option<int>("--read-pin-a", () => 0);
            Option<int> readPinBOption = new Option<int>("--read-pin-b", () => 1);
            Option<bool> useBcmOption = new Option<bool>("--use-bcm", () => false);
            Option<double> debounceOption = new Option<double>("--debounce", () => 2);
            Option<int> pulsesPerRotationOption = new Option<int>("--pulses-per-rotation", () => 25);
            Option<PinEventTypes> pinEventTypesOption = new Option<PinEventTypes>("--pin-event-types", () => PinEventTypes.Falling);

            #endregion

            #region Listen event command

            Command listenEventCommand = new Command("listen-event", "Checking the listen event.");

            listenEventCommand.SetHandler(ListenEventHandler, readPinAOption, readPinBOption, pinEventTypesOption, pulsesPerRotationOption, debounceOption, delayOption, useBcmOption);

            #endregion

            rotaryEncoderCommand.AddGlobalOption(delayOption);
            rotaryEncoderCommand.AddGlobalOption(useBcmOption);
            rotaryEncoderCommand.AddGlobalOption(debounceOption);
            rotaryEncoderCommand.AddGlobalOption(readPinAOption);
            rotaryEncoderCommand.AddGlobalOption(readPinBOption);
            rotaryEncoderCommand.AddGlobalOption(pinEventTypesOption);
            rotaryEncoderCommand.AddGlobalOption(pulsesPerRotationOption);

            rotaryEncoderCommand.AddCommand(listenEventCommand);

            root.AddCommand(rotaryEncoderCommand);
        }

        #region Handlers

        private static async Task ListenEventHandler(int readPinA, int readPinB, PinEventTypes pinEventTypes, int pulsesPerRotation, double debounce, int delay, bool useBcm)
        {
            int bcmReadPinA = useBcm ? readPinA : WiringPi.ToBcm(readPinA);
            int bcmReadPinB = useBcm ? readPinB : WiringPi.ToBcm(readPinB);

            using (ScaledQuadratureEncoder encoder = new ScaledQuadratureEncoder(bcmReadPinA, bcmReadPinB, pinEventTypes, pulsesPerRotation, 1, 0, 255))
            {
                encoder.Debounce = TimeSpan.FromMilliseconds(debounce);

                encoder.ValueChanged += EventHandler;

                while (Exit())
                {
                    await Task.Delay(delay);
                }

                encoder.ValueChanged -= EventHandler;

                void EventHandler(object _, RotaryEncoderEventArgs args)
                {
                    Console.WriteLine("Result: {0}", args.Value);
                }
            }
        }

        #endregion
    }
}
