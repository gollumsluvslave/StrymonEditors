namespace RITS.StrymonEditor.Updater
{
    partial class UpdateProgressTracker
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
            this.prgOverallProgress = new System.Windows.Forms.ProgressBar();
            this.lblFileName = new System.Windows.Forms.Label();
            this.lblFileCounters = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // prgOverallProgress
            // 
            this.prgOverallProgress.Location = new System.Drawing.Point(15, 23);
            this.prgOverallProgress.Name = "prgOverallProgress";
            this.prgOverallProgress.Size = new System.Drawing.Size(246, 19);
            this.prgOverallProgress.TabIndex = 0;
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFileName.Location = new System.Drawing.Point(12, 45);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(0, 13);
            this.lblFileName.TabIndex = 1;
            // 
            // lblFileCounters
            // 
            this.lblFileCounters.AutoSize = true;
            this.lblFileCounters.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFileCounters.Location = new System.Drawing.Point(12, 7);
            this.lblFileCounters.Name = "lblFileCounters";
            this.lblFileCounters.Size = new System.Drawing.Size(0, 13);
            this.lblFileCounters.TabIndex = 2;
            // 
            // UpdateProgressTracker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(276, 69);
            this.Controls.Add(this.lblFileCounters);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.prgOverallProgress);
            this.Name = "UpdateProgressTracker";
            this.Load += new System.EventHandler(this.UpdateProgressTracker_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar prgOverallProgress;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Label lblFileCounters;
    }
}