using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable All

namespace Amperka.IO.Exceptions.Internal
{
    [StackTraceHidden]
    internal static class ThrowHelper
    {
        [DoesNotReturn]
        internal static void ThrowArgumentNullException(string argumentName)
        {
            throw GetArgumentNullException(argumentName);
        }

        internal static ArgumentNullException GetArgumentNullException(string argumentName)
        {
            return new ArgumentNullException(argumentName);
        }

        [DoesNotReturn]
        internal static void ThrowArgumentOutOfRangeException(string argumentName)
        {
            throw GetArgumentOutOfRangeException(argumentName);
        }

        internal static ArgumentOutOfRangeException GetArgumentOutOfRangeException(string argumentName)
        {
            return new ArgumentOutOfRangeException(argumentName);
        }

        internal static AmperkaDeviceException GetAmperkaDeviceException(Exception exception)
        {
            return new AmperkaDeviceException(exception);
        }

        internal static void ThrowAmperkaDeviceException([DoesNotReturnIf(false)] bool ignoreThrow, Exception exception)
        {
            if (ignoreThrow)
            {
                return;
            }

            throw GetAmperkaDeviceException(exception);
        }

        internal static void ThrowObjectDisposedException([DoesNotReturnIf(true)] bool disposed, string objectName)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(objectName);
            }
        }
    }
}
