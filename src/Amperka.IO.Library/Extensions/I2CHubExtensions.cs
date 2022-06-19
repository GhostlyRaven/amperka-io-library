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
        /// <param name="method">A method for cyclic execution.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentNullException">The I2C hub or method object can't be a null reference.</exception>
        public static void ForEach(this II2CHub hub, Action method)
        {
            if (hub is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(hub));
            }

            if (method is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(method));
            }

            for (int channel = 0; channel < 8; channel++)
            {
                hub.SetChannel(channel);

                method();
            }
        }

        /// <summary>
        /// Cyclically performs the action on all channels with number.
        /// </summary>
        /// <param name="hub">Instance of II2CHub.</param>
        /// <param name="method">A method for cyclic execution.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentNullException">The I2C hub or method object can't be a null reference.</exception>
        public static void ForEach(this II2CHub hub, Action<int> method)
        {
            if (hub is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(hub));
            }

            if (method is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(method));
            }

            for (int channel = 0; channel < 8; channel++)
            {
                hub.SetChannel(channel);

                method(channel);
            }
        }

        /// <summary>
        /// Cyclically performs the action on all channels.
        /// </summary>
        /// <param name="hub">Instance of II2CHub.</param>
        /// <param name="method">A method for cyclic execution.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentNullException">The I2C hub or method object can't be a null reference.</exception>
        public static void ForEach(this II2CHub hub, Func<Task> method)
        {
            if (hub is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(hub));
            }

            if (method is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(method));
            }

            for (int channel = 0; channel < 8; channel++)
            {
                hub.SetChannel(channel);

                method().GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Cyclically performs the action on all channels with number.
        /// </summary>
        /// <param name="hub">Instance of II2CHub.</param>
        /// <param name="method">A method for cyclic execution.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentNullException">The I2C hub or method object can't be a null reference.</exception>
        public static void ForEach(this II2CHub hub, Func<int, Task> method)
        {
            if (hub is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(hub));
            }

            if (method is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(method));
            }

            for (int channel = 0; channel < 8; channel++)
            {
                hub.SetChannel(channel);

                method(channel).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Cyclically performs async action on all channels.
        /// </summary>
        /// <param name="hub">Instance of II2CHub.</param>
        /// <param name="method">A method for cyclic execution.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentNullException">The I2C hub or method object can't be a null reference.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public static async ValueTask ForEachAsync(this II2CHub hub, Action method, CancellationToken cancellationToken = default)
        {
            if (hub is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(hub));
            }

            if (method is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(method));
            }

            for (int channel = 0; channel < 8; channel++)
            {
                await hub.SetChannelAsync(channel, cancellationToken);

                method();
            }
        }

        /// <summary>
        /// Cyclically performs async action on all channels with number.
        /// </summary>
        /// <param name="hub">Instance of II2CHub.</param>
        /// <param name="method">A method for cyclic execution.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentNullException">The I2C hub or method object can't be a null reference.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public static async ValueTask ForEachAsync(this II2CHub hub, Action<int> method, CancellationToken cancellationToken = default)
        {
            if (hub is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(hub));
            }

            if (method is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(method));
            }

            for (int channel = 0; channel < 8; channel++)
            {
                await hub.SetChannelAsync(channel, cancellationToken);

                method(channel);
            }
        }

        /// <summary>
        /// Cyclically performs async action on all channels.
        /// </summary>
        /// <param name="hub">Instance of II2CHub.</param>
        /// <param name="method">A method for cyclic execution.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentNullException">The I2C hub or method object can't be a null reference.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public static async ValueTask ForEachAsync(this II2CHub hub, Func<CancellationToken, Task> method, CancellationToken cancellationToken = default)
        {
            if (hub is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(hub));
            }

            if (method is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(method));
            }

            for (int channel = 0; channel < 8; channel++)
            {
                await hub.SetChannelAsync(channel, cancellationToken);

                await method(cancellationToken);
            }
        }

        /// <summary>
        /// Cyclically performs async action on all channels with number.
        /// </summary>
        /// <param name="hub">Instance of II2CHub.</param>
        /// <param name="method">A method for cyclic execution.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentNullException">The I2C hub or method object can't be a null reference.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public static async ValueTask ForEachAsync(this II2CHub hub, Func<int, CancellationToken, Task> method, CancellationToken cancellationToken = default)
        {
            if (hub is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(hub));
            }

            if (method is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(method));
            }

            for (int channel = 0; channel < 8; channel++)
            {
                await hub.SetChannelAsync(channel, cancellationToken);

                await method(channel, cancellationToken);
            }
        }
    }
}
