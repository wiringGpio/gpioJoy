namespace GpioJoy
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.backgroundWorkerConnection = new System.ComponentModel.BackgroundWorker();
            this.tabControlRobot = new System.Windows.Forms.TabControl();
            this.tabPageRobot = new System.Windows.Forms.TabPage();
            this.labelConfigFile = new System.Windows.Forms.Label();
            this.buttonDPR = new System.Windows.Forms.Button();
            this.buttonDPD = new System.Windows.Forms.Button();
            this.buttonDPU = new System.Windows.Forms.Button();
            this.buttonDPL = new System.Windows.Forms.Button();
            this.buttonRS = new System.Windows.Forms.Button();
            this.buttonLS = new System.Windows.Forms.Button();
            this.labelRT = new System.Windows.Forms.Label();
            this.buttonRB = new System.Windows.Forms.Button();
            this.trackBarRT = new System.Windows.Forms.TrackBar();
            this.labelRSL = new System.Windows.Forms.Label();
            this.labelRSR = new System.Windows.Forms.Label();
            this.labelRSD = new System.Windows.Forms.Label();
            this.labelRSU = new System.Windows.Forms.Label();
            this.trackBarRSL = new System.Windows.Forms.TrackBar();
            this.trackBarRSR = new System.Windows.Forms.TrackBar();
            this.trackBarRSD = new System.Windows.Forms.TrackBar();
            this.trackBarRSU = new System.Windows.Forms.TrackBar();
            this.labelLSL = new System.Windows.Forms.Label();
            this.labelLSR = new System.Windows.Forms.Label();
            this.labelLSD = new System.Windows.Forms.Label();
            this.labelLSU = new System.Windows.Forms.Label();
            this.labelLT = new System.Windows.Forms.Label();
            this.buttonB = new System.Windows.Forms.Button();
            this.buttonA = new System.Windows.Forms.Button();
            this.buttonY = new System.Windows.Forms.Button();
            this.buttonX = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonHome = new System.Windows.Forms.Button();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonLB = new System.Windows.Forms.Button();
            this.trackBarLT = new System.Windows.Forms.TrackBar();
            this.trackBarLSL = new System.Windows.Forms.TrackBar();
            this.trackBarLSR = new System.Windows.Forms.TrackBar();
            this.trackBarLSD = new System.Windows.Forms.TrackBar();
            this.trackBarLSU = new System.Windows.Forms.TrackBar();
            this.groupBoxJoystick = new System.Windows.Forms.GroupBox();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.buttonConnectJoystick = new System.Windows.Forms.Button();
            this.comboBoxJoystickPaths = new System.Windows.Forms.ComboBox();
            this.tabPageGpio = new System.Windows.Forms.TabPage();
            this.gpioTab = new GpioJoy.GpioTab();
            this.backgroundWorkerDisconnect = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerConnectJoystick = new System.ComponentModel.BackgroundWorker();
            this.gpioTab1 = new GpioJoy.GpioTab();
            this.tabControlRobot.SuspendLayout();
            this.tabPageRobot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRSL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRSR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRSD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRSU)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLSL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLSR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLSD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLSU)).BeginInit();
            this.groupBoxJoystick.SuspendLayout();
            this.tabPageGpio.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlRobot
            // 
            this.tabControlRobot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlRobot.Controls.Add(this.tabPageRobot);
            this.tabControlRobot.Controls.Add(this.tabPageGpio);
            this.tabControlRobot.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControlRobot.Location = new System.Drawing.Point(12, 12);
            this.tabControlRobot.Name = "tabControlRobot";
            this.tabControlRobot.SelectedIndex = 0;
            this.tabControlRobot.Size = new System.Drawing.Size(804, 465);
            this.tabControlRobot.TabIndex = 2;
            // 
            // tabPageRobot
            // 
            this.tabPageRobot.Controls.Add(this.labelConfigFile);
            this.tabPageRobot.Controls.Add(this.buttonDPR);
            this.tabPageRobot.Controls.Add(this.buttonDPD);
            this.tabPageRobot.Controls.Add(this.buttonDPU);
            this.tabPageRobot.Controls.Add(this.buttonDPL);
            this.tabPageRobot.Controls.Add(this.buttonRS);
            this.tabPageRobot.Controls.Add(this.buttonLS);
            this.tabPageRobot.Controls.Add(this.labelRT);
            this.tabPageRobot.Controls.Add(this.buttonRB);
            this.tabPageRobot.Controls.Add(this.trackBarRT);
            this.tabPageRobot.Controls.Add(this.labelRSL);
            this.tabPageRobot.Controls.Add(this.labelRSR);
            this.tabPageRobot.Controls.Add(this.labelRSD);
            this.tabPageRobot.Controls.Add(this.labelRSU);
            this.tabPageRobot.Controls.Add(this.trackBarRSL);
            this.tabPageRobot.Controls.Add(this.trackBarRSR);
            this.tabPageRobot.Controls.Add(this.trackBarRSD);
            this.tabPageRobot.Controls.Add(this.trackBarRSU);
            this.tabPageRobot.Controls.Add(this.labelLSL);
            this.tabPageRobot.Controls.Add(this.labelLSR);
            this.tabPageRobot.Controls.Add(this.labelLSD);
            this.tabPageRobot.Controls.Add(this.labelLSU);
            this.tabPageRobot.Controls.Add(this.labelLT);
            this.tabPageRobot.Controls.Add(this.buttonB);
            this.tabPageRobot.Controls.Add(this.buttonA);
            this.tabPageRobot.Controls.Add(this.buttonY);
            this.tabPageRobot.Controls.Add(this.buttonX);
            this.tabPageRobot.Controls.Add(this.buttonStart);
            this.tabPageRobot.Controls.Add(this.buttonHome);
            this.tabPageRobot.Controls.Add(this.buttonBack);
            this.tabPageRobot.Controls.Add(this.buttonLB);
            this.tabPageRobot.Controls.Add(this.trackBarLT);
            this.tabPageRobot.Controls.Add(this.trackBarLSL);
            this.tabPageRobot.Controls.Add(this.trackBarLSR);
            this.tabPageRobot.Controls.Add(this.trackBarLSD);
            this.tabPageRobot.Controls.Add(this.trackBarLSU);
            this.tabPageRobot.Controls.Add(this.groupBoxJoystick);
            this.tabPageRobot.Location = new System.Drawing.Point(4, 29);
            this.tabPageRobot.Name = "tabPageRobot";
            this.tabPageRobot.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageRobot.Size = new System.Drawing.Size(796, 432);
            this.tabPageRobot.TabIndex = 0;
            this.tabPageRobot.Text = "Home";
            this.tabPageRobot.UseVisualStyleBackColor = true;
            // 
            // labelConfigFile
            // 
            this.labelConfigFile.Location = new System.Drawing.Point(276, 59);
            this.labelConfigFile.Name = "labelConfigFile";
            this.labelConfigFile.Size = new System.Drawing.Size(203, 20);
            this.labelConfigFile.TabIndex = 61;
            this.labelConfigFile.Text = "label1";
            this.labelConfigFile.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // buttonDPR
            // 
            this.buttonDPR.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDPR.Location = new System.Drawing.Point(348, 311);
            this.buttonDPR.Name = "buttonDPR";
            this.buttonDPR.Size = new System.Drawing.Size(61, 44);
            this.buttonDPR.TabIndex = 60;
            this.buttonDPR.Text = ">";
            this.buttonDPR.UseVisualStyleBackColor = true;
            this.buttonDPR.Click += new System.EventHandler(this.buttonDPL_Click);
            // 
            // buttonDPD
            // 
            this.buttonDPD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDPD.Location = new System.Drawing.Point(307, 361);
            this.buttonDPD.Name = "buttonDPD";
            this.buttonDPD.Size = new System.Drawing.Size(61, 44);
            this.buttonDPD.TabIndex = 59;
            this.buttonDPD.Text = "_";
            this.buttonDPD.UseVisualStyleBackColor = true;
            // 
            // buttonDPU
            // 
            this.buttonDPU.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDPU.Location = new System.Drawing.Point(307, 261);
            this.buttonDPU.Name = "buttonDPU";
            this.buttonDPU.Size = new System.Drawing.Size(61, 44);
            this.buttonDPU.TabIndex = 58;
            this.buttonDPU.Text = "^";
            this.buttonDPU.UseVisualStyleBackColor = true;
            // 
            // buttonDPL
            // 
            this.buttonDPL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDPL.Location = new System.Drawing.Point(264, 311);
            this.buttonDPL.Name = "buttonDPL";
            this.buttonDPL.Size = new System.Drawing.Size(61, 44);
            this.buttonDPL.TabIndex = 57;
            this.buttonDPL.Text = "<";
            this.buttonDPL.UseVisualStyleBackColor = true;
            this.buttonDPL.Click += new System.EventHandler(this.buttonDPR_Click);
            // 
            // buttonRS
            // 
            this.buttonRS.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRS.Location = new System.Drawing.Point(585, 259);
            this.buttonRS.Name = "buttonRS";
            this.buttonRS.Size = new System.Drawing.Size(45, 37);
            this.buttonRS.TabIndex = 56;
            this.buttonRS.Text = "X";
            this.buttonRS.UseVisualStyleBackColor = true;
            // 
            // buttonLS
            // 
            this.buttonLS.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLS.Location = new System.Drawing.Point(153, 193);
            this.buttonLS.Name = "buttonLS";
            this.buttonLS.Size = new System.Drawing.Size(45, 37);
            this.buttonLS.TabIndex = 55;
            this.buttonLS.Text = "X";
            this.buttonLS.UseVisualStyleBackColor = true;
            // 
            // labelRT
            // 
            this.labelRT.Location = new System.Drawing.Point(655, 30);
            this.labelRT.Name = "labelRT";
            this.labelRT.Size = new System.Drawing.Size(135, 20);
            this.labelRT.TabIndex = 54;
            this.labelRT.Text = "label1";
            this.labelRT.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonRB
            // 
            this.buttonRB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRB.Location = new System.Drawing.Point(675, 6);
            this.buttonRB.Name = "buttonRB";
            this.buttonRB.Size = new System.Drawing.Size(94, 23);
            this.buttonRB.TabIndex = 53;
            this.buttonRB.Text = "RB";
            this.buttonRB.UseVisualStyleBackColor = true;
            // 
            // trackBarRT
            // 
            this.trackBarRT.Location = new System.Drawing.Point(700, 53);
            this.trackBarRT.Maximum = 0;
            this.trackBarRT.Minimum = -100;
            this.trackBarRT.Name = "trackBarRT";
            this.trackBarRT.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarRT.Size = new System.Drawing.Size(45, 93);
            this.trackBarRT.TabIndex = 52;
            this.trackBarRT.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // labelRSL
            // 
            this.labelRSL.Location = new System.Drawing.Point(444, 305);
            this.labelRSL.Name = "labelRSL";
            this.labelRSL.Size = new System.Drawing.Size(135, 20);
            this.labelRSL.TabIndex = 51;
            this.labelRSL.Text = "label1";
            this.labelRSL.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelRSR
            // 
            this.labelRSR.Location = new System.Drawing.Point(636, 234);
            this.labelRSR.Name = "labelRSR";
            this.labelRSR.Size = new System.Drawing.Size(135, 20);
            this.labelRSR.TabIndex = 50;
            this.labelRSR.Text = "label1";
            this.labelRSR.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelRSD
            // 
            this.labelRSD.Location = new System.Drawing.Point(540, 399);
            this.labelRSD.Name = "labelRSD";
            this.labelRSD.Size = new System.Drawing.Size(135, 20);
            this.labelRSD.TabIndex = 49;
            this.labelRSD.Text = "label1";
            this.labelRSD.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelRSU
            // 
            this.labelRSU.Location = new System.Drawing.Point(540, 138);
            this.labelRSU.Name = "labelRSU";
            this.labelRSU.Size = new System.Drawing.Size(135, 20);
            this.labelRSU.TabIndex = 48;
            this.labelRSU.Text = "label1";
            this.labelRSU.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // trackBarRSL
            // 
            this.trackBarRSL.Location = new System.Drawing.Point(486, 257);
            this.trackBarRSL.Maximum = 0;
            this.trackBarRSL.Minimum = -100;
            this.trackBarRSL.Name = "trackBarRSL";
            this.trackBarRSL.Size = new System.Drawing.Size(93, 45);
            this.trackBarRSL.TabIndex = 47;
            this.trackBarRSL.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // trackBarRSR
            // 
            this.trackBarRSR.Location = new System.Drawing.Point(636, 257);
            this.trackBarRSR.Maximum = 100;
            this.trackBarRSR.Name = "trackBarRSR";
            this.trackBarRSR.Size = new System.Drawing.Size(93, 45);
            this.trackBarRSR.TabIndex = 46;
            this.trackBarRSR.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // trackBarRSD
            // 
            this.trackBarRSD.Location = new System.Drawing.Point(585, 302);
            this.trackBarRSD.Maximum = 0;
            this.trackBarRSD.Minimum = -100;
            this.trackBarRSD.Name = "trackBarRSD";
            this.trackBarRSD.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarRSD.Size = new System.Drawing.Size(45, 93);
            this.trackBarRSD.TabIndex = 45;
            this.trackBarRSD.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // trackBarRSU
            // 
            this.trackBarRSU.Location = new System.Drawing.Point(585, 161);
            this.trackBarRSU.Maximum = 100;
            this.trackBarRSU.Name = "trackBarRSU";
            this.trackBarRSU.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarRSU.Size = new System.Drawing.Size(45, 93);
            this.trackBarRSU.TabIndex = 44;
            this.trackBarRSU.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // labelLSL
            // 
            this.labelLSL.Location = new System.Drawing.Point(12, 238);
            this.labelLSL.Name = "labelLSL";
            this.labelLSL.Size = new System.Drawing.Size(135, 20);
            this.labelLSL.TabIndex = 43;
            this.labelLSL.Text = "label1";
            this.labelLSL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelLSR
            // 
            this.labelLSR.Location = new System.Drawing.Point(205, 167);
            this.labelLSR.Name = "labelLSR";
            this.labelLSR.Size = new System.Drawing.Size(135, 20);
            this.labelLSR.TabIndex = 42;
            this.labelLSR.Text = "label1";
            this.labelLSR.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelLSD
            // 
            this.labelLSD.Location = new System.Drawing.Point(109, 333);
            this.labelLSD.Name = "labelLSD";
            this.labelLSD.Size = new System.Drawing.Size(135, 20);
            this.labelLSD.TabIndex = 41;
            this.labelLSD.Text = "label1";
            this.labelLSD.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelLSU
            // 
            this.labelLSU.Location = new System.Drawing.Point(109, 71);
            this.labelLSU.Name = "labelLSU";
            this.labelLSU.Size = new System.Drawing.Size(135, 20);
            this.labelLSU.TabIndex = 40;
            this.labelLSU.Text = "label1";
            this.labelLSU.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelLT
            // 
            this.labelLT.Location = new System.Drawing.Point(5, 30);
            this.labelLT.Name = "labelLT";
            this.labelLT.Size = new System.Drawing.Size(135, 20);
            this.labelLT.TabIndex = 39;
            this.labelLT.Text = "label1";
            this.labelLT.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonB
            // 
            this.buttonB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonB.Location = new System.Drawing.Point(585, 48);
            this.buttonB.Name = "buttonB";
            this.buttonB.Size = new System.Drawing.Size(59, 31);
            this.buttonB.TabIndex = 38;
            this.buttonB.Text = "B";
            this.buttonB.UseVisualStyleBackColor = true;
            // 
            // buttonA
            // 
            this.buttonA.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonA.Location = new System.Drawing.Point(544, 85);
            this.buttonA.Name = "buttonA";
            this.buttonA.Size = new System.Drawing.Size(59, 31);
            this.buttonA.TabIndex = 37;
            this.buttonA.Text = "A";
            this.buttonA.UseVisualStyleBackColor = true;
            // 
            // buttonY
            // 
            this.buttonY.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonY.Location = new System.Drawing.Point(544, 11);
            this.buttonY.Name = "buttonY";
            this.buttonY.Size = new System.Drawing.Size(59, 31);
            this.buttonY.TabIndex = 36;
            this.buttonY.Text = "Y";
            this.buttonY.UseVisualStyleBackColor = true;
            // 
            // buttonX
            // 
            this.buttonX.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonX.Location = new System.Drawing.Point(502, 48);
            this.buttonX.Name = "buttonX";
            this.buttonX.Size = new System.Drawing.Size(59, 31);
            this.buttonX.TabIndex = 35;
            this.buttonX.Text = "X";
            this.buttonX.UseVisualStyleBackColor = true;
            // 
            // buttonStart
            // 
            this.buttonStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStart.Location = new System.Drawing.Point(413, 26);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(62, 23);
            this.buttonStart.TabIndex = 34;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            // 
            // buttonHome
            // 
            this.buttonHome.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonHome.Location = new System.Drawing.Point(348, 18);
            this.buttonHome.Name = "buttonHome";
            this.buttonHome.Size = new System.Drawing.Size(59, 38);
            this.buttonHome.TabIndex = 33;
            this.buttonHome.Text = "Home";
            this.buttonHome.UseVisualStyleBackColor = true;
            // 
            // buttonBack
            // 
            this.buttonBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBack.Location = new System.Drawing.Point(280, 26);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(62, 23);
            this.buttonBack.TabIndex = 32;
            this.buttonBack.Text = "Back";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // buttonLB
            // 
            this.buttonLB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLB.Location = new System.Drawing.Point(24, 6);
            this.buttonLB.Name = "buttonLB";
            this.buttonLB.Size = new System.Drawing.Size(96, 23);
            this.buttonLB.TabIndex = 31;
            this.buttonLB.Text = "LB";
            this.buttonLB.UseVisualStyleBackColor = true;
            // 
            // trackBarLT
            // 
            this.trackBarLT.Location = new System.Drawing.Point(50, 53);
            this.trackBarLT.Maximum = 0;
            this.trackBarLT.Minimum = -100;
            this.trackBarLT.Name = "trackBarLT";
            this.trackBarLT.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarLT.Size = new System.Drawing.Size(45, 93);
            this.trackBarLT.TabIndex = 30;
            this.trackBarLT.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // trackBarLSL
            // 
            this.trackBarLSL.Location = new System.Drawing.Point(54, 190);
            this.trackBarLSL.Maximum = 0;
            this.trackBarLSL.Minimum = -100;
            this.trackBarLSL.Name = "trackBarLSL";
            this.trackBarLSL.Size = new System.Drawing.Size(93, 45);
            this.trackBarLSL.TabIndex = 29;
            this.trackBarLSL.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // trackBarLSR
            // 
            this.trackBarLSR.Location = new System.Drawing.Point(204, 190);
            this.trackBarLSR.Maximum = 100;
            this.trackBarLSR.Name = "trackBarLSR";
            this.trackBarLSR.Size = new System.Drawing.Size(93, 45);
            this.trackBarLSR.TabIndex = 28;
            this.trackBarLSR.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // trackBarLSD
            // 
            this.trackBarLSD.Location = new System.Drawing.Point(154, 236);
            this.trackBarLSD.Maximum = 0;
            this.trackBarLSD.Minimum = -100;
            this.trackBarLSD.Name = "trackBarLSD";
            this.trackBarLSD.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarLSD.Size = new System.Drawing.Size(45, 93);
            this.trackBarLSD.TabIndex = 27;
            this.trackBarLSD.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // trackBarLSU
            // 
            this.trackBarLSU.Location = new System.Drawing.Point(154, 94);
            this.trackBarLSU.Maximum = 100;
            this.trackBarLSU.Name = "trackBarLSU";
            this.trackBarLSU.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarLSU.Size = new System.Drawing.Size(45, 93);
            this.trackBarLSU.TabIndex = 26;
            this.trackBarLSU.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // groupBoxJoystick
            // 
            this.groupBoxJoystick.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxJoystick.Controls.Add(this.buttonRefresh);
            this.groupBoxJoystick.Controls.Add(this.buttonConnectJoystick);
            this.groupBoxJoystick.Controls.Add(this.comboBoxJoystickPaths);
            this.groupBoxJoystick.Location = new System.Drawing.Point(6, 371);
            this.groupBoxJoystick.Name = "groupBoxJoystick";
            this.groupBoxJoystick.Size = new System.Drawing.Size(276, 55);
            this.groupBoxJoystick.TabIndex = 25;
            this.groupBoxJoystick.TabStop = false;
            this.groupBoxJoystick.Text = "Joystick";
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRefresh.Location = new System.Drawing.Point(102, 24);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(77, 24);
            this.buttonRefresh.TabIndex = 19;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // buttonConnectJoystick
            // 
            this.buttonConnectJoystick.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonConnectJoystick.Location = new System.Drawing.Point(185, 23);
            this.buttonConnectJoystick.Name = "buttonConnectJoystick";
            this.buttonConnectJoystick.Size = new System.Drawing.Size(77, 24);
            this.buttonConnectJoystick.TabIndex = 0;
            this.buttonConnectJoystick.Text = "Connect";
            this.buttonConnectJoystick.UseVisualStyleBackColor = true;
            this.buttonConnectJoystick.Click += new System.EventHandler(this.buttonConnectJoystick_Click);
            // 
            // comboBoxJoystickPaths
            // 
            this.comboBoxJoystickPaths.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxJoystickPaths.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxJoystickPaths.FormattingEnabled = true;
            this.comboBoxJoystickPaths.Location = new System.Drawing.Point(4, 24);
            this.comboBoxJoystickPaths.Name = "comboBoxJoystickPaths";
            this.comboBoxJoystickPaths.Size = new System.Drawing.Size(92, 24);
            this.comboBoxJoystickPaths.TabIndex = 18;
            // 
            // tabPageGpio
            // 
            this.tabPageGpio.Controls.Add(this.gpioTab);
            this.tabPageGpio.Location = new System.Drawing.Point(4, 22);
            this.tabPageGpio.Name = "tabPageGpio";
            this.tabPageGpio.Size = new System.Drawing.Size(192, 74);
            this.tabPageGpio.TabIndex = 4;
            this.tabPageGpio.Text = "GPIO";
            this.tabPageGpio.UseVisualStyleBackColor = true;
            // 
            // gpioTab
            // 
            this.gpioTab.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpioTab.Location = new System.Drawing.Point(0, 0);
            this.gpioTab.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gpioTab.Name = "gpioTab";
            this.gpioTab.Size = new System.Drawing.Size(793, 429);
            this.gpioTab.TabIndex = 1;
            this.gpioTab.Tag = "22";
            // 
            // backgroundWorkerConnectJoystick
            // 
            this.backgroundWorkerConnectJoystick.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerConnectJoystick_DoWork);
            this.backgroundWorkerConnectJoystick.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerConnectJoystick_RunWorkerCompleted);
            // 
            // gpioTab1
            // 
            this.gpioTab1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpioTab1.Location = new System.Drawing.Point(0, 3);
            this.gpioTab1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gpioTab1.Name = "gpioTab1";
            this.gpioTab1.Size = new System.Drawing.Size(749, 359);
            this.gpioTab1.TabIndex = 0;
            this.gpioTab1.Tag = "22";
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(828, 489);
            this.Controls.Add(this.tabControlRobot);
            this.KeyPreview = true;
            this.MaximumSize = new System.Drawing.Size(844, 528);
            this.MinimumSize = new System.Drawing.Size(844, 528);
            this.Name = "MainForm";
            this.Text = "GPIO Joy";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.tabControlRobot.ResumeLayout(false);
            this.tabPageRobot.ResumeLayout(false);
            this.tabPageRobot.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRSL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRSR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRSD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRSU)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLSL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLSR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLSD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLSU)).EndInit();
            this.groupBoxJoystick.ResumeLayout(false);
            this.tabPageGpio.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorkerConnection;
        private System.Windows.Forms.TabControl tabControlRobot;
        private System.Windows.Forms.TabPage tabPageRobot;
        private System.ComponentModel.BackgroundWorker backgroundWorkerDisconnect;
        private System.ComponentModel.BackgroundWorker backgroundWorkerConnectJoystick;
        private System.Windows.Forms.GroupBox groupBoxJoystick;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Button buttonConnectJoystick;
        private System.Windows.Forms.ComboBox comboBoxJoystickPaths;
        private System.Windows.Forms.TabPage tabPageGpio;
        private GpioTab gpioTab1;
        private GpioTab gpioTab;
        private System.Windows.Forms.Button buttonB;
        private System.Windows.Forms.Button buttonA;
        private System.Windows.Forms.Button buttonY;
        private System.Windows.Forms.Button buttonX;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonHome;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonLB;
        private System.Windows.Forms.TrackBar trackBarLT;
        private System.Windows.Forms.TrackBar trackBarLSL;
        private System.Windows.Forms.TrackBar trackBarLSR;
        private System.Windows.Forms.TrackBar trackBarLSD;
        private System.Windows.Forms.TrackBar trackBarLSU;
        private System.Windows.Forms.Label labelRSL;
        private System.Windows.Forms.Label labelRSR;
        private System.Windows.Forms.Label labelRSD;
        private System.Windows.Forms.Label labelRSU;
        private System.Windows.Forms.TrackBar trackBarRSL;
        private System.Windows.Forms.TrackBar trackBarRSR;
        private System.Windows.Forms.TrackBar trackBarRSD;
        private System.Windows.Forms.TrackBar trackBarRSU;
        private System.Windows.Forms.Label labelLSL;
        private System.Windows.Forms.Label labelLSR;
        private System.Windows.Forms.Label labelLSD;
        private System.Windows.Forms.Label labelLSU;
        private System.Windows.Forms.Label labelLT;
        private System.Windows.Forms.Label labelRT;
        private System.Windows.Forms.Button buttonRB;
        private System.Windows.Forms.TrackBar trackBarRT;
        private System.Windows.Forms.Button buttonDPR;
        private System.Windows.Forms.Button buttonDPD;
        private System.Windows.Forms.Button buttonDPU;
        private System.Windows.Forms.Button buttonDPL;
        private System.Windows.Forms.Button buttonRS;
        private System.Windows.Forms.Button buttonLS;
        private System.Windows.Forms.Label labelConfigFile;
    }
}