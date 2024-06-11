namespace TakahashiGroup_SyringeBotGUI
{
    using System.IO.Ports;
    using System.Xml.Linq;

    public partial class Form1 : Form
    {
        SerialPort serialPort1;
        public delegate void d1(string indata);
        bool isConnected = false;

        public Form1()
        {
            InitializeComponent();

            // Initialize the serial port
            serialPort1 = new SerialPort();
            serialPort1.BaudRate = 115200;
            serialPort1.Parity = Parity.None;
            serialPort1.StopBits = StopBits.One;
            serialPort1.DataBits = 8;
            serialPort1.Handshake = Handshake.None;
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(SerialPort1_DataReceived);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length != 0)
            {
                foreach (string port in ports)
                {
                    availableCOMPorts.Items.Add(port);
                }
                connectBtn.Enabled = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Close the serial port
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
        }

        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            if (availableCOMPorts != null && availableCOMPorts.SelectedItem != null)
            {
                serialPort1.PortName = availableCOMPorts.SelectedItem.ToString();
                serialPort1.Open();
                connectBtn.Visible = false;
                availableCOMPorts.Visible = false;
                serialPort1.Write("A77");
                timer1.Start();
            }
        }

        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string indata = serialPort1.ReadLine();
            d1 writeit = new d1(Write2Form);
            Invoke(writeit, indata);
        }


        // This function handles data sent from the Arduino
        public void Write2Form(string indata)
        {            
            char firstchar = indata[0];            
            if (firstchar == 'F')
            {
                int numdata = -1;
                Int32.TryParse(indata.Substring(1), out numdata);
                if (!isConnected)
                {
                    if (numdata == 22)
                    {
                        isConnected = true;
                        connStatus.Text = "ONLINE";
                        connStatus.ForeColor = Color.Lime;
                        sendCmdBtn.Enabled = true;
                    }
                }
                else {
                    if (numdata == 55)
                    {
                        sendCmdBtn.Enabled = true;
                        label1.Text = "Servo Moved";
                    }
                    else if (numdata == 44)
                    {
                        sendCmdBtn.Enabled = true;
                        label1.Text = "Motor Moved";
                    }
                    else if (numdata == 88)
                    {
                        sendCmdBtn.Enabled = true;
                        label1.Text = "Servos Reset to 0";
                    }
                    else if (numdata == 0)
                    {
                        sendCmdBtn.Enabled = true;
                        label1.Text = "Command Unknown";
                    }
                    else
                    {
                        label1.Text = "Illegal Response";
                    }
                    label2.Text = "Ready";
                    sendCmdBtn.Enabled = true;
                }                
            }
            else if(firstchar == 'E')
            {
                label3.Text = indata;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            if (!isConnected)
            {
                connStatus.ForeColor = Color.Crimson;
                connStatus.Text = "NO SYRINGE-BOT DEVICE";
            }


        }

        private void sendCmdBtn_Click(object sender, EventArgs e)
        {
            string cmd = commandText.Text;
            sendCmdBtn.Enabled = false;
            label2.Text = "Waiting";
            serialPort1.Write(cmd);
            commandText.Text = "";
            cmdHistory.Items.Add(cmd);
        }
    }
}
