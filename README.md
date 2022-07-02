# <img src="/images/amperka-logo-32.png"></img> **Amperka.IO.Library**

**Implementation of the library for devices from Amperka on the dotnet platform.**

**Target framework – .NET 6.**

# Usage example

```csharp
    internal static class Program
    {
        internal static void Main()
        {
            await using (II2CHub hub = await AmperkaDevices.CreateI2CHub())
            {
                Console.WriteLine("Checking the set channel (0).");

                await hub.SetChannel(0);
            }

            await using (II2CHub hub = await AmperkaDevices.CreateI2CHubAsync())
            {
                Console.WriteLine("Checking the set channel (1).");

                await hub.SetChannelAsync(1);
            }
        }
    }
```

# Development plan
The list contains the devices expected in the future in the library:
- [ ] [Digital weather sensor;](https://github.com/amperka/TroykaMeteoSensor)
- [x] [Troyka CAP/HAT;](https://github.com/amperka/TroykaHatCpp)
- [x] [GPIO Expander;](https://github.com/amperka/I2CioExpander)
- [ ] [Barometer V2;](https://github.com/amperka/Troyka-IMU)
- [ ] [P-FET/N-FET;](https://github.com/amperka/AmperkaFet)
- [x] [I2C Hub.](https://github.com/amperka/TroykaI2CHub)

# Additional information

* [Amperka on GitHub.](https://github.com/amperka)
* [Official website of Amperka.](https://amperka.com/)
