using MathNet.Filtering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.IO.Ports;

namespace MyoSensor.Model
{

    class DataModel
    {
        #region Variables
        private const int testFiltercMod = 0;
        private List<double> data = new List<double>();
        private double sampleRate;
        private bool stateNoice = false;

        private int indexDataReturned = 0;
        private ObservableCollection<string> comPorts = new ObservableCollection<string>();
        private bool stateConnected = false;
        private SerialPort serialPort = new SerialPort();
        private string currentNamePort = "";
        #endregion

        #region Setters
        public double SampleRate
        {
            get
            { return this.sampleRate; }
            set
            { this.sampleRate = value; }
        }

        public bool StateReceive
        {
            get;
            set;
        }

        public List<double> Data
        {
            get { return data; }
            set { data = value; }
        }

        public string NamePort
        {
            get
            { return currentNamePort; }
            set
            {
                currentNamePort = value;
            }
        }

        public bool StateConnected
        {
            get { return stateConnected; }
            set
            {
                stateConnected = value;
                if (stateConnected == false)
                {
                    DisconncetSerial();
                    StateReceive = false;
                }
                else
                {
                    if (currentNamePort != "")
                        ConnectToSerial(currentNamePort);
                    else
                        stateConnected = false;
                }

            }
        }

        public ObservableCollection<string> ComPorts
        {
            get { return comPorts; }
            private set { comPorts = value; }
        }

        public bool Noise
        {
            get
            { return stateNoice; }
            set { stateNoice = value; }
        }
        #endregion

        #region Constructor
        public DataModel()
        {
            ComPorts = new ObservableCollection<string>(SerialPort.GetPortNames());
            SerialSetup();
            StateReceive = false;
        }
        #endregion

        private void SerialSetup()
        {
            serialPort.BaudRate = 115200;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;
            serialPort.Handshake = Handshake.None;
            serialPort.RtsEnable = true;

            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }

        private void ConnectToSerial(string name)
        {
            serialPort.PortName = name;
            serialPort.Open();
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            int count_dat = serialPort.BytesToRead;
            byte[] data_com = new byte[count_dat];
            serialPort.Read(data_com, 0,count_dat);

            byte[] num_bute = new byte[10];
            int count = 0;
            for (int i = 0; i < count_dat; i++)
            {
                if (data_com[i] == '\n')
                {
                    // num_bute[count] = Convert.ToByte('\0');
                    string string_num = System.Text.Encoding.UTF8.GetString(num_bute);
                    // string num = BitConverter.ToString(num_bute, 0);
                    count = 0;
                    lock (data)
                    {
                        double num = Convert.ToDouble(string_num);
                        if (num > 4096)
                            continue;
                        data.Add(num);
                    }
                    continue;
                }
                num_bute[count] = data_com[i];
                count++;

            }
        }

        private void DisconncetSerial()
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }
       
        public void StopReceive()
        {
            serialPort.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);
            StateReceive = false;
            // data.Clear();
        }

        public void StartReceive()
        {
            if (StateConnected == true)
            {
                data.Clear();
                serialPort.DiscardInBuffer();
                serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                StateReceive = true;
            }
        }

        public List<double> GetNewData()
        {
            lock (data)
            {
                int currIndexReturnder = indexDataReturned;
                indexDataReturned = data.Count;
                var new_data = data.Skip(currIndexReturnder).Take(data.Count).ToList<double>();
                return new_data;
            }

        }

        
        public void Dispose()
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }
    }
}