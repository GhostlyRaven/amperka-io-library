﻿// ReSharper disable All

namespace Amperka.IO.Devices.GpioExpander
{
    /// <summary>
    /// Pin mode.
    /// </summary>
    public enum PinMode
    {
        /// <summary>
        /// Output.
        /// </summary>
        Output = 0,

        /// <summary>
        /// Input.
        /// </summary>
        Input = 1,

        /// <summary>
        /// Input pull up.
        /// </summary>
        InputPullUp = 2,

        /// <summary>
        /// Input pull down.
        /// </summary>
        InputPullDown = 3
    }
}
