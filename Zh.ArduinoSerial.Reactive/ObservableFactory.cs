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
                    Exception ex = new Exception("There was an error on the serial port.");
                    ex.Data["EventArgs"] = e.EventArgs;
                    throw ex;
                });
            var lifetimeObservable = GetOpenLifetimeObservable(serialPort, TimeSpan.FromMilliseconds(300));

            var x = lifetimeObservable
                .Select<bool, EventPattern<SerialDataReceivedEventArgs>>(e => { return null; });

            var result =
                GetDataReceivedObservable(serialPort)
                    .Merge(x)
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

        public static IObservable<bool> GetIsOpenObservable(SerialPort serialPort, TimeSpan checkInterval)
        {
            return Observable
                .Interval(checkInterval)
                .Select(l => serialPort.IsOpen)
                .DistinctUntilChanged();
        }

        public static IObservable<bool> GetOpenLifetimeObservable(SerialPort serialPort, TimeSpan checkInterval)
        {
            return GetIsOpenObservable(serialPort, checkInterval)
                .Select(isOpen =>
                {
                    if (isOpen)
                    {
                        return Observable.Return(true);
                    }
                    else
                    {
                        return Observable.Empty<bool>();
                    }
                })
                .Switch();
        }
    }
}
