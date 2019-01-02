using System;
using System.IO.Ports;
using System.Reactive;
using System.Reactive.Linq;

namespace Zh.ArduinoSerial.Reactive
{
    public class ObservableFactory
    {
        public IObservable<string> GetLinesObservable(SerialPort serialPort)
        {
            var errorObservable = GetErrorReceivedObservable(serialPort)
                .Select<EventPattern<SerialErrorReceivedEventArgs>, EventPattern<SerialDataReceivedEventArgs>>(e =>
                {
                    var ex = new Exception("There was an error on the serial port.");
                    ex.Data["EventArgs"] = e.EventArgs;
                    throw ex;
                });
            var result =
                GetDataReceivedObservable(serialPort)
                    .Merge(errorObservable)
                    .Select(e => serialPort.ReadLine());
            return result;
        }

        public static IObservable<EventPattern<SerialDataReceivedEventArgs>> GetDataReceivedObservable(SerialPort serialPort)
        {
            return Observable.FromEventPattern<SerialDataReceivedEventHandler, SerialDataReceivedEventArgs>(
                h => serialPort.DataReceived += h,
                h => serialPort.DataReceived -= h);
        }

        public static IObservable<EventPattern<SerialErrorReceivedEventArgs>> GetErrorReceivedObservable(SerialPort serialPort)
        {
            return Observable.FromEventPattern<SerialErrorReceivedEventHandler, SerialErrorReceivedEventArgs>(
                h => serialPort.ErrorReceived += h,
                h => serialPort.ErrorReceived -= h);
        }
    }
}
