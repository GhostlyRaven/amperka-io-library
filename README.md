# <img src="/images/amperka-logo-32.png"></img> **Amperka.IO.Library**

**Implementation of the library for devices from Amperka on the dotnet platform and Raspberry Pi.**

**Target framework – .NET 6.**

# Usage example

```csharp
using System.Device.I2c;
using Amperka.IO.Devices;

namespace Amperka.IO.Demo
{
    internal static class Program
    {
        internal static async Task<int> Main()
        {
            using (I2CHub hub = new I2CHub(I2cDevice.Create(new I2cConnectionSettings(1, I2CHub.DefaultAddress))))
            {
                Console.WriteLine("Checking the set channel (0).");

                hub.SetChannel(0);
            }

            await using (I2CHub hub = new I2CHub(I2cDevice.Create(new I2cConnectionSettings(1, I2CHub.DefaultAddress))))
            {
                Console.WriteLine("Checking the set channel (1).");

                await hub.SetChannelAsync(1);
            }

            return 0;
        }
    }
}
```

# Development plan
The list contains the devices expected in the future in the library:
- [x] [Troyka CAP/HAT;](https://github.com/amperka/TroykaHatCpp)
- [x] [GPIO Expander;](https://github.com/amperka/I2CioExpander)
- [x] [P-FET/N-FET;](https://github.com/amperka/AmperkaFet)
- [x] [I2C Hub.](https://github.com/amperka/TroykaI2CHub)

# Additional devices
The list contains the devices available in the debugging application:
- [x] [SHT3X;](https://github.com/dotnet/iot/tree/main/src/devices/Sht3x)
- [x] [LPS25H;](https://github.com/dotnet/iot/tree/main/src/devices/Lps25h)
- [x] [Rotary encoder;](https://github.com/dotnet/iot/tree/main/src/devices/RotaryEncoder)
- [x] [OneWire thermometer.](https://github.com/dotnet/iot/tree/main/src/devices/OneWire)

# Additional information

* [Amperka on GitHub.](https://github.com/amperka)
* [Official website of Amperka.](https://amperka.com/)
