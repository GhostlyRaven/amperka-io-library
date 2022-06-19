using Amperka.IO.Devices.GpioExpander;

// ReSharper disable All

namespace Amperka.IO.Devices
{
    /// <summary>
    /// A interface for working with Troyka Hat, Troyka Cap and GPIO expander.
    /// </summary>
    public interface IGpioExpander : IAsyncDisposable, IDisposable
    {
        #region Digital functions

        /// <summary>
        /// Reads data from the device port.
        /// </summary>
        /// <returns>The value of the data read from the port.</returns>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        int DigitalReadPort();

        /// <summary>
        /// Async reads data from the device port.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>The value of the data read from the port.</returns>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        ValueTask<int> DigitalReadPortAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Writes data from the device port.
        /// </summary>
        /// <param name="value">The value written to the port is from 0 to 1023.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value for writing to the port.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        void DigitalWritePort(int value);

        /// <summary>
        /// Async writes data from the device port.
        /// </summary>
        /// <param name="value">The value written to the port is from 0 to 1023.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value for writing to the port.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        ValueTask DigitalWritePortAsync(int value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads the high or low signal level from the selected pin.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <returns>High or low signal level.</returns>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid pin number value on the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        bool DigitalRead(int pin);

        /// <summary>
        /// Async reads the high or low signal level from the selected pin.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>High or low signal level.</returns>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid pin number value on the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        ValueTask<bool> DigitalReadAsync(int pin, CancellationToken cancellationToken = default);

        /// <summary>
        /// Records the high or low signal level to the selected pin.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="value">High or low signal level.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid pin number value on the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        void DigitalWrite(int pin, bool value);

        /// <summary>
        /// Async records the high or low signal level to the selected pin.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="value">High or low signal level.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid pin number value on the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        ValueTask DigitalWriteAsync(int pin, bool value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the operating mode of the selected pin.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="mode">Pin mode.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value is the pin number on the device or operating mode.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        void PinMode(int pin, PinMode mode);

        /// <summary>
        /// Async sets the operating mode of the selected pin.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="mode">Pin mode.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value is the pin number on the device or operating mode.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        ValueTask PinModeAsync(int pin, PinMode mode, CancellationToken cancellationToken = default);

        #endregion

        #region Analog functions

        /// <summary>
        /// Reads the value of the analog signal from the selected pin.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <returns>The value of the analog signal is in the range from 0 to 4095.</returns>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid pin number value on the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        int AnalogRead(int pin);

        /// <summary>
        /// Async reads the value of the analog signal from the selected pin.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <returns>The value of the analog signal is in the range from 0 to 4095.</returns>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid pin number value on the device.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        ValueTask<int> AnalogReadAsync(int pin, CancellationToken cancellationToken = default);

        /// <summary>
        /// Writes the PWM signal value to the selected pin.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="value">The PWM signal value is in the range from 0 to 255 or from 0 to 4095 in ADC mode.</param>
        /// <param name="mode">Scale mode.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value is the pin number on the device, the operating mode or the value being recorded.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        void AnalogWrite(int pin, int value, ScaleMode mode = ScaleMode.PWM);

        /// <summary>
        /// Async writes the PWM signal value to the selected pin.
        /// </summary>
        /// <param name="pin">Pin number on the device.</param>
        /// <param name="value">The PWM signal value is in the range from 0 to 255 or from 0 to 4095 in ADC mode.</param>
        /// <param name="mode">Scale mode.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value is the pin number on the device, the operating mode or the value being recorded.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        ValueTask AnalogWriteAsync(int pin, int value, ScaleMode mode = ScaleMode.PWM, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the frequency of the ADC operation.
        /// </summary>
        /// <param name="freq">The frequency of the ADC is from 100 Hz to 64 kHz.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value of the ADC frequency.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        void PwmFreq(int freq);

        /// <summary>
        /// Async sets the frequency of the ADC operation.
        /// </summary>
        /// <param name="freq">The frequency of the ADC is from 100 Hz to 64 kHz.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value of the ADC frequency.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        ValueTask PwmFreqAsync(int freq, CancellationToken cancellationToken = default);

        #endregion

        #region Shield settings

        /// <summary>
        /// Sets the speed of the ADC.
        /// </summary>
        /// <param name="speed">The value of the ADC operation speed is from 0 to 7.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value of the ADC operation speed.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        void AdcSpeed(int speed);

        /// <summary>
        /// Async sets the speed of the ADC.
        /// </summary>
        /// <param name="speed">The value of the ADC operation speed is from 0 to 7.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="AmperkaDeviceException">There was a malfunction of the device.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid value of the ADC operation speed.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        ValueTask AdcSpeedAsync(int speed, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets a new address for the device.
        /// </summary>
        /// <param name="newAddress">The address of the device on the bus.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid device address values on the bus.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        void ChangeAddress(int newAddress);

        /// <summary>
        /// Async sets a new address for the device.
        /// </summary>
        /// <param name="newAddress">The address of the device on the bus.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid device address values on the bus.</exception>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        ValueTask ChangeAddressAsync(int newAddress, CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves the new address for the device.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        void SaveAddress();

        /// <summary>
        /// Async saves the new address for the device.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        ValueTask SaveAddressAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Resets the device.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        void Reset();

        /// <summary>
        /// Async resets the device.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="ObjectDisposedException">The device is closed.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        ValueTask ResetAsync(CancellationToken cancellationToken = default);

        #endregion
    }
}
