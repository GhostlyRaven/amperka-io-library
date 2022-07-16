using System.Device.Spi;
using System.Diagnostics;
using Amperka.IO.Exceptions;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

// ReSharper disable All

namespace Amperka.IO.Devices
{
    /// <summary>
    /// A class for working with P-FET and N-FET.
    /// </summary>
    public sealed class XFet : IAsyncDisposable, IDisposable
    {
        #region Constructors and fields

        private bool _disposed;
        private SpiDevice _device;
        private Memory<int> _values;

        private readonly int _valuesCount;

        /// <summary>
        /// The count of pins on the device.
        /// </summary>
        public const int PinCount = 8;

        /// <summary>
        /// Initializes a new instance of the <see cref="XFet"/> class.
        /// </summary>
        /// <param name="device">Instance of SPI device.</param>
        /// <param name="count">The count of devices connected in a chain.</param>
        /// <exception cref="ArgumentNullException">The SPI device object can't be a null reference.</exception>
        public XFet(SpiDevice device, int count = 1)
        {
            _device = device ?? throw ThrowHelper.GetArgumentNullException(nameof(device));

            _valuesCount = count > 0 ? count : 1;

            _values = new int[_valuesCount];
        }

        /// <summary>
        /// The count of devices connected in a chain.
        /// </summary>
        public int DeviceCount => _valuesCount;

        #endregion

        #region Digital read functions

        /// <summary>
        /// Reads the signal level from all pin. Designed to work with a single connected device.
        /// </summary>
        /// <returns>High or low signal level.</returns>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public bool DigitalRead()
        {
            return DigitalReadMany(0);
        }

        /// <summary>
        /// Async reads the signal level from all pin. Designed to work with a single connected device.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>High or low signal level.</returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public ValueTask<bool> DigitalReadAsync(CancellationToken cancellationToken = default)
        {
            return DigitalReadManyAsync(0, cancellationToken);
        }

        /// <summary>
        /// Reads the signal level from selected pin. Designed to work with a single connected device.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <returns>High or low signal level.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Invalid pin number value on the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public bool DigitalRead(int pin)
        {
            return DigitalReadMany(0, pin);
        }

        /// <summary>
        /// Async reads the signal level from selected pin. Designed to work with a single connected device.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>High or low signal level.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Invalid pin number value on the device.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public ValueTask<bool> DigitalReadAsync(int pin, CancellationToken cancellationToken = default)
        {
            return DigitalReadManyAsync(0, pin, cancellationToken);
        }

        /// <summary>
        /// Reads the signal level from all pin. Designed to work with a multitude of connected devices.
        /// </summary>
        /// <returns>High or low signal level.</returns>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public bool DigitalReadMany()
        {
            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(XFet));

            Span<int> valuesBuffer = _values.Span;

            for (int index = 0; index < _valuesCount; index++)
            {
                if (valuesBuffer[index] is not 255)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Async reads the signal level from all pin. Designed to work with a multitude of connected devices.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>High or low signal level.</returns>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public ValueTask<bool> DigitalReadManyAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return ValueTask.FromResult(DigitalReadMany());
        }

        /// <summary>
        /// Reads the signal level from all pin. Designed to work with a specific connected device.
        /// </summary>
        /// <param name="device">Device number on the chain.</param>
        /// <returns>High or low signal level.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Invalid device number value on the chain.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public bool DigitalReadMany(int device)
        {
            if (device < 0 || device > _valuesCount)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(device));
            }

            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(XFet));

            Span<int> valuesBuffer = _values.Span;

            return valuesBuffer[device] is 255;
        }

        /// <summary>
        /// Async reads the signal level from all pin. Designed to work with a specific connected device.
        /// </summary>
        /// <param name="device">Device number on the chain.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>High or low signal level.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Invalid device number value on the chain.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public ValueTask<bool> DigitalReadManyAsync(int device, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return ValueTask.FromResult(DigitalReadMany(device));
        }

        /// <summary>
        /// Reads the signal level from selected pin. Designed to work with a specific connected device.
        /// </summary>
        /// <param name="device">Device number on the chain.</param>
        /// <param name="pin">Pin number on the device.</param>
        /// <returns>High or low signal level.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Invalid device number value on the chain or invalid pin number value on the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public bool DigitalReadMany(int device, int pin)
        {
            if (pin is < 0 or > 7)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(pin));
            }

            if (device < 0 || device > _valuesCount)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(device));
            }

            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(XFet));

            Span<int> valuesBuffer = _values.Span;

            return (valuesBuffer[device] & 1 << pin) > 0U;
        }

        /// <summary>
        /// Async reads the signal level from selected pin. Designed to work with a specific connected device.
        /// </summary>
        /// <param name="device">Device number on the chain.</param>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>High or low signal level.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Invalid device number value on the chain or invalid pin number value on the device.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public ValueTask<bool> DigitalReadManyAsync(int device, int pin, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return ValueTask.FromResult(DigitalReadMany(device, pin));
        }

        #endregion

        #region Digital write functions

        /// <summary>
        /// Turns all pin into a high or low signal level. Designed to work with a single connected device.
        /// </summary>
        /// <param name="value">High or low signal level.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public void DigitalWrite(bool value)
        {
            DigitalWriteMany(0, value);
        }

        /// <summary>
        /// Async turns all pin into a high or low signal level. Designed to work with a single connected device.
        /// </summary>
        /// <param name="value">High or low signal level.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public ValueTask DigitalWriteAsync(bool value, CancellationToken cancellationToken = default)
        {
            return DigitalWriteManyAsync(0, value, cancellationToken);
        }

        /// <summary>
        /// Turns the selected pin into a high or low signal level. Designed to work with a single connected device.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="value">High or low signal level.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid pin number value on the device.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public void DigitalWrite(int pin, bool value)
        {
            DigitalWriteMany(0, pin, value);
        }

        /// <summary>
        /// Async turns the selected pin into a high or low signal level. Designed to work with a single connected device.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="value">High or low signal level.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid pin number value on the device.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public ValueTask DigitalWriteAsync(int pin, bool value, CancellationToken cancellationToken = default)
        {
            return DigitalWriteManyAsync(0, pin, value, cancellationToken);
        }

        /// <summary>
        /// Turns all pin into a high or low signal level. Designed to work with a multitude of connected devices.
        /// </summary>
        /// <param name="value">High or low signal level.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public void DigitalWriteMany(bool value)
        {
            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(XFet));

            Span<int> valuesBuffer = _values.Span;

            if (value)
            {
                valuesBuffer.Fill(255);
            }
            else
            {
                valuesBuffer.Fill(0);
            }

            Write(true, false);
        }

        /// <summary>
        /// Async turns all pin into a high or low signal level. Designed to work with a multitude of connected devices.
        /// </summary>
        /// <param name="value">High or low signal level.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public ValueTask DigitalWriteManyAsync(bool value, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            DigitalWriteMany(value);

            return default;
        }

        /// <summary>
        /// Turns all pin into a high or low signal level. Designed to work with a specific connected device.
        /// </summary>
        /// <param name="device">Device number on the chain.</param>
        /// <param name="value">High or low signal level.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid device number value on the chain.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public void DigitalWriteMany(int device, bool value)
        {
            if (device < 0 || device > _valuesCount)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(device));
            }

            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(XFet));

            Span<int> valuesBuffer = _values.Span;

            if (value)
            {
                valuesBuffer[device] = 255;
            }
            else
            {
                valuesBuffer[device] = 0;
            }

            Write(true, false);
        }

        /// <summary>
        /// Async turns all pin into a high or low signal level. Designed to work with a specific connected device.
        /// </summary>
        /// <param name="device">Device number on the chain.</param>
        /// <param name="value">High or low signal level.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid device number value on the chain.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public ValueTask DigitalWriteManyAsync(int device, bool value, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            DigitalWriteMany(device, value);

            return default;
        }

        /// <summary>
        /// Turns the selected pin into a high or low signal level. Designed to work with a specific connected device.
        /// </summary>
        /// <param name="device">Device number on the chain.</param>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="value">High or low signal level.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid device number value on the chain or invalid pin number value on the device.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public void DigitalWriteMany(int device, int pin, bool value)
        {
            if (pin is < 0 or > 7)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(pin));
            }

            if (device < 0 || device > _valuesCount)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(device));
            }

            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(XFet));

            Span<int> valuesBuffer = _values.Span;

            if (value)
            {
                valuesBuffer[device] |= 1 << pin;
            }
            else
            {
                valuesBuffer[device] &= ~(1 << pin);
            }

            Write(true, false);
        }

        /// <summary>
        /// Async turns the selected pin into a high or low signal level. Designed to work with a specific connected device.
        /// </summary>
        /// <param name="device">Device number on the chain.</param>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="value">High or low signal level.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid device number value on the chain or invalid pin number value on the device.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public ValueTask DigitalWriteManyAsync(int device, int pin, bool value, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            DigitalWriteMany(device, pin, value);

            return default;
        }

        #endregion

        #region Private functions

        [StackTraceHidden]
        private void Write(bool useWriteMode, bool ignoreThrow)
        {
            try
            {
                Span<byte> writeBuffer = stackalloc byte[sizeof(byte) * _valuesCount];

                if (useWriteMode)
                {
                    Span<int> valuesBuffer = _values.Span;

                    for (int index = 0; index < _valuesCount; index++)
                    {
                        Unsafe.As<byte, int>(ref Unsafe.Add(ref MemoryMarshal.GetReference(writeBuffer), index)) = valuesBuffer[_valuesCount - index - 1];
                    }
                }

                _device.Write(writeBuffer);
            }
            catch (Exception error)
            {
                ThrowHelper.ThrowAmperkaDeviceException(ignoreThrow, error);
            }
        }

        #endregion

        #region IAsyncDisposable and IDisposable

        private void Shutdown()
        {
            if (_disposed)
            {
                return;
            }

            Write(false, true);

            _device.Dispose();
            _device = null;

            _values = null;

            _disposed = true;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Shutdown();

            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            Dispose();

            return default;
        }

        /// <inheritdoc />
        ~XFet()
        {
            Shutdown();
        }

        #endregion
    }
}
