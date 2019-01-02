using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Zh.ArduinoSerial;
using Zh.ArduinoSerial.Reactive;

namespace SerialConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            var instance = new Program();
            instance.Run().Wait();

            Console.WriteLine("Press Enter to Terminate Program");
            Console.ReadLine();
        }

        private async Task Run()
        {
            //ListAvailiblePorts();

            var spf = new SerialPortFactory();
            var serialPort = spf.Create("COM3", baudRate: 115200);

            var of = new ObservableFactory();
            var linesObservable = of.GetLinesObservable(serialPort);
            linesObservable.Subscribe(Console.WriteLine);
            
            var sw = Stopwatch.StartNew();
            while (sw.Elapsed.TotalMinutes < 1 &&
                   !serialPort.IsOpen)
            {
                try
                {
                    serialPort.Open();
                }
                catch (IOException IoException) when(IoException.Message.EndsWith("does not exist."))
                {
                    //we ignore this error and try again
                }
                await Task.Delay(TimeSpan.FromMilliseconds(300));
            }

            await linesObservable.ToTask();
        }

        private static void ListAvailiblePorts()
        {
            Console.WriteLine("Available Ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }
        }
    }
}
