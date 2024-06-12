namespace TakahashiGroup_SyringeBotGUI
{
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.Logging;
    using System.IO.Ports;
    using System.Windows.Forms;
    using System.Xml.Linq;
    using static System.Windows.Forms.LinkLabel;

    /// <summary>
    /// SBGUIMainForm Class Definition
    /// </summary>
    public partial class SBGUIMainForm : Form
    {
        // Consts
        public const int NO_OF_SYRINGES = 4;
        public const int BIG_SYRINGE_MAX = 60;
        public const int SMALL_SYRINGE_MAX = 10;
        public const double SPIN_PER_ML_10 = 2.9; //29 for full movement
        public const double SPIN_PER_ML_60 = 0.75; //45 for full movement
        public const int ANGLE_GRADE_PER_STEP = 90;
        string[] valveStates = { "AC", "MO", "SO" };

        enum connStates : int { neverConnected = -1, disconnected = 0, connected = 1, }

        // Global Variables
        SerialPort serialPort1;
        public delegate void d1(string indata);
        int isConnected = (int)connStates.neverConnected;
        int isResetting = -1;
        int isInit = -1;
        int[] syPumpTargetVals = Enumerable.Repeat(0, NO_OF_SYRINGES).ToArray();

        bool macroIsRecording = false;
        List<string> currentMacro = new List<string>();
        int macroPointer = -1;


        /// <summary>
        /// Main Form Constructor
        /// </summary>
        public SBGUIMainForm()
        {
            this.WindowState = FormWindowState.Maximized;
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


        /// <summary>
        /// EVENT HANDLER: Main Form Finished Loading, finalize and initiate component settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SBGUIMainForm_Load(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;

            string[] ports = SerialPort.GetPortNames();
            if (ports.Length != 0)
            {
                foreach (string port in ports)
                {
                    availableCOMPorts.Items.Add(port);
                }
                connectBtn.Enabled = true;
            }
            availableCOMPorts.Select();
        }


        /// <summary>
        /// EVENT HANDLER: Main Form Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Close the serial port
            if (serialPort1.IsOpen) { serialPort1.Close(); }
        }


        /// <summary>
        /// EVENT HANDLER: Connection Button Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            if (isConnected == (int)connStates.neverConnected)
            {
                if (availableCOMPorts != null && availableCOMPorts.SelectedItem != null && availableCOMPorts.SelectedIndex != -1)
                {
                    serialPort1.PortName = availableCOMPorts.SelectedItem.ToString();
                    serialPort1.Open();
                    connectBtn.Enabled = false;
                    availableCOMPorts.Enabled = false;
                    sendCmdToBot("A77");
                    timer1.Start();
                }
            }
            else if (isConnected == (int)connStates.disconnected)
            {
                connectBtn.Enabled = false;
                sendCmdToBot("C77");
            }
            else if (isConnected == (int)connStates.connected)
            {
                connectBtn.Enabled = false;
                sendCmdToBot("D77");
            }
        }


        /// <summary>
        /// EVENT HANDLER: Serial Data Recieved on the USB COM port (from SB)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string indata = serialPort1.ReadLine();
            d1 writeit = new d1(Write2Form);
            BeginInvoke(writeit, indata);
        }

        /// <summary>
        /// Sends the command to the serial port
        /// </summary>
        /// <param name="cmd"></param>
        private void sendCmdToBot(string cmd)
        {
            if (macroIsRecording)
            {
                currentMacro.Add(cmd);
                currLoadedMacro.Text += cmd + "\r\n";
            }
            serialPort1.Write(cmd);
        }


        /// <summary>
        /// Returns the Correct text for valve state
        /// </summary>
        /// <param name="cbIndex"></param>
        private string getValveStateTxt(int cbIndex)
        {
            string vst = "N/A";
            if (cbIndex > -1)
            {
                vst = valveStates[cbIndex];
            }
            return vst;
        }


        /// <summary>
        /// This function handles what to do when reset has been finished by SB
        /// </summary>
        /// <returns>msg string</returns>
        private string initFinished(int syIdx)
        {
            string msg = "Syringe " + isInit + " initiated";
            isInit = -1;

            if (syIdx == 0)
            {
                Sy1CurrValTrackBar.Value = 60;
                Sy1CurrValTrackBar.BackColor = SystemColors.Control;
                Sy1CurrValTxt.Text = "60ml";
                Sy1V1StateTxt.Text = "AC";
                Sy1V2StateTxt.Text = "AC";
                Sy1StateInfo.ForeColor = Color.Lime;
                Sy1StateInfo.Text = "Ready!";
                Sy1Panel.Enabled = true;
                InitS1Btn.Enabled = true;
            }
            else if (syIdx == 1)
            {
                Sy2CurrValTrackBar.Value = 60;
                Sy2CurrValTrackBar.BackColor = SystemColors.Control;
                Sy2CurrValTxt.Text = "60ml";
                Sy2V1StateTxt.Text = "AC";
                Sy2V2StateTxt.Text = "AC";
                Sy2StateInfo.ForeColor = Color.Lime;
                Sy2StateInfo.Text = "Ready!";
                Sy2Panel.Enabled = true;
                InitS2Btn.Enabled = true;
            }
            else if (syIdx == 2)
            {
                Sy3CurrValTrackBar.Value = 10;
                Sy3CurrValTrackBar.BackColor = SystemColors.Control;
                Sy3CurrValTxt.Text = "10ml";
                Sy3V1StateTxt.Text = "AC";
                Sy3V2StateTxt.Text = "AC";
                Sy3StateInfo.ForeColor = Color.Lime;
                Sy3StateInfo.Text = "Ready!";
                Sy3Panel.Enabled = true;
                InitS3Btn.Enabled = true;
            }
            else if (syIdx == 3)
            {
                Sy4CurrValTrackBar.Value = 10;
                Sy4CurrValTrackBar.BackColor = SystemColors.Control;
                Sy4CurrValTxt.Text = "10ml";
                Sy4V1StateTxt.Text = "AC";
                Sy4V2StateTxt.Text = "AC";
                Sy4StateInfo.ForeColor = Color.Lime;
                Sy4StateInfo.Text = "Ready!";
                Sy4Panel.Enabled = true;
                InitS4Btn.Enabled = true;
            }

            return msg;
        }



        /// <summary>
        /// This function handles what to do when reset has been finished by SB
        /// </summary>
        /// <returns>msg string</returns>
        private string resetFinished()
        {
            isResetting = -1;
            string msg = "All Syringes Reset";
            Sy1CurrValTrackBar.Value = 60;
            Sy1CurrValTrackBar.BackColor = SystemColors.Control;
            Sy1CurrValTxt.Text = "60ml";
            Sy1V1StateTxt.Text = "AC";
            Sy1V2StateTxt.Text = "AC";
            Sy2CurrValTrackBar.Value = 60;
            Sy2CurrValTrackBar.BackColor = SystemColors.Control;
            Sy2CurrValTxt.Text = "60ml";
            Sy2V1StateTxt.Text = "AC";
            Sy2V2StateTxt.Text = "AC";
            Sy3CurrValTrackBar.Value = 10;
            Sy3CurrValTrackBar.BackColor = SystemColors.Control;
            Sy3CurrValTxt.Text = "10ml";
            Sy3V1StateTxt.Text = "AC";
            Sy3V2StateTxt.Text = "AC";
            Sy4CurrValTrackBar.Value = 10;
            Sy4CurrValTrackBar.BackColor = SystemColors.Control;
            Sy4CurrValTxt.Text = "10ml";
            Sy4V1StateTxt.Text = "AC";
            Sy4V2StateTxt.Text = "AC";
            Sy1StateInfo.ForeColor = Color.Lime;
            Sy1StateInfo.Text = "Ready!";
            Sy2StateInfo.ForeColor = Color.Lime;
            Sy2StateInfo.Text = "Ready!";
            Sy3StateInfo.ForeColor = Color.Lime;
            Sy3StateInfo.Text = "Ready!";
            Sy4StateInfo.ForeColor = Color.Lime;
            Sy4StateInfo.Text = "Ready!";
            Sy1Panel.Enabled = true;
            Sy2Panel.Enabled = true;
            Sy3Panel.Enabled = true;
            Sy4Panel.Enabled = true;
            resetBtn.BackColor = SystemColors.Control;
            resetBtn.Enabled = true;
            return msg;
        }

        /// <summary>
        /// This function handles what to do when reset has been finished by SB
        /// </summary>
        /// <returns>msg string</returns>
        private string emergencyStopped(int whatSyringe)
        {
            string msg = "";
            if (whatSyringe == -1) { msg = "Emergency Stop!"; }
            else { msg = "Syringe " + (whatSyringe + 1).ToString() + " manually dislocated."; }

            if (whatSyringe == 0 || whatSyringe == -1)
            {
                Sy1CurrValTrackBar.Value = BIG_SYRINGE_MAX / 2;
                Sy1CurrValTrackBar.BackColor = Color.Red;
                Sy1CurrValTxt.Text = "?ml";
                Sy1V1StateTxt.Text = "N/A";
                Sy1V2StateTxt.Text = "N/A";
                Sy1StateInfo.ForeColor = Color.Crimson;
                Sy1StateInfo.Text = "Init or Reset Required";
                Sy1Panel.Enabled = false;
            }

            if (whatSyringe == 1 || whatSyringe == -1)
            {
                Sy2CurrValTrackBar.Value = BIG_SYRINGE_MAX / 2;
                Sy2CurrValTrackBar.BackColor = Color.Red;
                Sy2CurrValTxt.Text = "?ml";
                Sy2V1StateTxt.Text = "N/A";
                Sy2V2StateTxt.Text = "N/A";
                Sy2StateInfo.ForeColor = Color.Crimson;
                Sy2StateInfo.Text = "Init or Reset Required";
                Sy2Panel.Enabled = false;
            }

            if (whatSyringe == 2 || whatSyringe == -1)
            {
                Sy3CurrValTrackBar.Value = SMALL_SYRINGE_MAX / 2;
                Sy3CurrValTrackBar.BackColor = Color.Red;
                Sy3CurrValTxt.Text = "?ml";
                Sy3V1StateTxt.Text = "N/A";
                Sy3V2StateTxt.Text = "N/A";
                Sy3StateInfo.ForeColor = Color.Crimson;
                Sy3StateInfo.Text = "Init or Reset Required";
                Sy3Panel.Enabled = false;
            }

            if (whatSyringe == 3 || whatSyringe == -1)
            {
                Sy4CurrValTrackBar.Value = SMALL_SYRINGE_MAX / 2;
                Sy4CurrValTrackBar.BackColor = Color.Red;
                Sy4CurrValTxt.Text = "?ml";
                Sy4V1StateTxt.Text = "N/A";
                Sy4V2StateTxt.Text = "N/A";
                Sy4StateInfo.ForeColor = Color.Crimson;
                Sy4StateInfo.Text = "Init or Reset Required";
                Sy4Panel.Enabled = false;
            }

            resetBtn.BackColor = Color.FromArgb(128, 255, 128);
            return msg;
        }


        /// <summary>
        /// Collect needed data and send a pump command to the proper Syringe
        /// </summary>
        /// <param name="syIdx"></param>
        private void execPumpCmd(int syIdx)
        {
            bool[] isOks = { false, false, false, false };
            if (syIdx == 0)
            {
                isOks[0] = Sy1ReqValTrackBar.Value >= 0;
                isOks[1] = Sy1ReqValTrackBar.Value <= BIG_SYRINGE_MAX;
                isOks[2] = Sy1ReqValTrackBar.Value != Sy1CurrValTrackBar.Value;
                isOks[3] = !(Sy1V1StateTxt.Text == valveStates[0] || (Sy1V1StateTxt.Text != valveStates[2] && Sy1V2StateTxt.Text == valveStates[0]));
            }
            else if (syIdx == 1)
            {
                isOks[0] = Sy2ReqValTrackBar.Value >= 0;
                isOks[1] = Sy2ReqValTrackBar.Value <= BIG_SYRINGE_MAX;
                isOks[2] = Sy2ReqValTrackBar.Value != Sy2CurrValTrackBar.Value;
                isOks[3] = !(Sy2V1StateTxt.Text == valveStates[0] || (Sy2V1StateTxt.Text != valveStates[2] && Sy2V2StateTxt.Text == valveStates[0]));
            }
            else if (syIdx == 2)
            {
                isOks[0] = Sy3ReqValTrackBar.Value >= 0;
                isOks[1] = Sy3ReqValTrackBar.Value <= BIG_SYRINGE_MAX;
                isOks[2] = Sy3ReqValTrackBar.Value != Sy3CurrValTrackBar.Value;
                isOks[3] = !(Sy3V1StateTxt.Text == valveStates[0] || (Sy3V1StateTxt.Text != valveStates[2] && Sy3V2StateTxt.Text == valveStates[0]));
            }
            else if (syIdx == 3)
            {
                isOks[0] = Sy4ReqValTrackBar.Value >= 0;
                isOks[1] = Sy4ReqValTrackBar.Value <= BIG_SYRINGE_MAX;
                isOks[2] = Sy4ReqValTrackBar.Value != Sy4CurrValTrackBar.Value;
                isOks[3] = !(Sy4V1StateTxt.Text == valveStates[0] || (Sy4V1StateTxt.Text != valveStates[2] && Sy4V2StateTxt.Text == valveStates[0]));
            }

            if (isOks[0] && isOks[1] && isOks[2] && isOks[3])
            {
                int currMLVal = -1, diffMLVal = -1, reqMLVal = -1;
                double currSpinPerMl = SPIN_PER_ML_60;

                if (syIdx == 0)
                {
                    reqMLVal = Sy1ReqValTrackBar.Value;
                    currMLVal = Sy1CurrValTrackBar.Value;
                }
                else if (syIdx == 1)
                {
                    reqMLVal = Sy2ReqValTrackBar.Value;
                    currMLVal = Sy2CurrValTrackBar.Value;
                }
                else if (syIdx == 2)
                {
                    reqMLVal = Sy3ReqValTrackBar.Value;
                    currMLVal = Sy3CurrValTrackBar.Value;
                    currSpinPerMl = SPIN_PER_ML_10;
                }
                else if (syIdx == 3)
                {
                    reqMLVal = Sy4ReqValTrackBar.Value;
                    currMLVal = Sy4CurrValTrackBar.Value;
                    currSpinPerMl = SPIN_PER_ML_10;
                }

                if (reqMLVal != -1 && currMLVal != -1)
                {
                    syPumpTargetVals[syIdx] = reqMLVal;
                    diffMLVal = syPumpTargetVals[syIdx] - currMLVal;
                    double mlToSpin = diffMLVal * currSpinPerMl;
                    string cmd = "P" + syIdx.ToString() + mlToSpin.ToString();
                    SBInfo.Text = "Sy-" + syIdx.ToString() + " Pump Cmd Sent...";
                    sendCmdToBot(cmd);
                }

                if (syIdx == 0) { Sy1StateInfo.ForeColor = Color.Lime; Sy1StateInfo.Text = "Ready!"; }
                else if (syIdx == 1) { Sy2StateInfo.ForeColor = Color.Lime; Sy2StateInfo.Text = "Ready!"; }
                else if (syIdx == 2) { Sy3StateInfo.ForeColor = Color.Lime; Sy3StateInfo.Text = "Ready!"; }
                else if (syIdx == 3) { Sy4StateInfo.ForeColor = Color.Lime; Sy4StateInfo.Text = "Ready!"; }
            }
            else
            {
                if (!isOks[3])
                {
                    if (syIdx == 0) { Sy1StateInfo.ForeColor = Color.Crimson; Sy1StateInfo.Text = "Pump Disabled (Valves Closed)"; }
                    else if (syIdx == 1) { Sy2StateInfo.ForeColor = Color.Crimson; Sy2StateInfo.Text = "Pump Disabled (Valves Closed)"; }
                    else if (syIdx == 2) { Sy3StateInfo.ForeColor = Color.Crimson; Sy3StateInfo.Text = "Pump Disabled (Valves Closed)"; }
                    else if (syIdx == 3) { Sy4StateInfo.ForeColor = Color.Crimson; Sy4StateInfo.Text = "Pump Disabled (Valves Closed)"; }
                }
            }
        }


        /// <summary>
        /// Collect needed data and send a valve command to the proper Syringe (and valve)
        /// </summary>
        /// <param name="syIdx"></param>
        /// <param name="valveId"></param>
        private void execValveCmd(int syIdx, int valveId)
        {
            int newAngle = -1;
            if (syIdx == 0)
            {
                if (valveId == 1) { newAngle = (Sy1V1ReqStateCB.SelectedIndex != -1) ? Sy1V1ReqStateCB.SelectedIndex * ANGLE_GRADE_PER_STEP : -1; }
                else if (valveId == 2) { newAngle = (Sy1V2ReqStateCB.SelectedIndex != -1) ? Sy1V2ReqStateCB.SelectedIndex * ANGLE_GRADE_PER_STEP : -1; }
            }
            if (syIdx == 1)
            {
                if (valveId == 1) { newAngle = (Sy2V1ReqStateCB.SelectedIndex != -1) ? Sy2V1ReqStateCB.SelectedIndex * ANGLE_GRADE_PER_STEP : -1; }
                else if (valveId == 2) { newAngle = (Sy2V2ReqStateCB.SelectedIndex != -1) ? Sy2V2ReqStateCB.SelectedIndex * ANGLE_GRADE_PER_STEP : -1; }
            }
            if (syIdx == 2)
            {
                if (valveId == 1) { newAngle = (Sy3V1ReqStateCB.SelectedIndex != -1) ? Sy3V1ReqStateCB.SelectedIndex * ANGLE_GRADE_PER_STEP : -1; }
                else if (valveId == 2) { newAngle = (Sy3V2ReqStateCB.SelectedIndex != -1) ? Sy3V2ReqStateCB.SelectedIndex * ANGLE_GRADE_PER_STEP : -1; }
            }
            if (syIdx == 3)
            {
                if (valveId == 1) { newAngle = (Sy4V1ReqStateCB.SelectedIndex != -1) ? Sy4V1ReqStateCB.SelectedIndex * ANGLE_GRADE_PER_STEP : -1; }
                else if (valveId == 2) { newAngle = (Sy4V2ReqStateCB.SelectedIndex != -1) ? Sy4V2ReqStateCB.SelectedIndex * ANGLE_GRADE_PER_STEP : -1; }
            }

            if (newAngle != -1)
            {
                if (newAngle > 170) { newAngle = 170; }
                string cmd = "V" + syIdx.ToString() + valveId.ToString() + newAngle.ToString();
                SBInfo.Text = "Sy-" + syIdx.ToString() + " Valve-" + valveId.ToString() + " Cmd Sent...";
                sendCmdToBot(cmd);

                if (syIdx == 0) { Sy1StateInfo.ForeColor = Color.Lime; Sy1StateInfo.Text = "Ready!"; }
                else if (syIdx == 1) { Sy2StateInfo.ForeColor = Color.Lime; Sy2StateInfo.Text = "Ready!"; }
                else if (syIdx == 2) { Sy3StateInfo.ForeColor = Color.Lime; Sy3StateInfo.Text = "Ready!"; }
                else if (syIdx == 3) { Sy4StateInfo.ForeColor = Color.Lime; Sy4StateInfo.Text = "Ready!"; }

            }
        }/// <summary>
         /// Collect needed data and send a init command to the proper Syringe
         /// </summary>
         /// <param name="syIdx"></param>
        private void execInitSyringeCmd(int syIdx)
        {
            string cmd = "I" + syIdx.ToString();
            SBInfo.Text = "Sy-" + syIdx.ToString() + " Init Cmd Sent...";
            sendCmdToBot(cmd);
        }



        /// <summary>
        /// This function handles data sent from the Arduino
        /// </summary>
        /// <param name="indata"></param>
        public void Write2Form(string indata)
        {
            char firstchar = indata[0];
            // F Messages are most basic messages from SB
            if (firstchar == 'F')
            {
                int numdata = -1;
                Int32.TryParse(indata.Substring(1), out numdata);
                string msg = "";

                if (numdata >= 550 && numdata <= 553)
                {
                    int syIdx = numdata - 550;
                    if (syIdx == 0) { Sy1CurrValTrackBar.Value = syPumpTargetVals[syIdx]; }
                    if (syIdx == 1) { Sy2CurrValTrackBar.Value = syPumpTargetVals[syIdx]; }
                    if (syIdx == 2) { Sy3CurrValTrackBar.Value = syPumpTargetVals[syIdx]; }
                    if (syIdx == 3) { Sy4CurrValTrackBar.Value = syPumpTargetVals[syIdx]; }

                    msg = "Syringe " + (syIdx + 1).ToString() + " Motor Moved to the target level successfully";
                }
                else if (numdata >= 570 && numdata <= 573)
                {
                    int syIdx = numdata - 570;
                    if (syIdx == 0) { Sy1CurrValTrackBar.Value = BIG_SYRINGE_MAX; }
                    if (syIdx == 1) { Sy2CurrValTrackBar.Value = BIG_SYRINGE_MAX; }
                    if (syIdx == 2) { Sy3CurrValTrackBar.Value = SMALL_SYRINGE_MAX; }
                    if (syIdx == 3) { Sy4CurrValTrackBar.Value = SMALL_SYRINGE_MAX; }
                    msg = "Syringe " + (syIdx + 1).ToString() + " Motor Stopped at the Upper-most position";
                    if (isResetting > -1) { isResetting--; msg = ""; }
                    if (isResetting == 0) { msg = resetFinished(); }
                    if (isInit > 0) { msg = initFinished(syIdx); }
                }
                else if (numdata >= 530 && numdata <= 533)
                {
                    int syIdx = numdata - 530;
                    emergencyStopped(-1);
                    msg = "Syringe " + (syIdx + 1).ToString() + " Motor Stopped because Valve was closed";
                }
                else if (numdata >= 6601 && numdata <= 6633)
                {
                    int syIdx = (numdata - 6601) / 10;
                    int vId = numdata % 2 == 0 ? 2 : 1;
                    if (syIdx == 0) { if (vId == 1) { Sy1V1StateTxt.Text = getValveStateTxt(Sy1V1ReqStateCB.SelectedIndex); } else if (vId == 2) { Sy1V2StateTxt.Text = getValveStateTxt(Sy1V2ReqStateCB.SelectedIndex); } }
                    if (syIdx == 1) { if (vId == 1) { Sy2V1StateTxt.Text = getValveStateTxt(Sy2V1ReqStateCB.SelectedIndex); } else if (vId == 2) { Sy2V2StateTxt.Text = getValveStateTxt(Sy2V2ReqStateCB.SelectedIndex); } }
                    if (syIdx == 2) { if (vId == 1) { Sy3V1StateTxt.Text = getValveStateTxt(Sy3V1ReqStateCB.SelectedIndex); } else if (vId == 2) { Sy3V2StateTxt.Text = getValveStateTxt(Sy3V2ReqStateCB.SelectedIndex); } }
                    if (syIdx == 3) { if (vId == 1) { Sy4V1StateTxt.Text = getValveStateTxt(Sy4V1ReqStateCB.SelectedIndex); } else if (vId == 2) { Sy4V2StateTxt.Text = getValveStateTxt(Sy4V2ReqStateCB.SelectedIndex); } }
                    msg = "Syringe " + (syIdx + 1).ToString() + " Valve " + (vId).ToString() + " reached its target position";
                }
                else if (numdata == 99)
                {
                    msg = emergencyStopped(-1);
                }
                else if (numdata == 0)
                {
                    msg = "Command Unknown to SB";
                }
                else
                {
                    msg = "Illegal Response: " + indata;
                }

                if (isConnected == (int)connStates.connected)
                {
                    manualCmdSBMsg.Text = msg;
                    SBInfo.Text = msg;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(msg);
                }

                if (macroPointer >= 0)
                {
                    macroPointer++;
                    if (macroPointer < currentMacro.Count)
                    {
                        Thread.Sleep(10);
                        sendCmdToBot(currentMacro[macroPointer]);
                    }
                    else
                    {
                        macroPointer = -1;
                    }
                }

            }

            // B Messages are all about connection related issues
            else if (firstchar == 'B')
            {
                int numdata = -1;
                Int32.TryParse(indata.Substring(1), out numdata);
                if (isConnected == (int)connStates.neverConnected)
                {
                    if (numdata == 22) { isConnected = (int)connStates.connected; }
                }
                else if (isConnected == (int)connStates.disconnected)
                {
                    if (numdata == 33) { isConnected = (int)connStates.connected; }
                }
                else if (isConnected == (int)connStates.connected)
                {
                    if (numdata == 44) { isConnected = (int)connStates.disconnected; }
                }

                // Set all connection comps accordingly
                if (isConnected == (int)connStates.connected)
                {
                    connStatus.Text = "ONLINE";
                    connStatus.ForeColor = Color.Lime;
                    connectBtn.Text = "Disconnect";
                    connectBtn.ForeColor = Color.Crimson;
                    connectBtn.Enabled = true;
                    manualCmdGB.Enabled = true;
                    SyringeGB.Enabled = true;
                    macroManagerPanel.Enabled = true;
                }
                else
                {
                    connStatus.Text = "OFFLINE";
                    connStatus.ForeColor = Color.Crimson;
                    connectBtn.Text = "Connect";
                    connectBtn.ForeColor = Color.Lime;
                    connectBtn.Enabled = true;
                    manualCmdGB.Enabled = false;
                    SyringeGB.Enabled = false;
                    macroManagerPanel.Enabled = false;
                }
            }
        }


        /// <summary>
        /// Event Manager: when the form timer reach its target
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            if (isConnected == (int)connStates.neverConnected)
            {
                connStatus.ForeColor = Color.Crimson;
                connStatus.Text = "NO SYRINGE-BOT DEVICE DETECTED";
            }
        }


        /// <summary>
        /// Event Manager: Send Manual command for pump
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendPumpCmdBtn_Click(object sender, EventArgs e)
        {
            if (syringeSelect1.SelectedIndex != -1 && dirSelect.SelectedIndex != -1 && !string.IsNullOrEmpty(numTurns.Text))
            {
                emergencyStopped(syringeSelect1.SelectedIndex);
                string theDir = (dirSelect.SelectedIndex == 0) ? "" : "-";
                string cmd = "P" + syringeSelect1.SelectedIndex.ToString() + theDir + numTurns.Value.ToString();
                manualCmdSBMsg.Text = "Sent Pump and Waiting... (" + cmd + ")";
                sendCmdToBot(cmd);
            }
        }


        /// <summary>
        /// Event Manager: Send Manual command for Valve
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendValveCmdBtn_Click(object sender, EventArgs e)
        {
            if (syringeSelect2.SelectedIndex != -1 && valveSelect.SelectedIndex != -1 && !string.IsNullOrEmpty(numAngles.Text))
            {
                emergencyStopped(syringeSelect2.SelectedIndex);
                string cmd = "V" + syringeSelect2.SelectedIndex.ToString() + (valveSelect.SelectedIndex + 1).ToString() + numAngles.Value.ToString();
                manualCmdSBMsg.Text = "Sent Valve and Waiting... (" + cmd + ")";
                sendCmdToBot(cmd);
            }
        }


        /// <summary>
        /// Event Manager: Send Manual Stop All command for SB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void manualStopBtn_Click(object sender, EventArgs e)
        {
            sendCmdToBot("S");
        }


        /// <summary>
        /// Event Manager: Send SB Reset/Init All command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetBtn_Click(object sender, EventArgs e)
        {
            isResetting = NO_OF_SYRINGES;
            Sy1V1ReqStateCB.SelectedIndex = 0;
            Sy1V2ReqStateCB.SelectedIndex = 0;
            Sy2V1ReqStateCB.SelectedIndex = 0;
            Sy2V2ReqStateCB.SelectedIndex = 0;
            Sy3V1ReqStateCB.SelectedIndex = 0;
            Sy3V2ReqStateCB.SelectedIndex = 0;
            Sy4V1ReqStateCB.SelectedIndex = 0;
            Sy4V2ReqStateCB.SelectedIndex = 0;
            resetBtn.Enabled = false;
            sendCmdToBot("R");
        }


        /// <summary>
        /// Event Manager: Send SB Stop All command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopBtn_Click(object sender, EventArgs e)
        {
            sendCmdToBot("S");
        }


        /// <summary>
        /// Event Manager: if requested ml value track bar change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sy1ReqValTrackBar_Scroll(object sender, EventArgs e)
        {
            Sy1ReqValTxt.Text = Sy1ReqValTrackBar.Value.ToString() + "ml";
        }


        /// <summary>
        /// Event Manager: if requested ml value track bar change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sy2ReqValTrackBar_Scroll(object sender, EventArgs e)
        {
            Sy2ReqValTxt.Text = Sy2ReqValTrackBar.Value.ToString() + "ml";
        }


        /// <summary>
        /// Event Manager: if requested ml value track bar change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sy3ReqValTrackBar_Scroll(object sender, EventArgs e)
        {
            Sy3ReqValTxt.Text = Sy3ReqValTrackBar.Value.ToString() + "ml";
        }


        /// <summary>
        /// Event Manager: if requested ml value track bar change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sy4ReqValTrackBar_Scroll(object sender, EventArgs e)
        {
            Sy4ReqValTxt.Text = Sy4ReqValTrackBar.Value.ToString() + "ml";
        }


        /// <summary>
        /// Event Manager: if SB current value track bar change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sy1CurrValTrackBar_ValueChanged(object sender, EventArgs e)
        {
            Sy1CurrValTxt.Text = Sy1CurrValTrackBar.Value.ToString() + "ml";
        }


        /// <summary>
        /// Event Manager: if SB current value track bar change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sy2CurrValTrackBar_ValueChanged(object sender, EventArgs e)
        {
            Sy2CurrValTxt.Text = Sy2CurrValTrackBar.Value.ToString() + "ml";
        }


        /// <summary>
        /// Event Manager: if SB current value track bar change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sy3CurrValTrackBar_ValueChanged(object sender, EventArgs e)
        {
            Sy3CurrValTxt.Text = Sy3CurrValTrackBar.Value.ToString() + "ml";
        }


        /// <summary>
        /// Event Manager: if SB current value track bar change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sy4CurrValTrackBar_ValueChanged(object sender, EventArgs e)
        {
            Sy4CurrValTxt.Text = Sy4CurrValTrackBar.Value.ToString() + "ml";
        }


        /// <summary>
        /// Event Manager: if send pump command clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sy1ActivatePumpBtn_Click(object sender, EventArgs e)
        {
            execPumpCmd(0);
        }

        private void Sy2ActivatePumpBtn_Click(object sender, EventArgs e)
        {
            execPumpCmd(1);
        }

        private void Sy3ActivatePumpBtn_Click(object sender, EventArgs e)
        {
            execPumpCmd(2);
        }

        private void Sy4ActivatePumpBtn_Click(object sender, EventArgs e)
        {
            execPumpCmd(3);
        }


        /// <summary>
        /// Event Manager: if send valve command clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sy1V1ActivateBtn_Click(object sender, EventArgs e)
        {
            execValveCmd(0, 1);
        }

        private void Sy1V2ActivateBtn_Click(object sender, EventArgs e)
        {
            execValveCmd(0, 2);
        }

        private void Sy2V1ActivateBtn_Click(object sender, EventArgs e)
        {
            execValveCmd(1, 1);
        }

        private void Sy2V2ActivateBtn_Click(object sender, EventArgs e)
        {
            execValveCmd(1, 2);
        }

        private void Sy3V1ActivateBtn_Click(object sender, EventArgs e)
        {
            execValveCmd(2, 1);
        }

        private void Sy3V2ActivateBtn_Click(object sender, EventArgs e)
        {
            execValveCmd(2, 2);
        }

        private void Sy4V1ActivateBtn_Click(object sender, EventArgs e)
        {
            execValveCmd(3, 1);
        }

        private void Sy4V2ActivateBtn_Click(object sender, EventArgs e)
        {
            execValveCmd(3, 2);
        }

        private void loadMacroBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Get the full file path selected by the user
                        string userFilePath = openFileDialog.FileName;

                        // Read all lines from the file
                        string[] lines = File.ReadAllLines(userFilePath);

                        // Process the lines as needed
                        // Assuming currentMacro is a List<string>
                        currentMacro.Clear();
                        currentMacro.AddRange(lines);

                        // For demonstration, let's output the lines to a list box or other control
                        currLoadedMacro.Text = "";
                        foreach (string str in currentMacro)
                        {
                            currLoadedMacro.Text += str + "\r\n";
                            //System.Diagnostics.Debug.WriteLine(str);
                        }

                        //MessageBox.Show("File loaded successfully.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading file: {ex.Message}");
                    }
                }
            }

            
        }

        private void sendMacroBtn_Click(object sender, EventArgs e)
        {
            if (Sy1Panel.Enabled == true && Sy2Panel.Enabled == true && Sy3Panel.Enabled == true && Sy4Panel.Enabled == true)
            {
                if (currentMacro.Count > 0)
                {
                    macroPointer = 0;
                    sendCmdToBot(currentMacro[macroPointer]);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No macro loaded");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Bot not properly prepared");
            }
        }

        private void InitS1Btn_Click(object sender, EventArgs e)
        {
            isInit = 1;
            InitS1Btn.Enabled = false;
            Sy1Panel.Enabled = false;
            Sy1V1ReqStateCB.SelectedIndex = 0;
            Sy1V2ReqStateCB.SelectedIndex = 0;
            execInitSyringeCmd(0);
        }

        private void InitS2Btn_Click(object sender, EventArgs e)
        {
            isInit = 2;
            InitS2Btn.Enabled = false;
            Sy2Panel.Enabled = false;
            Sy2V1ReqStateCB.SelectedIndex = 0;
            Sy2V2ReqStateCB.SelectedIndex = 0;
            execInitSyringeCmd(1);
        }

        private void InitS3Btn_Click(object sender, EventArgs e)
        {
            isInit = 3;
            InitS3Btn.Enabled = false;
            Sy3Panel.Enabled = false;
            Sy3V1ReqStateCB.SelectedIndex = 0;
            Sy3V2ReqStateCB.SelectedIndex = 0;
            execInitSyringeCmd(2);
        }

        private void InitS4Btn_Click(object sender, EventArgs e)
        {
            isInit = 4;
            InitS4Btn.Enabled = false;
            Sy4Panel.Enabled = false;
            Sy4V1ReqStateCB.SelectedIndex = 0;
            Sy4V2ReqStateCB.SelectedIndex = 0;
            execInitSyringeCmd(3);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recordMacroBtn_Click(object sender, EventArgs e)
        {
            if (!macroIsRecording)
            {
                if (Sy1CurrValTrackBar.Value == 60 && Sy2CurrValTrackBar.Value == 60 && Sy3CurrValTrackBar.Value == 10 && Sy4CurrValTrackBar.Value == 10)
                {
                    currentMacro.Clear();
                    currLoadedMacro.Text = "";
                    macroIsRecording = true;
                    recordMacroBtn.BackColor = Color.Red;
                    recordMacroBtn.Text = "STOP MACRO RECORDING";
                }
                else
                {
                    MessageBox.Show("Macros can only be recorded if all syringes are in start positions of max top");
                }
            }
            else
            {
                macroIsRecording = false;
                recordMacroBtn.BackColor = SystemColors.Control;
                recordMacroBtn.Text = "START MACRO RECORDING";
            }
        }


        /// <summary>
        /// Sets the selected syringe to have the volume ml value applied and let the user interact
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void calibrateForSetPosBtn_Click(object sender, EventArgs e)
        {
            int syIdx = syringeSelect1.SelectedIndex;
            int volume = Convert.ToInt32(Math.Round(observedVolume.Value, 0));

            if (syIdx == 0)
            {
                Sy1CurrValTrackBar.Value = volume;
                Sy1CurrValTxt.Text = volume.ToString() + "ml";
                Sy1CurrValTrackBar.BackColor = SystemColors.Control;
                Sy1StateInfo.ForeColor = Color.Lime;
                Sy1StateInfo.Text = "Ready!";
                Sy1Panel.Enabled = true;
            }
            else if (syIdx == 1)
            {
                Sy2CurrValTrackBar.Value = volume;
                Sy2CurrValTxt.Text = volume.ToString() + "ml";
                Sy2CurrValTrackBar.BackColor = SystemColors.Control;
                Sy2StateInfo.ForeColor = Color.Lime;
                Sy2StateInfo.Text = "Ready!";
                Sy2Panel.Enabled = true;
            }
            else if (syIdx == 2)
            {
                Sy3CurrValTrackBar.Value = volume;
                Sy3CurrValTxt.Text = volume.ToString() + "ml";
                Sy3CurrValTrackBar.BackColor = SystemColors.Control;
                Sy3StateInfo.ForeColor = Color.Lime;
                Sy3StateInfo.Text = "Ready!";
                Sy3Panel.Enabled = true;
            }
            else if (syIdx == 3)
            {
                Sy4CurrValTrackBar.Value = volume;
                Sy4CurrValTxt.Text = volume.ToString() + "ml";
                Sy4CurrValTrackBar.BackColor = SystemColors.Control;
                Sy4StateInfo.ForeColor = Color.Lime;
                Sy4StateInfo.Text = "Ready!";
                Sy4Panel.Enabled = true;
            }
        }

        private void saveMacroBtn_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the full file path entered by the user
                    string userFilePath = saveFileDialog.FileName;

                    // Ensure the file has the desired extension (e.g., ".txt")
                    string fullFileName = Path.ChangeExtension(userFilePath, ".txt");

                    // Write list content to the file
                    string[] lines = currentMacro.ToArray();
                    File.WriteAllLines(fullFileName, lines);
                }
            }
        }

        /// <summary>
        /// creates a macro for specified syringe to fill the tubes with liquid
        /// </summary>
        /// <param name="syIdx"></param>
        private void prepTubingMacro(int syIdx)
        {
            double spinPerMl = SPIN_PER_ML_60;
            int maxSpin = BIG_SYRINGE_MAX;
            int fillSpin = 10;
            if (syIdx > 1) { spinPerMl = SPIN_PER_ML_10; maxSpin = SMALL_SYRINGE_MAX; fillSpin = 6; }
            currentMacro.Clear();
            currentMacro.Add("V" + syIdx.ToString() + "190");
            currentMacro.Add("V" + syIdx.ToString() + "290");
            double mlToSpin = maxSpin * spinPerMl;
            currentMacro.Add("P" + syIdx.ToString() + "-" + mlToSpin.ToString());
            mlToSpin = fillSpin * spinPerMl;
            currentMacro.Add("P" + syIdx.ToString() + mlToSpin.ToString());
            currentMacro.Add("V" + syIdx.ToString() + "20");
            currentMacro.Add("V" + syIdx.ToString() + "1170");
            currentMacro.Add("P" + syIdx.ToString() + "-" + mlToSpin.ToString());
        }

        private void calibS1TubesBtn_Click(object sender, EventArgs e)
        {
            if(Sy1CurrValTrackBar.Value == 60)
            {
                prepTubingMacro(0);
                macroPointer = 0;
                sendCmdToBot(currentMacro[macroPointer]);
            }
        }

        private void calibS2TubesBtn_Click(object sender, EventArgs e)
        {
            if (Sy2CurrValTrackBar.Value == 60)
            {
                prepTubingMacro(1);
                macroPointer = 0;
                sendCmdToBot(currentMacro[macroPointer]);
            }
        }

        private void calibS3TubesBtn_Click(object sender, EventArgs e)
        {
            if (Sy3CurrValTrackBar.Value == 10)
            {
                prepTubingMacro(2);
                macroPointer = 0;
                sendCmdToBot(currentMacro[macroPointer]);
            }
        }

        private void calibS4TubesBtn_Click(object sender, EventArgs e)
        {
            if (Sy4CurrValTrackBar.Value == 10)
            {
                prepTubingMacro(3);
                macroPointer = 0;
                sendCmdToBot(currentMacro[macroPointer]);
            }
        }

        // System.Diagnostics.Debug.WriteLine("Bot not properly prepared");
        // MessageBox.Show($"Error saving file: {ex.Message}");

    }
}
