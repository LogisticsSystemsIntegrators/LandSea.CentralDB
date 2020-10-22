namespace Demo
{
    partial class frmMain
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
            this.gbxAuthentication = new System.Windows.Forms.GroupBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblResponseResult = new System.Windows.Forms.Label();
            this.lblResponse = new System.Windows.Forms.Label();
            this.btnAuthenticate = new System.Windows.Forms.Button();
            this.gbxSendXML = new System.Windows.Forms.GroupBox();
            this.txtSendXMLPath = new System.Windows.Forms.TextBox();
            this.btnSendXML = new System.Windows.Forms.Button();
            this.btnSelectXML = new System.Windows.Forms.Button();
            this.gbxGetXML = new System.Windows.Forms.GroupBox();
            this.rtxXMLMessageResult = new System.Windows.Forms.RichTextBox();
            this.txtMessageKey = new System.Windows.Forms.TextBox();
            this.btnGetXML = new System.Windows.Forms.Button();
            this.lblKey = new System.Windows.Forms.Label();
            this.radShipment = new System.Windows.Forms.RadioButton();
            this.radOrganization = new System.Windows.Forms.RadioButton();
            this.ofdSelectXML = new System.Windows.Forms.OpenFileDialog();
            this.gbxMarkAsProcessed = new System.Windows.Forms.GroupBox();
            this.txtProcessedMessageID = new System.Windows.Forms.TextBox();
            this.lblMarkAsProcessedMessageID = new System.Windows.Forms.Label();
            this.btnMarkAsProcessed = new System.Windows.Forms.Button();
            this.gbxAuthentication.SuspendLayout();
            this.gbxSendXML.SuspendLayout();
            this.gbxGetXML.SuspendLayout();
            this.gbxMarkAsProcessed.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxAuthentication
            // 
            this.gbxAuthentication.Controls.Add(this.txtPassword);
            this.gbxAuthentication.Controls.Add(this.txtUsername);
            this.gbxAuthentication.Controls.Add(this.lblPassword);
            this.gbxAuthentication.Controls.Add(this.lblUsername);
            this.gbxAuthentication.Controls.Add(this.lblResponseResult);
            this.gbxAuthentication.Controls.Add(this.lblResponse);
            this.gbxAuthentication.Controls.Add(this.btnAuthenticate);
            this.gbxAuthentication.Location = new System.Drawing.Point(12, 12);
            this.gbxAuthentication.Name = "gbxAuthentication";
            this.gbxAuthentication.Size = new System.Drawing.Size(776, 100);
            this.gbxAuthentication.TabIndex = 0;
            this.gbxAuthentication.TabStop = false;
            this.gbxAuthentication.Text = "Authentication";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(294, 23);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(137, 20);
            this.txtPassword.TabIndex = 6;
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(70, 23);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(137, 20);
            this.txtUsername.TabIndex = 5;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(232, 26);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(56, 13);
            this.lblPassword.TabIndex = 4;
            this.lblPassword.Text = "Password:";
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(6, 26);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(58, 13);
            this.lblUsername.TabIndex = 3;
            this.lblUsername.Text = "Username:";
            // 
            // lblResponseResult
            // 
            this.lblResponseResult.AutoSize = true;
            this.lblResponseResult.Location = new System.Drawing.Point(181, 62);
            this.lblResponseResult.Name = "lblResponseResult";
            this.lblResponseResult.Size = new System.Drawing.Size(125, 13);
            this.lblResponseResult.TabIndex = 2;
            this.lblResponseResult.Text = "Press button to get value";
            // 
            // lblResponse
            // 
            this.lblResponse.AutoSize = true;
            this.lblResponse.Location = new System.Drawing.Point(117, 62);
            this.lblResponse.Name = "lblResponse";
            this.lblResponse.Size = new System.Drawing.Size(58, 13);
            this.lblResponse.TabIndex = 1;
            this.lblResponse.Text = "Response:";
            // 
            // btnAuthenticate
            // 
            this.btnAuthenticate.Location = new System.Drawing.Point(6, 57);
            this.btnAuthenticate.Name = "btnAuthenticate";
            this.btnAuthenticate.Size = new System.Drawing.Size(75, 23);
            this.btnAuthenticate.TabIndex = 0;
            this.btnAuthenticate.Text = "Authenticate";
            this.btnAuthenticate.UseVisualStyleBackColor = true;
            this.btnAuthenticate.Click += new System.EventHandler(this.btnAuthenticate_Click);
            // 
            // gbxSendXML
            // 
            this.gbxSendXML.Controls.Add(this.txtSendXMLPath);
            this.gbxSendXML.Controls.Add(this.btnSendXML);
            this.gbxSendXML.Controls.Add(this.btnSelectXML);
            this.gbxSendXML.Location = new System.Drawing.Point(12, 118);
            this.gbxSendXML.Name = "gbxSendXML";
            this.gbxSendXML.Size = new System.Drawing.Size(776, 100);
            this.gbxSendXML.TabIndex = 1;
            this.gbxSendXML.TabStop = false;
            this.gbxSendXML.Text = "Send XML";
            // 
            // txtSendXMLPath
            // 
            this.txtSendXMLPath.Location = new System.Drawing.Point(87, 30);
            this.txtSendXMLPath.Name = "txtSendXMLPath";
            this.txtSendXMLPath.Size = new System.Drawing.Size(683, 20);
            this.txtSendXMLPath.TabIndex = 6;
            // 
            // btnSendXML
            // 
            this.btnSendXML.Location = new System.Drawing.Point(6, 71);
            this.btnSendXML.Name = "btnSendXML";
            this.btnSendXML.Size = new System.Drawing.Size(75, 23);
            this.btnSendXML.TabIndex = 2;
            this.btnSendXML.Text = "Send XML";
            this.btnSendXML.UseVisualStyleBackColor = true;
            this.btnSendXML.Click += new System.EventHandler(this.btnSendXML_Click);
            // 
            // btnSelectXML
            // 
            this.btnSelectXML.Location = new System.Drawing.Point(6, 28);
            this.btnSelectXML.Name = "btnSelectXML";
            this.btnSelectXML.Size = new System.Drawing.Size(75, 23);
            this.btnSelectXML.TabIndex = 1;
            this.btnSelectXML.Text = "Select XML";
            this.btnSelectXML.UseVisualStyleBackColor = true;
            this.btnSelectXML.Click += new System.EventHandler(this.btnSelectXML_Click);
            // 
            // gbxGetXML
            // 
            this.gbxGetXML.Controls.Add(this.rtxXMLMessageResult);
            this.gbxGetXML.Controls.Add(this.txtMessageKey);
            this.gbxGetXML.Controls.Add(this.btnGetXML);
            this.gbxGetXML.Controls.Add(this.lblKey);
            this.gbxGetXML.Controls.Add(this.radShipment);
            this.gbxGetXML.Controls.Add(this.radOrganization);
            this.gbxGetXML.Location = new System.Drawing.Point(12, 224);
            this.gbxGetXML.Name = "gbxGetXML";
            this.gbxGetXML.Size = new System.Drawing.Size(776, 278);
            this.gbxGetXML.TabIndex = 2;
            this.gbxGetXML.TabStop = false;
            this.gbxGetXML.Text = "Get XML";
            // 
            // rtxXMLMessageResult
            // 
            this.rtxXMLMessageResult.Location = new System.Drawing.Point(222, 20);
            this.rtxXMLMessageResult.Name = "rtxXMLMessageResult";
            this.rtxXMLMessageResult.Size = new System.Drawing.Size(548, 252);
            this.rtxXMLMessageResult.TabIndex = 7;
            this.rtxXMLMessageResult.Text = "";
            // 
            // txtMessageKey
            // 
            this.txtMessageKey.Enabled = false;
            this.txtMessageKey.Location = new System.Drawing.Point(40, 116);
            this.txtMessageKey.Name = "txtMessageKey";
            this.txtMessageKey.Size = new System.Drawing.Size(137, 20);
            this.txtMessageKey.TabIndex = 6;
            this.txtMessageKey.Visible = false;
            // 
            // btnGetXML
            // 
            this.btnGetXML.Location = new System.Drawing.Point(6, 71);
            this.btnGetXML.Name = "btnGetXML";
            this.btnGetXML.Size = new System.Drawing.Size(75, 23);
            this.btnGetXML.TabIndex = 5;
            this.btnGetXML.Text = "Get XML";
            this.btnGetXML.UseVisualStyleBackColor = true;
            this.btnGetXML.Click += new System.EventHandler(this.btnGetXML_Click);
            // 
            // lblKey
            // 
            this.lblKey.AutoSize = true;
            this.lblKey.Enabled = false;
            this.lblKey.Location = new System.Drawing.Point(6, 119);
            this.lblKey.Name = "lblKey";
            this.lblKey.Size = new System.Drawing.Size(28, 13);
            this.lblKey.TabIndex = 4;
            this.lblKey.Text = "Key:";
            this.lblKey.Visible = false;
            // 
            // radShipment
            // 
            this.radShipment.AutoSize = true;
            this.radShipment.Location = new System.Drawing.Point(120, 20);
            this.radShipment.Name = "radShipment";
            this.radShipment.Size = new System.Drawing.Size(69, 17);
            this.radShipment.TabIndex = 1;
            this.radShipment.TabStop = true;
            this.radShipment.Text = "Shipment";
            this.radShipment.UseVisualStyleBackColor = true;
            // 
            // radOrganization
            // 
            this.radOrganization.AutoSize = true;
            this.radOrganization.Location = new System.Drawing.Point(9, 20);
            this.radOrganization.Name = "radOrganization";
            this.radOrganization.Size = new System.Drawing.Size(84, 17);
            this.radOrganization.TabIndex = 0;
            this.radOrganization.TabStop = true;
            this.radOrganization.Text = "Organization";
            this.radOrganization.UseVisualStyleBackColor = true;
            // 
            // gbxMarkAsProcessed
            // 
            this.gbxMarkAsProcessed.Controls.Add(this.txtProcessedMessageID);
            this.gbxMarkAsProcessed.Controls.Add(this.lblMarkAsProcessedMessageID);
            this.gbxMarkAsProcessed.Controls.Add(this.btnMarkAsProcessed);
            this.gbxMarkAsProcessed.Location = new System.Drawing.Point(12, 508);
            this.gbxMarkAsProcessed.Name = "gbxMarkAsProcessed";
            this.gbxMarkAsProcessed.Size = new System.Drawing.Size(776, 100);
            this.gbxMarkAsProcessed.TabIndex = 3;
            this.gbxMarkAsProcessed.TabStop = false;
            this.gbxMarkAsProcessed.Text = "Mark as Processed";
            // 
            // txtProcessedMessageID
            // 
            this.txtProcessedMessageID.Location = new System.Drawing.Point(79, 13);
            this.txtProcessedMessageID.Name = "txtProcessedMessageID";
            this.txtProcessedMessageID.Size = new System.Drawing.Size(137, 20);
            this.txtProcessedMessageID.TabIndex = 8;
            // 
            // lblMarkAsProcessedMessageID
            // 
            this.lblMarkAsProcessedMessageID.AutoSize = true;
            this.lblMarkAsProcessedMessageID.Location = new System.Drawing.Point(6, 16);
            this.lblMarkAsProcessedMessageID.Name = "lblMarkAsProcessedMessageID";
            this.lblMarkAsProcessedMessageID.Size = new System.Drawing.Size(67, 13);
            this.lblMarkAsProcessedMessageID.TabIndex = 7;
            this.lblMarkAsProcessedMessageID.Text = "Message ID:";
            // 
            // btnMarkAsProcessed
            // 
            this.btnMarkAsProcessed.Location = new System.Drawing.Point(6, 52);
            this.btnMarkAsProcessed.Name = "btnMarkAsProcessed";
            this.btnMarkAsProcessed.Size = new System.Drawing.Size(75, 42);
            this.btnMarkAsProcessed.TabIndex = 6;
            this.btnMarkAsProcessed.Text = "Mark as Processed";
            this.btnMarkAsProcessed.UseVisualStyleBackColor = true;
            this.btnMarkAsProcessed.Click += new System.EventHandler(this.btnMarkAsProcessed_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 623);
            this.Controls.Add(this.gbxMarkAsProcessed);
            this.Controls.Add(this.gbxGetXML);
            this.Controls.Add(this.gbxSendXML);
            this.Controls.Add(this.gbxAuthentication);
            this.Name = "frmMain";
            this.Text = "Demo App";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.gbxAuthentication.ResumeLayout(false);
            this.gbxAuthentication.PerformLayout();
            this.gbxSendXML.ResumeLayout(false);
            this.gbxSendXML.PerformLayout();
            this.gbxGetXML.ResumeLayout(false);
            this.gbxGetXML.PerformLayout();
            this.gbxMarkAsProcessed.ResumeLayout(false);
            this.gbxMarkAsProcessed.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxAuthentication;
        private System.Windows.Forms.GroupBox gbxSendXML;
        private System.Windows.Forms.GroupBox gbxGetXML;
        private System.Windows.Forms.Button btnAuthenticate;
        private System.Windows.Forms.Label lblResponseResult;
        private System.Windows.Forms.Label lblResponse;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Button btnSelectXML;
        private System.Windows.Forms.OpenFileDialog ofdSelectXML;
        private System.Windows.Forms.Button btnSendXML;
        private System.Windows.Forms.TextBox txtSendXMLPath;
        private System.Windows.Forms.RadioButton radOrganization;
        private System.Windows.Forms.RadioButton radShipment;
        private System.Windows.Forms.Label lblKey;
        private System.Windows.Forms.Button btnGetXML;
        private System.Windows.Forms.TextBox txtMessageKey;
        private System.Windows.Forms.GroupBox gbxMarkAsProcessed;
        private System.Windows.Forms.RichTextBox rtxXMLMessageResult;
        private System.Windows.Forms.Button btnMarkAsProcessed;
        private System.Windows.Forms.TextBox txtProcessedMessageID;
        private System.Windows.Forms.Label lblMarkAsProcessedMessageID;
    }
}

