using Amperka.IO.Devices;
using Amperka.IO.Exceptions.Internal;

// ReSharper disable All

namespace Amperka.IO.Extensions
{
    /// <summary>
    /// Provides additional functions for working with the I2C hub.
    /// </summary>
    public static class I2CHubExtensions
    {
        /// <summary>
        /// Cyclically performs the action on all channels.
        /// </summary>
        /// <param name="hub">Instance of II2CHub.</param>
        /// <param name="body">A method for cyclic execution.</param>
        /// <exception cref="ArgumentNullException">The I2C hub or method body object can't be a null reference.</exception>
        public static void ForEach(this II2CHub hub, Action body)
        {
            if (hub is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(hub));
            }

            if (body is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(body));
            }

            for (int channel = 0; channel < 8; channel++)
            {
                hub.SetChannel(channel);

                body();
            }
        }

        /// <summary>
        /// Cyclically performs the action on all channels.
        /// </summary>
        /// <param name="hub">Instance of II2CHub.</param>
        /// <param name="body">A method for cyclic execution.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="ArgumentNullException">The I2C hub or method body object can't be a null reference.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public static async ValueTask ForEachAsync(this II2CHub hub, Action body, CancellationToken cancellationToken = default)
        {
            if (hub is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(hub));
            }

            if (body is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(body));
            }

            for (int channel = 0; channel < 8; channel++)
            {
                await hub.SetChannelAsync(channel, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                body();
            }
        }

        /// <summary>
        /// Cyclically performs async action on all channels.
        /// </summary>
        /// <param name="hub">Instance of II2CHub.</param>
        /// <param name="body">A method for cyclic execution.</param>
        /// <exception cref="ArgumentNullException">The I2C hub or method body object can't be a null reference.</exception>
        public static void ForEach(this II2CHub hub, Func<Task> body)
        {
            if (hub is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(hub));
            }

            if (body is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(body));
            }

            for (int channel = 0; channel < 8; channel++)
            {
                hub.SetChannel(channel);

                body().GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Cyclically performs async action on all channels.
        /// </summary>
        /// <param name="hub">Instance of II2CHub.</param>
        /// <param name="body">A method for cyclic execution.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="ArgumentNullException">The I2C hub or method body object can't be a null reference.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public static async ValueTask ForEachAsync(this II2CHub hub, Func<Task> body, CancellationToken cancellationToken = default)
        {
            if (hub is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(hub));
            }

            if (body is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(body));
            }

            for (int channel = 0; channel < 8; channel++)
            {
                await hub.SetChannelAsync(channel, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                await body();
            }
        }
    }
}
