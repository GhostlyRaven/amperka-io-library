// ReSharper disable All

namespace Amperka.IO.Devices
{
    /// <summary>
    /// A interface for working with I2C Hub.
    /// </summary>
    public interface II2CHub : IDisposable, IAsyncDisposable
    {
        #region Channel functions

        /// <summary>
        /// Switches the I2C hub to the selected channel.
        /// </summary>
        /// <param name="channel">Channel number.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value of the channel.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        void SetChannel(int channel);

        /// <summary>
        /// Async switches the I2C hub to the selected channel.
        /// </summary>
        /// <param name="channel">Channel number.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value of the channel.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        ValueTask SetChannelAsync(int channel, CancellationToken cancellationToken = default);

        #endregion
    }
}
