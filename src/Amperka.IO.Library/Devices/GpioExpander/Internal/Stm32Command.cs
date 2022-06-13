// ReSharper disable All

namespace Amperka.IO.Devices.GpioExpander.Internal
{
    internal enum Stm32Command
    {
        Reset = 1,
        ChangeI2CAddress = 2,
        SaveI2CAddress = 3,
        PortModeInput = 4,
        PortModePullUp = 5,
        PortModePullDown = 6,
        PortModeOutput = 7,
        DigitalRead = 8,
        DigitalWriteHigh = 9,
        DigitalWriteLow = 10,
        AnalogWrite = 11,
        AnalogRead = 12,
        PwmFreq = 13,
        AdcSpeed = 14
    }
}
