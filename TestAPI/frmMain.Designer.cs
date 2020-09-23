namespace TestAPI
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
            this.btnGetXML = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnGetXML
            // 
            this.btnGetXML.Location = new System.Drawing.Point(277, 72);
            this.btnGetXML.Name = "btnGetXML";
            this.btnGetXML.Size = new System.Drawing.Size(140, 23);
            this.btnGetXML.TabIndex = 0;
            this.btnGetXML.Text = "Get XML";
            this.btnGetXML.UseVisualStyleBackColor = true;
            this.btnGetXML.Click += new System.EventHandler(this.btnGetXML_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 168);
            this.Controls.Add(this.btnGetXML);
            this.Name = "frmMain";
            this.Text = "API Testing";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGetXML;
    }
}

