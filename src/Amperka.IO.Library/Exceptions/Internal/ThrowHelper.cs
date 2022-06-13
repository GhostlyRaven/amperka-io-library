using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable All

namespace Amperka.IO.Exceptions.Internal
{
    //https://github.com/dotnet/runtime/blob/215b39abf947da7a40b0cb137eab4bceb24ad3e3/src/libraries/System.Private.CoreLib/src/System/ThrowHelper.cs

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
