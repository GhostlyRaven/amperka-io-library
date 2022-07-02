using System.Device.I2c;
using Amperka.IO.Exceptions.Internal;

// ReSharper disable All

namespace Amperka.IO.Devices
{
    public static partial class AmperkaDevices
    {
        #region GPIO expander creation functions

        /// <summary>
        /// Initializes a new IGpioExpander.
        /// </summary>
        /// <returns>Instance of IGpioExpander.</returns>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The device address on the bus or the bus id is in an invalid range.</exception>
        public static IGpioExpander CreateGpioExpander()
        {
            return CreateGpioExpander(new I2cConnectionSettings(1, 42));
        }

        /// <summary>
        /// Initializes a new IGpioExpander.
        /// </summary>
        /// <param name="settings">I2C bus settings on this device.</param>
        /// <returns>Instance of IGpioExpander.</returns>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentNullException">The bus settings object can't be a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The device address on the bus or the bus id is in an invalid range.</exception>
        public static IGpioExpander CreateGpioExpander(I2cConnectionSettings settings)
        {
            if (settings is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(settings));
            }

            if (settings.BusId < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(settings.BusId));
            }

            if (settings.DeviceAddress is < 0 or > 127)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(settings.DeviceAddress));
            }

            try
            {
                return new GpioExpander.Internal.GpioExpander(I2cDevice.Create(settings));
            }
            catch (Exception error)
            {
                throw ThrowHelper.GetAmperkaDeviceException(error);
            }
        }

        /// <summary>
        /// Async initializes a new IGpioExpander.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>Instance of IGpioExpander.</returns>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The device address on the bus or the bus id is in an invalid range.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public static ValueTask<IGpioExpander> CreateGpioExpanderAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return ValueTask.FromResult(CreateGpioExpander());
        }

        /// <summary>
        /// Async initializes a new IGpioExpander.
        /// </summary>
        /// <param name="settings">I2C bus settings on this device.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>Instance of IGpioExpander.</returns>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentNullException">The bus settings object can't be a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The device address on the bus or the bus id is in an invalid range.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public static ValueTask<IGpioExpander> CreateGpioExpanderAsync(I2cConnectionSettings settings, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return ValueTask.FromResult(CreateGpioExpander(settings));
        }

        #endregion
    }
}
