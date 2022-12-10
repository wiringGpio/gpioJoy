
namespace GpioJoy
{
    partial class JoystickTab
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.buttonConnectJoystick = new System.Windows.Forms.Button();
            this.comboBoxJoystickPaths = new System.Windows.Forms.ComboBox();
            this.backgroundWorkerConnectJoystick = new System.ComponentModel.BackgroundWorker();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRefresh.Location = new System.Drawing.Point(378, 77);
            this.buttonRefresh.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(116, 37);
            this.buttonRefresh.TabIndex = 19;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // buttonConnectJoystick
            // 
            this.buttonConnectJoystick.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonConnectJoystick.Location = new System.Drawing.Point(378, 124);
            this.buttonConnectJoystick.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonConnectJoystick.Name = "buttonConnectJoystick";
            this.buttonConnectJoystick.Size = new System.Drawing.Size(116, 37);
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
            this.comboBoxJoystickPaths.Location = new System.Drawing.Point(34, 84);
            this.comboBoxJoystickPaths.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comboBoxJoystickPaths.Name = "comboBoxJoystickPaths";
            this.comboBoxJoystickPaths.Size = new System.Drawing.Size(326, 24);
            this.comboBoxJoystickPaths.TabIndex = 18;
            // 
            // backgroundWorkerConnectJoystick
            // 
            this.backgroundWorkerConnectJoystick.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerConnectJoystick_DoWork);
            this.backgroundWorkerConnectJoystick.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerConnectJoystick_RunWorkerCompleted);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 20);
            this.label1.TabIndex = 20;
            this.label1.Text = "Available Controllers";
            // 
            // JoystickTab
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonConnectJoystick);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.comboBoxJoystickPaths);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "JoystickTab";
            this.Size = new System.Drawing.Size(785, 448);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Button buttonConnectJoystick;
        private System.Windows.Forms.ComboBox comboBoxJoystickPaths;
        private System.ComponentModel.BackgroundWorker backgroundWorkerConnectJoystick;
        private System.Windows.Forms.Label label1;
    }
}
