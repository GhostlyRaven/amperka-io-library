using System.Device.Gpio;
using Amperka.IO.Exceptions;

// ReSharper disable All

namespace Amperka.IO.Extensions
{
    /// <summary>
    /// Provides additional functions for working with the <see cref="GpioController"/> class.
    /// </summary>
    public static class GpioControllerExtensions
    {
        /// <summary>
        /// Fixes click of the TroykaButton.
        /// </summary>
        /// <param name="controller">Instance of <see cref="GpioController"/> class.</param>
        /// <param name="pin">Pin number on the device.</param>
        /// <returns>True - click, False - not click.</returns>
        /// <exception cref="ArgumentNullException">The GPIO controller object can't be a null reference.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        public static bool TroykaButtonClick(this GpioController controller, int pin)
        {
            if (controller is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(controller));
            }

            try
            {
                return controller.Read(pin) == PinValue.Low;
            }
            catch (Exception error)
            {
                throw ThrowHelper.GetAmperkaDeviceException(error);
            }
        }

        /// <summary>
        /// Async fixes click of the TroykaButton.
        /// </summary>
        /// <param name="controller">Instance of <see cref="GpioController"/> class.</param>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>True - click, False - not click.</returns>
        /// <exception cref="ArgumentNullException">The GPIO controller object can't be a null reference.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public static ValueTask<bool> TroykaButtonClickAsync(this GpioController controller, int pin, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return ValueTask.FromResult(controller.TroykaButtonClick(pin));
        }
    }
}
