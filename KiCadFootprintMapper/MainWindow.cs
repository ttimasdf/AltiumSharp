using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using AltiumSharp;

namespace KiCadFootprintMapper
{
    public partial class MainWindow : Form
    {
        public string Log
        {
            get => labelCmt.Text;
            set => labelCmt.Text = value;
        }

        private SymbolMapper symbolMapper = new();
        private StepModelMapper stepModelMapper = new();

        private string folderName = "";
        private string outputFolderName = "";
        private string[] schFileList = new string[0];
        private string[] pcbFileList = new string[0];

        public MainWindow()
        {
            InitializeComponent();
        }

        private bool canSchJobStart()
        {
            return schFileList.Length > 0 && symbolMapper.IsLoaded;
        }

        private bool canPcbJobStart()
        {
            return pcbFileList.Length > 0 && stepModelMapper.IsLoaded;
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                folderName = folderBrowserDialog.SelectedPath;
                outputFolderName = Path.Combine(folderName, "Output.nosync");
                if (!Directory.Exists(outputFolderName)) Directory.CreateDirectory(outputFolderName);
                schFileList = Directory.EnumerateFiles(folderName, "*.SchLib", SearchOption.AllDirectories).ToArray();
                pcbFileList = Directory.EnumerateFiles(folderName, "*.PcbLib", SearchOption.AllDirectories).ToArray();

                Log = $"Found {schFileList.Length} SchLib, {pcbFileList.Length} PcbLib";
                buttonDoSch.Enabled = canSchJobStart();
                buttonDoPcb.Enabled = canPcbJobStart();
            }
        }

        private void ButtonLoadSch_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = "mapping.json";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                symbolMapper = new SymbolMapper(openFileDialog.FileName);
                buttonDoSch.Enabled = canSchJobStart();
            }
        }
        private void ButtonLoadPcb_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = "pcblib.json";
            folderBrowserDialog.Description = "Select 3D model directory";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK && folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                stepModelMapper = new StepModelMapper(openFileDialog.FileName, folderBrowserDialog.SelectedPath);
                buttonDoPcb.Enabled = canPcbJobStart();
            }
        }

        private void ButtonDoSch_Click(object sender, EventArgs e)
        {
            if (!applyMappingsWorker.IsBusy)
            {
                applyMappingsWorker.RunWorkerAsync();
                Log = $"Applying patch.";
                progressBar.Value = 0;
                buttonDoSch.Text = "Cancel";
            }
            else
            {
                applyMappingsWorker.CancelAsync();
            }
        }

        private void ButtonDoPcb_Click(object sender, EventArgs e)
        {
            if (!addModelWorker.IsBusy)
            {
                addModelWorker.RunWorkerAsync();
                Log = $"Applying patch.";
                progressBar.Value = 0;
                buttonDoPcb.Text = "Cancel";
            }
            else
            {
                addModelWorker.CancelAsync();
            }
        }

        private void applyMappingsWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (sender as BackgroundWorker)!;
            foreach (var (fileName, i) in schFileList.Select((fn, i) => (fn, i)))
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                var libName = Path.GetFileNameWithoutExtension(fileName);
                var libFileName = Path.GetFileName(fileName);
                SchLib? lib;

                using (var reader = new SchLibReader())
                {
                    lib = reader.Read(fileName);
                    symbolMapper.ApplyMapping(lib, libName);
                }
                using (var writer = new SchLibWriter())
                {
                    writer.Write(lib, Path.Combine(outputFolderName, libFileName), true);
                }
                worker.ReportProgress(100 * i / schFileList.Length);
            }
            e.Result = $"Processed {schFileList.Length} schlibs";
            worker.ReportProgress(100);

        }
        private void addModelWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (sender as BackgroundWorker)!;
            foreach (var (fileName, i) in pcbFileList.Select((fn, i) => (fn, i)))
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                var libName = Path.GetFileNameWithoutExtension(fileName);
                var libFileName = Path.GetFileName(fileName);
                PcbLib? lib;

                using (var reader = new PcbLibReader())
                {
                    lib = reader.Read(fileName);
                    stepModelMapper.ApplyMapping(lib);
                }
                using (var writer = new PcbLibWriter())
                {
                    writer.Write(lib, Path.Combine(outputFolderName, libFileName), true);
                }
                worker.ReportProgress(100 * i / pcbFileList.Length);
            }
            e.Result = $"{pcbFileList.Length} pcblibs, {stepModelMapper.CountHasModel} models, {stepModelMapper.CountNoneModel} missing";
            worker.ReportProgress(100);
        }

        private void xxWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void xxWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
                Log = "Canceled!";
            else if (e.Error != null)
                Log = "Error: " + e.Error.Message;
            else if (e.Result is string @logstr)
                Log = $"Done! {logstr}";
            else
                Log = "Done!";
            buttonDoSch.Text = "mod Sch";
            buttonDoSch.Enabled = canSchJobStart();
            buttonDoPcb.Text = "mod Pcb";
            buttonDoPcb.Enabled = canPcbJobStart();
        }

    }
}
