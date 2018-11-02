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
using System.Diagnostics;

namespace MyoSensor.Model
{

    class DataModel
    {
        #region Variables
        private const int testFiltercMod = 0;
        // private List<double> data_sensor1 = new List<double>();
        // private List<double> data_sensor2 = new List<double>();
        SensorData dataSensor1 = new SensorData();
        SensorData dataSensor2 = new SensorData();
        private List<byte> input_data = new List<byte>();
        private double sampleRate;
        private bool stateNoice = false;

        private int indexDataReturned = 0;
        private ObservableCollection<string> comPorts = new ObservableCollection<string>();
        private bool stateConnected = false;
        private SerialPort serialPort = new SerialPort();
        private string currentNamePort = "";

        public struct SensorData
        {
            public List<double> dataSensor;
            public int indexDataReturned;
        }
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

        public List<List<double>> Data
        {
            get {
                List<List<double>> Data = new List<List<double>> { dataSensor1.dataSensor, dataSensor2.dataSensor };
                return Data;
            }
            set {
                dataSensor1.dataSensor = value[0];
                dataSensor2.dataSensor = value[1];
            }
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
            InitStruct();
        }
        #endregion

        private void InitStruct()
        {
            dataSensor1.dataSensor = new List<double>();
            dataSensor2.dataSensor = new List<double>();

            dataSensor1.indexDataReturned = 0;
            dataSensor2.indexDataReturned = 0;
        }

        private void SerialSetup()
        {
            serialPort.BaudRate = 115200;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;
            serialPort.Handshake = Handshake.None;
            serialPort.RtsEnable = true;
            // serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }

        private void ConnectToSerial(string name)
        {
            serialPort.PortName = name;
            serialPort.Open();
        }

        int count_rec = 0;
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            while (serialPort.BytesToRead > 10)
            {
                string data = serialPort.ReadLine();
                string[] tokens = data.Split(' ');
                count_rec++;
                double sensor1, sensor2;
                try
                {
                    sensor1 = Convert.ToDouble(tokens[0]);
                    sensor2 = Convert.ToDouble(tokens[1]);

                    lock (dataSensor1.dataSensor)
                    {
                        dataSensor1.dataSensor.Add(sensor1);
                    }

                    lock (dataSensor2.dataSensor)
                    {
                        dataSensor2.dataSensor.Add(sensor2);
                    }
                }
                catch
                {
                    continue;
                }
            }
            //int count_dat = serialPort.BytesToRead;
            //byte[] data_com = new byte[count_dat];
            //serialPort.Read(data_com, 0, count_dat);
            //string InputString = Encoding.UTF8.GetString(data_com);
            //for (int i = 0; i < count_dat; i++)
            //{
            //    input_data.Add(data_com[i]);
            //    /*if (data_com[i] == '\n')
            //    {
            //        // num_bute[count] = Convert.ToByte('\0');
            //        string string_num = System.Text.Encoding.UTF8.GetString(num_bute);
            //        // string num = BitConverter.ToString(num_bute, 0);
            //        count = 0;
            //        double num = 0;
            //        try
            //        {
            //            num = Convert.ToDouble(string_num);
            //        }
            //        catch
            //        {
            //            continue;
            //        }
            //        if (num > 4096)
            //            continue;

            //        lock (data)
            //        {
            //            data.Add(num);
            //        }
            //        continue;
            //    }
            //    num_bute[count] = data_com[i];
            //    count++;
            //    */
            //}
            //// return;

            //int count = 0;
            //byte[] num_byte = new byte[20];
            //for (int i = 0; i < input_data.Count; i++)
            //{
            //    num_byte[count] = input_data[i];
            //    if (num_byte[count] == '\n')
            //    {
            //        //num_byte[count] = Convert.ToByte('\0');
            //        string StringValues = Encoding.UTF8.GetString(num_byte);
            //        string[] tokens = StringValues.Split(' ');

            //        double sensor1_value, sensor2_value;
            //        try
            //        {
            //            string string_sensor1 = tokens[0];// Encoding.UTF8.GetString(sensor1_byte);
            //            string string_sensor2 = tokens[1];// Encoding.UTF8.GetString(sensor2_byte);
            //            sensor1_value = Convert.ToDouble(string_sensor1);
            //            sensor2_value = Convert.ToDouble(string_sensor2);

            //        }
            //        catch
            //        {
            //            input_data.RemoveRange(0, count);
            //            count = 0;
            //            continue;
            //        }


            //        input_data.RemoveRange(0, count);

            //        lock (dataSensor1.dataSensor)
            //        {
            //            dataSensor1.dataSensor.Add(sensor1_value);
            //        }

            //        lock (dataSensor2.dataSensor)
            //        {
            //            dataSensor2.dataSensor.Add(sensor2_value);
            //        }
            //        count = 0;
            //        continue;
            //    }
            //    count++;
            //}
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
                dataSensor1.dataSensor.Clear();
                dataSensor2.dataSensor.Clear();

                dataSensor1.indexDataReturned = 0;
                dataSensor2.indexDataReturned = 0;

                serialPort.DiscardInBuffer();
                serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                StateReceive = true;
            }
        }

        public List<List<double>> GetNewData()
        {
            List<List<double>> data_result = new List<List<double>>();
            lock (dataSensor1.dataSensor)
            {
                int currIndexReturnder = dataSensor1.indexDataReturned;
                dataSensor1.indexDataReturned = dataSensor1.dataSensor.Count;
                var new_data = dataSensor1.dataSensor.Skip(currIndexReturnder).Take(dataSensor1.dataSensor.Count).ToList<double>();
                data_result.Add(new_data);
            }

            lock (dataSensor2.dataSensor)
            {
                int currIndexReturnder = dataSensor2.indexDataReturned;
                dataSensor2.indexDataReturned = dataSensor2.dataSensor.Count;
                var new_data = dataSensor2.dataSensor.Skip(currIndexReturnder).Take(dataSensor1.dataSensor.Count).ToList<double>();
                data_result.Add(new_data);
            }

            return data_result;
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