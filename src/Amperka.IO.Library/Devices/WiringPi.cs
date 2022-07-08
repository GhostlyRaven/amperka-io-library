using Amperka.IO.Exceptions;

// ReSharper disable All

namespace Amperka.IO.Devices
{
    /// <summary>
    /// A class for working with WiringPi pins.
    /// </summary>
    public static class WiringPi
    {
        #region Convert pin functions

        /// <summary>
        /// Convert Wiring Pi pin to Bcm pin on the device.
        /// </summary>
        /// <param name="pin">Wiring Pi pin number on the device.</param>
        /// <returns>Bcm pin number on the device.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Invalid pin number value on the device.</exception>
        public static int ToBcm(int pin)
        {
            return pin switch
            {
                0 => 17,
                1 => 18,
                2 => 27,
                3 => 22,
                4 => 23,
                5 => 24,
                6 => 25,
                7 => 4,
                8 => 2,
                9 => 3,
                10 => 8,
                11 => 7,
                12 => 10,
                13 => 9,
                14 => 11,
                15 => 14,
                16 => 15,
                21 => 5,
                22 => 6,
                23 => 13,
                24 => 19,
                25 => 26,
                26 => 12,
                27 => 16,
                28 => 20,
                29 => 21,
                30 => 0,
                31 => 1,
                _ => throw ThrowHelper.GetArgumentOutOfRangeException(nameof(pin))
            };
        }

        /// <summary>
        /// Convert Wiring Pi pin to Bcm pin on the device.
        /// </summary>
        /// <param name="pin">Bcm pin number on the device.</param>
        /// <returns>Wiring Pi pin number on the device.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Invalid pin number value on the device.</exception>
        public static int FromBcm(int pin)
        {
            return pin switch
            {
                0 => 30,
                1 => 31,
                2 => 8,
                3 => 9,
                4 => 7,
                5 => 21,
                6 => 22,
                7 => 11,
                8 => 10,
                9 => 13,
                10 => 12,
                11 => 14,
                12 => 26,
                13 => 23,
                14 => 15,
                15 => 16,
                16 => 27,
                17 => 0,
                18 => 1,
                19 => 24,
                20 => 28,
                21 => 29,
                22 => 3,
                23 => 4,
                24 => 5,
                25 => 6,
                26 => 25,
                27 => 2,
                _ => throw ThrowHelper.GetArgumentOutOfRangeException(nameof(pin))
            };
        }

        #endregion
    }
}
