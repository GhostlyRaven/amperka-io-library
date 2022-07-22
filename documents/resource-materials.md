# Resource materials for the project

List of links to resource materials:
1. [SHT3X;](https://github.com/dotnet/iot/tree/main/src/devices/Sht3x)
2. [LPS25H;](https://github.com/dotnet/iot/tree/main/src/devices/Lps25h)
3. [OneWire;](https://github.com/dotnet/iot/tree/main/src/devices/OneWire)
4. [IoT in C#;](https://github.com/dotnet/iot)
5. [GPIO pin convert;](https://github.com/davidjalbers/GPIOPinConvert)
6. [ThrowHelper example;](https://github.com/dotnet/runtime/blob/215b39abf947da7a40b0cb137eab4bceb24ad3e3/src/libraries/System.Private.CoreLib/src/System/ThrowHelper.cs)
7. [Quadrature rotary encoder;](https://github.com/dotnet/iot/tree/main/src/devices/RotaryEncoder)
8. [I2C IO Firmware repository;](https://github.com/amperka/i2cio-firmware)
9. [DS18B20 temperature sensor;](https://www.circuitbasics.com/raspberry-pi-ds18b20-temperature-sensor-tutorial)
10. [Troyka Hat Python repository.](https://github.com/amperka/TroykaHatPython)

# Checking the calculations of the counter period and the restored frequency for STM32

Formulas:
1. $$ Freq = \frac {TimeClock}{Prescaler * CounterPeriod} $$
2. $$ Prescaler = \frac {TimeClock}{Freq * CounterPeriod} $$
3. $$ CounterPeriod = \frac {TimeClock}{Prescaler * Freq} $$
4. $$ CounterPeriod = \frac {48.000.000}{24 * Freq} $$

Where **TimeClock** - frequency of the device timer operation, **Prescaler** - prescaler for **TimeClock**, **CounterPeriod (ARR-register)** - counter overflow value, **Freq** - generated frequency.

```csharp
public static class STM32
{
    private static readonly int Digits = 3; //Number of decimal places.

    private static readonly uint TimeClock = 2_000_000U; //2 MHz.

    public static void CalculateParameters()
    {
        while (Exit())
        {
            Console.Write("Enter the estimated frequency: ");

            if (ushort.TryParse(Console.ReadLine(), out ushort freq))
            {
                try
                {
                    ushort counterPeriod = (ushort)(TimeClock / freq);

                    double restoredFreq = Math.Round(1.0 / ((double)counterPeriod / TimeClock), Digits);

                    Console.WriteLine($"Counter period: {counterPeriod} | Restored freq: {restoredFreq} Hz{Environment.NewLine}");

                    Task.Delay(1000).Wait();
                }
                catch (Exception error)
                {
                    ConsoleColor oldColor = Console.ForegroundColor;

                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine($"{error}{Environment.NewLine}");

                    Console.ForegroundColor = oldColor;
                }
            }
        }
    }

    private static bool Exit()
    {
        return !(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape);
    }
}
```
