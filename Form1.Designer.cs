namespace TakahashiGroup_SyringeBotGUI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            tgLogo = new PictureBox();
            huLogo = new PictureBox();
            tgTitle = new Label();
            huTitle = new Label();
            appTitle = new PictureBox();
            panel1 = new Panel();
            panel3 = new Panel();
            panel4 = new Panel();
            panel2 = new Panel();
            availableCOMPorts = new ComboBox();
            connStatus = new Label();
            connectBtn = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            sendCommandLable = new Label();
            commandText = new TextBox();
            sendCmdBtn = new Button();
            cmdHistory = new ListBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            ((System.ComponentModel.ISupportInitialize)tgLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)huLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)appTitle).BeginInit();
            panel3.SuspendLayout();
            panel4.SuspendLayout();
            SuspendLayout();
            // 
            // tgLogo
            // 
            tgLogo.Image = (Image)resources.GetObject("tgLogo.Image");
            tgLogo.Location = new Point(3, 3);
            tgLogo.Name = "tgLogo";
            tgLogo.Size = new Size(227, 221);
            tgLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            tgLogo.TabIndex = 2;
            tgLogo.TabStop = false;
            // 
            // huLogo
            // 
            huLogo.BackColor = SystemColors.Control;
            huLogo.Image = (Image)resources.GetObject("huLogo.Image");
            huLogo.Location = new Point(3, 3);
            huLogo.Name = "huLogo";
            huLogo.Size = new Size(227, 221);
            huLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            huLogo.TabIndex = 3;
            huLogo.TabStop = false;
            // 
            // tgTitle
            // 
            tgTitle.AutoSize = true;
            tgTitle.Font = new Font("Impact", 24F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tgTitle.Location = new Point(392, 9);
            tgTitle.Name = "tgTitle";
            tgTitle.Size = new Size(352, 60);
            tgTitle.TabIndex = 4;
            tgTitle.Text = "Takahashi Group";
            tgTitle.TextAlign = ContentAlignment.TopCenter;
            // 
            // huTitle
            // 
            huTitle.AutoSize = true;
            huTitle.Font = new Font("Javanese Text", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            huTitle.Location = new Point(453, 55);
            huTitle.Name = "huTitle";
            huTitle.Size = new Size(238, 54);
            huTitle.TabIndex = 5;
            huTitle.Text = "Hokkaido University";
            huTitle.TextAlign = ContentAlignment.TopCenter;
            // 
            // appTitle
            // 
            appTitle.Image = (Image)resources.GetObject("appTitle.Image");
            appTitle.Location = new Point(251, 123);
            appTitle.Name = "appTitle";
            appTitle.Size = new Size(636, 70);
            appTitle.SizeMode = PictureBoxSizeMode.StretchImage;
            appTitle.TabIndex = 8;
            appTitle.TabStop = false;
            // 
            // panel1
            // 
            panel1.BackColor = Color.DarkRed;
            panel1.Location = new Point(252, 123);
            panel1.Name = "panel1";
            panel1.Size = new Size(635, 4);
            panel1.TabIndex = 9;
            // 
            // panel3
            // 
            panel3.BackColor = Color.Maroon;
            panel3.Controls.Add(tgLogo);
            panel3.Location = new Point(12, 12);
            panel3.Name = "panel3";
            panel3.Size = new Size(233, 227);
            panel3.TabIndex = 11;
            // 
            // panel4
            // 
            panel4.BackColor = Color.Maroon;
            panel4.Controls.Add(huLogo);
            panel4.Location = new Point(894, 12);
            panel4.Name = "panel4";
            panel4.Size = new Size(233, 228);
            panel4.TabIndex = 12;
            // 
            // panel2
            // 
            panel2.BackColor = Color.DarkRed;
            panel2.Location = new Point(252, 189);
            panel2.Name = "panel2";
            panel2.Size = new Size(635, 4);
            panel2.TabIndex = 10;
            // 
            // availableCOMPorts
            // 
            availableCOMPorts.FormattingEnabled = true;
            availableCOMPorts.Location = new Point(577, 206);
            availableCOMPorts.Name = "availableCOMPorts";
            availableCOMPorts.Size = new Size(192, 33);
            availableCOMPorts.TabIndex = 13;
            // 
            // connStatus
            // 
            connStatus.AutoSize = true;
            connStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            connStatus.ForeColor = Color.Crimson;
            connStatus.Location = new Point(252, 211);
            connStatus.Name = "connStatus";
            connStatus.Size = new Size(83, 25);
            connStatus.TabIndex = 17;
            connStatus.Text = "OFFLINE";
            // 
            // connectBtn
            // 
            connectBtn.Enabled = false;
            connectBtn.Location = new Point(775, 205);
            connectBtn.Name = "connectBtn";
            connectBtn.Size = new Size(112, 34);
            connectBtn.TabIndex = 18;
            connectBtn.Text = "Connect";
            connectBtn.UseVisualStyleBackColor = true;
            connectBtn.Click += ConnectBtn_Click;
            // 
            // timer1
            // 
            timer1.Interval = 5000;
            timer1.Tick += timer1_Tick;
            // 
            // sendCommandLable
            // 
            sendCommandLable.AutoSize = true;
            sendCommandLable.Location = new Point(15, 360);
            sendCommandLable.Name = "sendCommandLable";
            sendCommandLable.Size = new Size(167, 25);
            sendCommandLable.TabIndex = 19;
            sendCommandLable.Text = "Command to Send:";
            // 
            // commandText
            // 
            commandText.Location = new Point(15, 388);
            commandText.Name = "commandText";
            commandText.Size = new Size(189, 31);
            commandText.TabIndex = 20;
            // 
            // sendCmdBtn
            // 
            sendCmdBtn.Enabled = false;
            sendCmdBtn.Location = new Point(210, 385);
            sendCmdBtn.Name = "sendCmdBtn";
            sendCmdBtn.Size = new Size(112, 34);
            sendCmdBtn.TabIndex = 21;
            sendCmdBtn.Text = "SEND";
            sendCmdBtn.UseVisualStyleBackColor = true;
            sendCmdBtn.Click += sendCmdBtn_Click;
            // 
            // cmdHistory
            // 
            cmdHistory.FormattingEnabled = true;
            cmdHistory.ItemHeight = 25;
            cmdHistory.Location = new Point(15, 425);
            cmdHistory.Name = "cmdHistory";
            cmdHistory.Size = new Size(189, 179);
            cmdHistory.TabIndex = 22;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(392, 385);
            label1.Name = "label1";
            label1.Size = new Size(33, 25);
            label1.TabIndex = 23;
            label1.Text = "---";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(29, 281);
            label2.Name = "label2";
            label2.Size = new Size(33, 25);
            label2.TabIndex = 24;
            label2.Text = "---";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(394, 283);
            label3.Name = "label3";
            label3.Size = new Size(123, 25);
            label3.TabIndex = 25;
            label3.Text = "Endstops Info";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Info;
            ClientSize = new Size(1139, 686);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(cmdHistory);
            Controls.Add(sendCmdBtn);
            Controls.Add(commandText);
            Controls.Add(sendCommandLable);
            Controls.Add(connectBtn);
            Controls.Add(connStatus);
            Controls.Add(availableCOMPorts);
            Controls.Add(panel2);
            Controls.Add(panel4);
            Controls.Add(panel3);
            Controls.Add(panel1);
            Controls.Add(appTitle);
            Controls.Add(tgTitle);
            Controls.Add(huTitle);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Takahashi Group Syringe-Bot Controller Application";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)tgLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)huLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)appTitle).EndInit();
            panel3.ResumeLayout(false);
            panel4.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private PictureBox tgLogo;
        private PictureBox huLogo;
        private Label tgTitle;
        private Label huTitle;
        private PictureBox appTitle;
        private Panel panel1;
        private Panel panel3;
        private Panel panel4;
        private Panel panel2;
        private ComboBox availableCOMPorts;
        private Label connStatus;
        private Button connectBtn;
        private System.Windows.Forms.Timer timer1;
        private Label sendCommandLable;
        private TextBox commandText;
        private Button sendCmdBtn;
        private ListBox cmdHistory;
        private Label label1;
        private Label label2;
        private Label label3;
    }
}
