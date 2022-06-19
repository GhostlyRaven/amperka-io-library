using System.Device.I2c;
using Amperka.IO.Exceptions.Internal;

// ReSharper disable All

namespace Amperka.IO.Devices
{
    //Save link in other place. Methods for thermometer: https://www.circuitbasics.com/raspberry-pi-ds18b20-temperature-sensor-tutorial/

    /// <summary>
    /// A class providing devices from Amperka.
    /// </summary>
    public static class AmperkaDevices
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

        #region I2C hub creation functions

        /// <summary>
        /// Initializes a new II2CHub.
        /// </summary>
        /// <returns>Instance of II2CHub.</returns>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The device address on the bus or the bus id is in an invalid range.</exception>
        public static II2CHub CreateI2CHub()
        {
            return CreateI2CHub(new I2cConnectionSettings(1, 112));
        }

        /// <summary>
        /// Initializes a new II2CHub.
        /// </summary>
        /// <param name="settings">I2C bus settings on this device.</param>
        /// <returns>Instance of II2CHub.</returns>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentNullException">The bus settings object can't be a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The device address on the bus or the bus id is in an invalid range.</exception>
        public static II2CHub CreateI2CHub(I2cConnectionSettings settings)
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
                return new I2CHub.Internal.I2CHub(I2cDevice.Create(settings));
            }
            catch (Exception error)
            {
                throw ThrowHelper.GetAmperkaDeviceException(error);
            }
        }

        /// <summary>
        /// Async initializes a new II2CHub.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>Instance of II2CHub.</returns>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The device address on the bus or the bus id is in an invalid range.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public static ValueTask<II2CHub> CreateI2CHubAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return ValueTask.FromResult(CreateI2CHub());
        }

        /// <summary>
        /// Async initializes a new II2CHub.
        /// </summary>
        /// <param name="settings">I2C bus settings on this device.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>Instance of II2CHub.</returns>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentNullException">The bus settings object can't be a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The device address on the bus or the bus id is in an invalid range.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public static ValueTask<II2CHub> CreateI2CHubAsync(I2cConnectionSettings settings, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return ValueTask.FromResult(CreateI2CHub(settings));
        }

        #endregion
    }
}
