using Amperka.IO.Resources;
using System.Runtime.Serialization;

// ReSharper disable All

namespace Amperka.IO
{
    /// <summary>
    /// The exception that is thrown when an Amperka device error occurs.
    /// </summary>
    [Serializable]
    public class AmperkaDeviceException : Exception
    {
        /// <inheritdoc />
        public AmperkaDeviceException() : base(Strings.Amperka_Device_Exception_Message) { }

        /// <summary>
        /// Initializes a new instance of the Exception class with a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public AmperkaDeviceException(Exception innerException) : base(Strings.Amperka_Device_Exception_Message, innerException) { }

        /// <inheritdoc />
        public AmperkaDeviceException(string message) : base(message) { }

        /// <inheritdoc />
        public AmperkaDeviceException(string message, Exception innerException) : base(message, innerException) { }

        /// <inheritdoc />
        protected AmperkaDeviceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
