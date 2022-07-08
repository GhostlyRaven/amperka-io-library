using System.Device.I2c;
using System.Diagnostics;
using Amperka.IO.Exceptions;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

// ReSharper disable All

namespace Amperka.IO.Devices
{
    /// <summary>
    /// A class for working with I2C Hub.
    /// </summary>
    public sealed class I2CHub : IAsyncDisposable, IDisposable
    {
        #region Constructors and fields

        private bool _disposed;
        private I2cDevice _device;

        /// <summary>
        /// Default device address.
        /// </summary>
        public const int DefaultAddress = 112;

        /// <summary>
        /// Initializes a new instance of the <see cref="I2CHub"/> class.
        /// </summary>
        /// <param name="device">Instance of I2C device.</param>
        /// <exception cref="ArgumentNullException">The I2C device object can't be a null reference.</exception>
        public I2CHub(I2cDevice device)
        {
            _device = device ?? throw ThrowHelper.GetArgumentNullException(nameof(device));
        }

        #endregion

        #region Channel functions

        /// <summary>
        /// Switches the I2C hub to the selected channel.
        /// </summary>
        /// <param name="channel">Channel number.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value of the channel.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public void SetChannel(int channel)
        {
            if (channel is < 0 or > 7)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(channel));
            }

            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(I2CHub));

            Write(channel, false);
        }

        /// <summary>
        /// Async switches the I2C hub to the selected channel.
        /// </summary>
        /// <param name="channel">Channel number.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value of the channel.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public ValueTask SetChannelAsync(int channel, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            SetChannel(channel);

            return default;
        }

        #endregion

        #region Private functions

        [StackTraceHidden]
        private void Write(int channel, bool ignoreThrow)
        {
            try
            {
                Span<byte> writeBuffer = stackalloc byte[sizeof(byte)];

                Unsafe.As<byte, int>(ref MemoryMarshal.GetReference(writeBuffer)) = channel | 8;

                _device.Write(writeBuffer);
            }
            catch (Exception error)
            {
                ThrowHelper.ThrowAmperkaDeviceException(ignoreThrow, error);
            }
        }

        #endregion

        #region IAsyncDisposable and IDisposable

        private void Shutdown(bool disposing)
        {
            Write(0, true);

            if (disposing)
            {
                _device.Dispose();
            }

            _device = default;

            _disposed = true;
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            Shutdown(disposing);
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            Dispose();

            return default;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        ~I2CHub()
        {
            Dispose(false);
        }

        #endregion
    }
}
