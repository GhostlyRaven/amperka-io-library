using System.Device.I2c;
using System.Diagnostics;
using Amperka.IO.Exceptions;
using Amperka.IO.Devices.Settings;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

// ReSharper disable All

namespace Amperka.IO.Devices
{
    /// <summary>
    /// A class for working with GPIO expander, Troyka Hat and Troyka Cap.
    /// </summary>
    public sealed class GpioExpander : IAsyncDisposable, IDisposable
    {
        #region Constructors and fields

        private bool _disposed;
        private I2cDevice _device;

        /// <summary>
        /// Default device address.
        /// </summary>
        public const int DefaultAddress = 42;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioExpander"/> class.
        /// </summary>
        /// <param name="device">Instance of I2C device.</param>
        /// <exception cref="ArgumentNullException">The I2C device object can't be a null reference.</exception>
        public GpioExpander(I2cDevice device)
        {
            _device = device ?? throw ThrowHelper.GetArgumentNullException(nameof(device));
        }

        #endregion

        #region Digital functions

        /// <summary>
        /// Reads data from the device port.
        /// </summary>
        /// <returns>The value of the data read from the port.</returns>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public int DigitalReadPort()
        {
            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(GpioExpander));

            int data = Read(Stm32Command.DigitalRead, default);

            return Reverse(data);
        }

        /// <summary>
        /// Async reads data from the device port.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>The value of the data read from the port.</returns>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public ValueTask<int> DigitalReadPortAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return ValueTask.FromResult(DigitalReadPort());
        }

        /// <summary>
        /// Writes data from the device port.
        /// </summary>
        /// <param name="value">The value written to the port is from 0 to 1023.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value for writing to the port.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public void DigitalWritePort(int value)
        {
            if (value is < 0 or > 1023)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(value));
            }

            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(GpioExpander));

            int data = Reverse(value);

            Write(Stm32Command.DigitalWriteHigh, data, false);

            data = ~data;

            Write(Stm32Command.DigitalWriteLow, data, false);
        }

        /// <summary>
        /// Async writes data from the device port.
        /// </summary>
        /// <param name="value">The value written to the port is from 0 to 1023.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value for writing to the port.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public ValueTask DigitalWritePortAsync(int value, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            DigitalWritePort(value);

            return default;
        }

        /// <summary>
        /// Reads the high or low signal level from the selected pin.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <returns>High or low signal level.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Invalid pin number value on the device.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public bool DigitalRead(int pin)
        {
            if (pin is < 0 or > 8)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(pin));
            }

            return (DigitalReadPort() & Mask(pin)) > 0;
        }

        /// <summary>
        /// Async reads the high or low signal level from the selected pin.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>High or low signal level.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Invalid pin number value on the device.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public ValueTask<bool> DigitalReadAsync(int pin, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return ValueTask.FromResult(DigitalRead(pin));
        }

        /// <summary>
        /// Records the high or low signal level to the selected pin.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="value">High or low signal level.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid pin number value on the device.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public void DigitalWrite(int pin, bool value)
        {
            if (pin is < 0 or > 8)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(pin));
            }

            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(GpioExpander));

            int data = Mask(pin);
            data = Reverse(data);

            Stm32Command command = value
                ? Stm32Command.DigitalWriteHigh
                : Stm32Command.DigitalWriteLow;

            Write(command, data, false);
        }

        /// <summary>
        /// Async records the high or low signal level to the selected pin.
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
            cancellationToken.ThrowIfCancellationRequested();

            DigitalWrite(pin, value);

            return default;
        }

        /// <summary>
        /// Sets the operating mode of the selected pin.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="mode">Pin mode.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value is the pin number on the device or operating mode.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public void PinMode(int pin, PinMode mode)
        {
            if (pin is < 0 or > 8)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(pin));
            }

            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(GpioExpander));

            Stm32Command command = mode switch
            {
                Settings.PinMode.Output => Stm32Command.PortModeOutput,
                Settings.PinMode.Input => Stm32Command.PortModeInput,
                Settings.PinMode.InputPullUp => Stm32Command.PortModePullUp,
                Settings.PinMode.InputPullDown => Stm32Command.PortModePullDown,
                _ => throw ThrowHelper.GetArgumentOutOfRangeException(nameof(mode))
            };

            int data = Mask(pin);
            data = Reverse(data);

            Write(command, data, false);
        }

        /// <summary>
        /// Async sets the operating mode of the selected pin.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="mode">Pin mode.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value is the pin number on the device or operating mode.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public ValueTask PinModeAsync(int pin, PinMode mode, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            PinMode(pin, mode);

            return default;
        }

        #endregion

        #region Analog functions

        /// <summary>
        /// Reads the value of the analog signal from the selected pin.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <returns>The value of the analog signal is in the range from 0 to 4095.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Invalid pin number value on the device.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public int AnalogRead(int pin)
        {
            if (pin is < 0 or > 8)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(pin));
            }

            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(GpioExpander));

            int data = Read(Stm32Command.AnalogRead, pin);

            return Reverse(data);
        }

        /// <summary>
        /// Async reads the value of the analog signal from the selected pin.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>The value of the analog signal is in the range from 0 to 4095.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Invalid pin number value on the device.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public ValueTask<int> AnalogReadAsync(int pin, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return ValueTask.FromResult(AnalogRead(pin));
        }

        /// <summary>
        /// Writes the PWM signal value to the selected pin.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="value">The PWM signal value is in the range from 0 to 255 or from 0 to 4095 in ADC mode.</param>
        /// <param name="mode">Scale mode.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value is the pin number on the device, the operating mode or the value being recorded.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public void AnalogWrite(int pin, int value, ScaleMode mode = ScaleMode.PWM)
        {
            if (pin is < 0 or > 8)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(pin));
            }

            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(GpioExpander));

            int data = (mode, value) switch
            {
                (ScaleMode.PWM, >= 0 and <= 255) => value,
                (ScaleMode.ADC, >= 0 and <= 4095) => value >> 4,
                (_, < 0 or > 4095) => throw ThrowHelper.GetArgumentOutOfRangeException(nameof(value)),
                _ => throw ThrowHelper.GetArgumentOutOfRangeException(nameof(mode))
            };

            data = (pin & 0xFF) | ((data & 0xFF) << 8);

            Write(Stm32Command.AnalogWrite, data, false);
        }

        /// <summary>
        /// Async writes the PWM signal value to the selected pin.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="value">The PWM signal value is in the range from 0 to 255 or from 0 to 4095 in ADC mode.</param>
        /// <param name="mode">Scale mode.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value is the pin number on the device, the operating mode or the value being recorded.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public ValueTask AnalogWriteAsync(int pin, int value, ScaleMode mode = ScaleMode.PWM, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            AnalogWrite(pin, value, mode);

            return default;
        }


        /// <summary>
        /// Sets the frequency of the ADC operation.
        /// </summary>
        /// <param name="freq">The frequency of the ADC is from 100 Hz to 64 kHz.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value of the ADC frequency.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public void PwmFreq(int freq)
        {
            if (freq is < 100 or > 64000)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(freq));
            }

            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(GpioExpander));

            int data = Reverse(freq);

            Write(Stm32Command.PwmFreq, data, false);
        }

        /// <summary>
        /// Async sets the frequency of the ADC operation.
        /// </summary>
        /// <param name="freq">The frequency of the ADC is from 100 Hz to 64 kHz.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value of the ADC frequency.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public ValueTask PwmFreqAsync(int freq, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            PwmFreq(freq);

            return default;
        }

        #endregion

        #region Shield settings

        /// <summary>
        /// Sets the speed of the ADC.
        /// </summary>
        /// <param name="speed">The value of the ADC operation speed is from 0 to 7.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value of the ADC operation speed.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public void AdcSpeed(int speed)
        {
            if (speed is < 0 or > 7)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(speed));
            }

            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(GpioExpander));

            Write(Stm32Command.AdcSpeed, speed, false);
        }

        /// <summary>
        /// Async sets the speed of the ADC.
        /// </summary>
        /// <param name="speed">The value of the ADC operation speed is from 0 to 7.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value of the ADC operation speed.</exception>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public ValueTask AdcSpeedAsync(int speed, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            AdcSpeed(speed);

            return default;
        }

        /// <summary>
        /// Sets a new address for the device.
        /// </summary>
        /// <param name="newAddress">The address of the device on the bus.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid device address values on the bus.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public void ChangeAddress(int newAddress)
        {
            if (newAddress is < 0 or > 127)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(newAddress));
            }

            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(GpioExpander));

            Write(Stm32Command.ChangeI2CAddress, newAddress, true);

            Shutdown(true);
        }

        /// <summary>
        /// Async sets a new address for the device.
        /// </summary>
        /// <param name="newAddress">The address of the device on the bus.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid device address values on the bus.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public ValueTask ChangeAddressAsync(int newAddress, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ChangeAddress(newAddress);

            return default;
        }

        /// <summary>
        /// Saves the new address for the device.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public void SaveAddress()
        {
            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(GpioExpander));

            Write(Stm32Command.SaveI2CAddress, default, true);

            Shutdown(true);
        }

        /// <summary>
        /// Async saves the new address for the device.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public ValueTask SaveAddressAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            SaveAddress();

            return default;
        }

        /// <summary>
        /// Resets the device.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public void Reset()
        {
            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(GpioExpander));

            Write(Stm32Command.Reset, default, true);

            Shutdown(true);
        }

        /// <summary>
        /// Async resets the device.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        public ValueTask ResetAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Reset();

            return default;
        }

        #endregion

        #region Private functions

        private int Mask(int value)
        {
            return 0x0001 << value;
        }

        private int Reverse(int value)
        {
            return ((value & 0xFF) << 8) | ((value >> 8) & 0xFF);
        }

        [StackTraceHidden]
        private int Read(Stm32Command command, int data)
        {
            try
            {
                Span<byte> readBuffer = stackalloc byte[sizeof(int)];
                Span<byte> writeBuffer = stackalloc byte[sizeof(int)];

                Unsafe.As<byte, int>(ref MemoryMarshal.GetReference(writeBuffer)) = Unsafe.As<Stm32Command, int>(ref command) | (data << 8);

                _device.WriteRead(writeBuffer, readBuffer);

                return Unsafe.As<byte, int>(ref MemoryMarshal.GetReference(readBuffer));
            }
            catch (Exception error)
            {
                throw ThrowHelper.GetAmperkaDeviceException(error);
            }
        }

        [StackTraceHidden]
        private void Write(Stm32Command command, int data, bool ignoreThrow)
        {
            try
            {
                Span<byte> writeBuffer = stackalloc byte[sizeof(int)];

                Unsafe.As<byte, int>(ref MemoryMarshal.GetReference(writeBuffer)) = Unsafe.As<Stm32Command, int>(ref command) | (data << 8);

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
            Write(Stm32Command.DigitalWriteHigh, 0, true);
            Write(Stm32Command.DigitalWriteLow, -1, true);

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
        ~GpioExpander()
        {
            Dispose(false);
        }

        #endregion
    }
}
