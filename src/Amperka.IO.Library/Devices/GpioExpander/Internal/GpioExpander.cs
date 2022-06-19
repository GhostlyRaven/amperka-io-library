using System.Device.I2c;
using System.Diagnostics;
using Amperka.IO.Exceptions.Internal;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

// ReSharper disable All

namespace Amperka.IO.Devices.GpioExpander.Internal
{
    internal class GpioExpander : IGpioExpander
    {
        #region Constructors and fields

        private bool _disposed;

        private readonly I2cDevice _device;

        public GpioExpander(I2cDevice device)
        {
            _device = device;
        }

        #endregion

        #region Digital functions

        public int DigitalReadPort()
        {
            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(GpioExpander));

            int data = Read(Stm32Command.DigitalRead, default);

            return Reverse(data);
        }

        public ValueTask<int> DigitalReadPortAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return ValueTask.FromResult(DigitalReadPort());
        }

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

        public ValueTask DigitalWritePortAsync(int value, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            DigitalWritePort(value);

            return default;
        }

        public bool DigitalRead(int pin)
        {
            if (pin is < 0 or > 8)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(pin));
            }

            return (DigitalReadPort() & Mask(pin)) > 0;
        }

        public ValueTask<bool> DigitalReadAsync(int pin, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return ValueTask.FromResult(DigitalRead(pin));
        }

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

        public ValueTask DigitalWriteAsync(int pin, bool value, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            DigitalWrite(pin, value);

            return default;
        }

        public void PinMode(int pin, PinMode mode)
        {
            if (pin is < 0 or > 8)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(pin));
            }

            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(GpioExpander));

            Stm32Command command = mode switch
            {
                Devices.GpioExpander.PinMode.Output => Stm32Command.PortModeOutput,
                Devices.GpioExpander.PinMode.Input => Stm32Command.PortModeInput,
                Devices.GpioExpander.PinMode.InputPullUp => Stm32Command.PortModePullUp,
                Devices.GpioExpander.PinMode.InputPullDown => Stm32Command.PortModePullDown,
                _ => throw ThrowHelper.GetArgumentOutOfRangeException(nameof(mode))
            };

            int data = Mask(pin);
            data = Reverse(data);

            Write(command, data, false);
        }

        public ValueTask PinModeAsync(int pin, PinMode mode, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            PinMode(pin, mode);

            return default;
        }

        #endregion

        #region Analog functions

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

        public ValueTask<int> AnalogReadAsync(int pin, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return ValueTask.FromResult(AnalogRead(pin));
        }

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

        public ValueTask AnalogWriteAsync(int pin, int value, ScaleMode mode = ScaleMode.PWM, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            AnalogWrite(pin, value, mode);

            return default;
        }

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

        public ValueTask PwmFreqAsync(int freq, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            PwmFreq(freq);

            return default;
        }

        #endregion

        #region Shield settings

        public void AdcSpeed(int speed)
        {
            if (speed is < 0 or > 7)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(speed));
            }

            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(GpioExpander));

            Write(Stm32Command.AdcSpeed, speed, false);
        }

        public ValueTask AdcSpeedAsync(int speed, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            AdcSpeed(speed);

            return default;
        }

        public void ChangeAddress(int newAddress)
        {
            if (newAddress is < 0 or > 127)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(newAddress));
            }

            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(GpioExpander));

            Shutdown();

            Write(Stm32Command.ChangeI2CAddress, newAddress, true);

            _device.Dispose();

            _disposed = true;
        }

        public ValueTask ChangeAddressAsync(int newAddress, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ChangeAddress(newAddress);

            return default;
        }

        public void SaveAddress()
        {
            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(GpioExpander));

            Shutdown();

            Write(Stm32Command.SaveI2CAddress, default, true);

            _device.Dispose();

            _disposed = true;
        }

        public ValueTask SaveAddressAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            SaveAddress();

            return default;
        }

        public void Reset()
        {
            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(GpioExpander));

            Shutdown();

            Write(Stm32Command.Reset, default, true);

            _device.Dispose();

            _disposed = true;
        }

        public ValueTask ResetAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Reset();

            return default;
        }

        #endregion

        #region Private functions

        private void Shutdown()
        {
            Write(Stm32Command.DigitalWriteHigh, 0, true);
            Write(Stm32Command.DigitalWriteLow, -1, true);
        }

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

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            Shutdown();

            if (disposing)
            {
                _device.Dispose();
            }

            _disposed = true;
        }

        public ValueTask DisposeAsync()
        {
            Dispose();

            return default;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GpioExpander()
        {
            Dispose(false);
        }

        #endregion
    }
}
