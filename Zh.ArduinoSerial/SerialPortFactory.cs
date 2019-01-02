using System.IO.Ports;

namespace Zh.ArduinoSerial
{
    public class SerialPortFactory
    {
        public SerialPort Create(string portName, 
            int baudRate = 9600, 
            Parity parity = Parity.None,
            int dataBits = 8,
            StopBits stopBits = StopBits.One)
        {
            var result = new SerialPort(portName, baudRate, parity);
            result.DataBits = dataBits;
            result.StopBits = stopBits;

            return result;
        }
    }
}
