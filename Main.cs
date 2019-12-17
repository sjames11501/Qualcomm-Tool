using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QCOH.SAHARA;
using System.IO;
using System.IO.Ports;
using QCOH.FIREHOSE;
using System.Threading;
using QCOH.Crypto;
using System.Management;


namespace QCOH
{
    public partial class Main : Form
    {
        //.Skip(72).Take(16)

        private uiManager _uiManager = new uiManager();
        private BackgroundWorker bw = new BackgroundWorker();
        private SAHARA_MANAGER _saharaManager;
        private SerialPortCommuncationManager _spcm = new SerialPortCommuncationManager();
        private SerialPort sp = new SerialPort();
        private FirehoseManager _firehoseManager = new FirehoseManager();
        private string port;
        private string portName;
        bool spConnected = false;
        private string partition_to_dump;

        private void getQCPort()
        {

            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
            {
                var portnames = SerialPort.GetPortNames();
                var ports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());

                var portList = portnames.Select(n => n + " - " + ports.FirstOrDefault(s => s.Contains(n))).ToList();
                foreach (var s in portList)
                {
                    if (s.Contains("9008"))
                    {
                        int start = s.IndexOf("C") + 0;
                        int length = s.IndexOf("-") - start;
                        var output = s.Substring(start, length);
                        this.port = output;
                        this.portName = s;

                    }
                }
            }

       
        
                
            

        }
        public Main()
        {
            InitializeComponent();
            cmbModel.DropDownStyle = ComboBoxStyle.DropDownList;
            /*************************************************Config UI**/
            _uiManager.btnConnect = this.btnConnect;
            _uiManager.ckbEraseSt = this.ckbEraseST;
            _uiManager.ckbDwnModem = this.ckbDwnModem;
            _uiManager.selectPartitionForm = new dlgSelectPartition();
            _uiManager.rtb = this.rtbLog;
            _uiManager.prgBar = this.prgBar;
            _uiManager.rtb.ReadOnly = true;
            _uiManager.flashFiledialog = this.openFileDialog1;
            _uiManager.stsStrip = this.statusStrip1;
            _uiManager.cmbModel = this.cmbModel;
            _uiManager.controls = this.Controls;

            this.openFileDialog1.Filter =
             "Qualcomm Flash Programmer (*.hex;*.mbn;)" +
             "All files (*.*)|*.*";
            this.openFileDialog1.FileName = "";
            //disable resize.
            
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            /********************************************************************/
            getQCPort();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;



        }

        #region FIREHOSE FUNCTIONS
        //FUNCTIONS - USING BACKGROUND WORKERS
        void dumpPartition(object sender, DoWorkEventArgs e)
        {
            if (this._firehoseManager != null)
            {

                _firehoseManager.DumpPartition(partition_to_dump);
            }
        }

        void fh_SimLockTCL(object sender, DoWorkEventArgs e)
        {
            if (this._firehoseManager != null)
            {

                _firehoseManager.writeSimLock();
            }
        }


        void fh_restoreSimLock(object sender, DoWorkEventArgs e)
        {
            if (this._firehoseManager != null)
            {

                _firehoseManager.restoreSimLock();
            }
        }

        void fh_pwreset(object sender, DoWorkEventArgs e)
        {
            if (this._firehoseManager != null)
            {

                _firehoseManager.SendReset();
            }
        }


        #endregion
        private void bw_RunWorkerCompleted(
    object sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                // Next, handle the case where the user canceled 
                // the operation.
                // Note that due to a race condition in 
                // the DoWork event handler, the Cancelled
                // flag may not have been set, even though
                // CancelAsync was called.
              
            }
            else
            {
                // Finally, handle the case where the operation 
                // succeeded.
               
            }

      
        }

        private  void btnConnect_Click(object sender, EventArgs e)
        {
  
            if(cmbModel.SelectedItem == null)
            {
                this.rtbLog.Text = "";
                _uiManager.DisplayData(uiManager.MessageType.Error, "Please select a model before continuing.." + "\n", false);
       
            }
         
            if (port == null) 
            {
             
                    getQCPort();
                    this.rtbLog.Text = "";
                _uiManager.DisplayData(uiManager.MessageType.Error, "No device found. Try Again" + "\n", false);

            }


            if (cmbModel.SelectedItem != null && port != null)
            {
                try
                {
                    BackgroundWorker bws = new BackgroundWorker();
                    if (!(this.rtbLog.Text.Length > 50))
                    {
                        this.rtbLog.Text = "";
                    }
                  
                    _uiManager.DisplayData(uiManager.MessageType.Outgoing, "===Session Started.===\n", false);
                    _uiManager.DisplayData(uiManager.MessageType.Outgoing, "Found Device on " + this.port+ "\n", false);
                    _uiManager.DisplayData(uiManager.MessageType.Outgoing, "Device friendly name: " + this.portName + "\n", false);
                    _uiManager.DisplayData(uiManager.MessageType.Outgoing, "Starting Connection.\n", false);
                    _uiManager.lockUI();
                    bws.DoWork += new DoWorkEventHandler(startSahara);
                    bws.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

                    bws.RunWorkerAsync();
                    bws.Dispose();
                }
                catch (Exception ee)
                {

                }

            }
                   
                  
                   
                
              


          
            
        }


  
 

  

        private void btnSelectFlash_Click(object sender, EventArgs e)
        {
       
        }

        private void btnDumpPartition_Click(object sender, EventArgs e)
        {
            var _items = this._uiManager.partitions;
            foreach(var item in _items)
            {
                _uiManager.selectPartitionForm.cmbPartionName.Items.Add(item.partName.ToString());

            }
            if (_uiManager.selectPartitionForm.ShowDialog() == DialogResult.OK)
            {
                var partName = _uiManager.selectPartitionForm.cmbPartionName.SelectedItem.ToString();
                this.partition_to_dump = partName;

                bw.Dispose();
                BackgroundWorker bw2 = new BackgroundWorker();
                bw2.DoWork += new DoWorkEventHandler(dumpPartition);
                bw2.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                bw2.RunWorkerAsync();

            }



        }


        void startSahara(object sender, DoWorkEventArgs e)
        {
            getQCPort();
            if (spConnected == false)
            {
                sp.BaudRate = 921600; //BaudRate
                sp.Handshake = Handshake.None;
                sp.DataBits = 8; //DataBits
                sp.StopBits = System.IO.Ports.StopBits.One;
                sp.Parity = System.IO.Ports.Parity.None;
                sp.DtrEnable = true;
                sp.NewLine = Environment.NewLine;
                sp.PortName = port;
                spConnected = true;
            }
            else
            {
                sp.Close();
                sp.Dispose();
            }
          
            _firehoseManager = new FirehoseManager();
            _spcm.setupSP(sp);
           
            
            _saharaManager = new SAHARA_MANAGER(this._uiManager, _firehoseManager, this._spcm);
        }

        private void btnDump_Click(object sender, EventArgs e)
        {

        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void btnSimLock_Click(object sender, EventArgs e)
        {
            bw.Dispose();
            BackgroundWorker bw2 = new BackgroundWorker();
            bw2.DoWork += new DoWorkEventHandler(fh_SimLockTCL);
            bw2.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw2.RunWorkerAsync();
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            bw.Dispose();
            BackgroundWorker bw2 = new BackgroundWorker();
            bw2.DoWork += new DoWorkEventHandler(fh_restoreSimLock);
            bw2.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw2.RunWorkerAsync();
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            bw.Dispose();
            BackgroundWorker bw2 = new BackgroundWorker();
            bw2.DoWork += new DoWorkEventHandler(fh_pwreset);
            bw2.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw2.RunWorkerAsync();
        }
    }
}
