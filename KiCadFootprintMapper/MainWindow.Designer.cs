namespace KiCadFootprintMapper
{
    partial class MainWindow
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
            this.buttonDoSch = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.labelCmt = new System.Windows.Forms.Label();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.buttonLoadSch = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.applyMappingsWorker = new System.ComponentModel.BackgroundWorker();
            this.buttonLoadPcb = new System.Windows.Forms.Button();
            this.buttonDoPcb = new System.Windows.Forms.Button();
            this.addModelWorker = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // buttonDoSch
            // 
            this.buttonDoSch.Enabled = false;
            this.buttonDoSch.Location = new System.Drawing.Point(93, 58);
            this.buttonDoSch.Name = "buttonDoSch";
            this.buttonDoSch.Size = new System.Drawing.Size(75, 23);
            this.buttonDoSch.TabIndex = 0;
            this.buttonDoSch.Text = "mod Sch";
            this.buttonDoSch.UseVisualStyleBackColor = true;
            this.buttonDoSch.Click += new System.EventHandler(this.ButtonDoSch_Click);
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Open Symbol Folder";
            // 
            // labelCmt
            // 
            this.labelCmt.AutoSize = true;
            this.labelCmt.Location = new System.Drawing.Point(12, 38);
            this.labelCmt.Name = "labelCmt";
            this.labelCmt.Size = new System.Drawing.Size(122, 17);
            this.labelCmt.TabIndex = 1;
            this.labelCmt.Text = "Select a folder first!";
            // 
            // buttonOpen
            // 
            this.buttonOpen.Location = new System.Drawing.Point(12, 12);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(75, 23);
            this.buttonOpen.TabIndex = 2;
            this.buttonOpen.Text = "Open";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // buttonLoadSch
            // 
            this.buttonLoadSch.Location = new System.Drawing.Point(12, 58);
            this.buttonLoadSch.Name = "buttonLoadSch";
            this.buttonLoadSch.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadSch.TabIndex = 3;
            this.buttonLoadSch.Text = "Load";
            this.buttonLoadSch.UseVisualStyleBackColor = true;
            this.buttonLoadSch.Click += new System.EventHandler(this.ButtonLoadSch_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 134);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(316, 23);
            this.progressBar.Step = 1;
            this.progressBar.TabIndex = 4;
            // 
            // applyMappingsWorker
            // 
            this.applyMappingsWorker.WorkerReportsProgress = true;
            this.applyMappingsWorker.WorkerSupportsCancellation = true;
            this.applyMappingsWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.applyMappingsWorker_DoWork);
            this.applyMappingsWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.xxWorker_ProgressChanged);
            this.applyMappingsWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.xxWorker_RunWorkerCompleted);
            // 
            // buttonLoadPcb
            // 
            this.buttonLoadPcb.Location = new System.Drawing.Point(12, 87);
            this.buttonLoadPcb.Name = "buttonLoadPcb";
            this.buttonLoadPcb.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadPcb.TabIndex = 5;
            this.buttonLoadPcb.Text = "Load";
            this.buttonLoadPcb.UseVisualStyleBackColor = true;
            this.buttonLoadPcb.Click += new System.EventHandler(this.ButtonLoadPcb_Click);
            // 
            // buttonDoPcb
            // 
            this.buttonDoPcb.Enabled = false;
            this.buttonDoPcb.Location = new System.Drawing.Point(93, 87);
            this.buttonDoPcb.Name = "buttonDoPcb";
            this.buttonDoPcb.Size = new System.Drawing.Size(75, 23);
            this.buttonDoPcb.TabIndex = 6;
            this.buttonDoPcb.Text = "mod Pcb";
            this.buttonDoPcb.UseVisualStyleBackColor = true;
            this.buttonDoPcb.Click += new System.EventHandler(this.ButtonDoPcb_Click);
            // 
            // addModelWorker
            // 
            this.addModelWorker.WorkerReportsProgress = true;
            this.addModelWorker.WorkerSupportsCancellation = true;
            this.addModelWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.addModelWorker_DoWork);
            this.addModelWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.xxWorker_ProgressChanged);
            this.addModelWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.xxWorker_RunWorkerCompleted);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 169);
            this.Controls.Add(this.buttonDoPcb);
            this.Controls.Add(this.buttonLoadPcb);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonLoadSch);
            this.Controls.Add(this.buttonOpen);
            this.Controls.Add(this.labelCmt);
            this.Controls.Add(this.buttonDoSch);
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button buttonDoSch;
        private FolderBrowserDialog folderBrowserDialog;
        private Label labelCmt;
        private Button buttonOpen;
        private Button buttonLoadSch;
        private OpenFileDialog openFileDialog;
        private ProgressBar progressBar;
        private System.ComponentModel.BackgroundWorker applyMappingsWorker;
        private Button buttonLoadPcb;
        private Button buttonDoPcb;
        private System.ComponentModel.BackgroundWorker addModelWorker;
    }
}