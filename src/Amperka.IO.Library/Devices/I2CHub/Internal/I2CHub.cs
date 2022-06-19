using System.Device.I2c;
using System.Diagnostics;
using Amperka.IO.Exceptions.Internal;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

// ReSharper disable All

namespace Amperka.IO.Devices.I2CHub.Internal
{
    internal class I2CHub : II2CHub
    {
        #region Constructors and fields

        private bool _disposed;

        private readonly I2cDevice _device;

        public I2CHub(I2cDevice device)
        {
            _device = device;
        }

        #endregion

        #region Channel functions

        public void SetChannel(int channel)
        {
            if (channel is < 0 or > 8)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(channel));
            }

            ThrowHelper.ThrowObjectDisposedException(_disposed, nameof(I2CHub));

            Write(channel, false);
        }

        public ValueTask SetChannelAsync(int channel, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            SetChannel(channel);

            return default;
        }

        #endregion

        #region Private functions

        private void Shutdown()
        {
            Write(0, true);
        }

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

        ~I2CHub()
        {
            Dispose(false);
        }

        #endregion
    }
}
