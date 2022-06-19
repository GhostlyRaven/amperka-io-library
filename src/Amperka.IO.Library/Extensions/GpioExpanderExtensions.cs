using Amperka.IO.Devices;
using Amperka.IO.Exceptions.Internal;

// ReSharper disable All

namespace Amperka.IO.Extensions
{
    /// <summary>
    /// Provides additional functions for working with the GPIO expander.
    /// </summary>
    public static class GpioExpanderExtensions
    {
        /// <summary>
        /// Fixes click of the TroykaButton.
        /// </summary>
        /// <param name="expander">Instance of IGpioExpander.</param>
        /// <param name="pin">Pin number on the device.</param>
        /// <returns>True - click, False - not click.</returns>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentNullException">The GPIO expander object can't be a null reference.</exception>
        public static bool TroykaButtonClick(this IGpioExpander expander, int pin)
        {
            if (expander is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(expander));
            }

            return !expander.DigitalRead(pin);
        }

        /// <summary>
        /// Async fixes click of the TroykaButton.
        /// </summary>
        /// <param name="expander">Instance of IGpioExpander.</param>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>True - click, False - not click.</returns>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentNullException">The GPIO expander object can't be a null reference.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public static async ValueTask<bool> TroykaButtonClickAsync(this IGpioExpander expander, int pin, CancellationToken cancellationToken = default)
        {
            if (expander is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(expander));
            }

            return !await expander.DigitalReadAsync(pin, cancellationToken);
        }

        /// <summary>
        /// Sets all pins to high level.
        /// </summary>
        /// <param name="expander">Instance of IGpioExpander.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentNullException">The GPIO expander object can't be a null reference.</exception>
        public static void DigitalPortHighLevel(this IGpioExpander expander)
        {
            if (expander is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(expander));
            }

            expander.DigitalWritePort(1023);
        }

        /// <summary>
        /// Async sets all pins to high level.
        /// </summary>
        /// <param name="expander">Instance of IGpioExpander.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentNullException">The GPIO expander object can't be a null reference.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public static async ValueTask DigitalPortHighLevelAsync(this IGpioExpander expander, CancellationToken cancellationToken = default)
        {
            if (expander is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(expander));
            }

            await expander.DigitalWritePortAsync(1023, cancellationToken);
        }

        /// <summary>
        /// Sets all pins to low level.
        /// </summary>
        /// <param name="expander">Instance of IGpioExpander.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentNullException">The GPIO expander object can't be a null reference.</exception>
        public static void DigitalPortLowLevel(this IGpioExpander expander)
        {
            if (expander is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(expander));
            }

            expander.DigitalWritePort(0);
        }

        /// <summary>
        /// Async sets all pins to low level.
        /// </summary>
        /// <param name="expander">Instance of IGpioExpander.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentNullException">The GPIO expander object can't be a null reference.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public static async ValueTask DigitalPortLowLevelAsync(this IGpioExpander expander, CancellationToken cancellationToken = default)
        {
            if (expander is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(expander));
            }

            await expander.DigitalWritePortAsync(0, cancellationToken);
        }
    }
}
