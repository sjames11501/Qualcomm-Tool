using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Data;
using QCOH.SAHARA;
using System.IO;
using System.IO.Ports;
using QCOH.FIREHOSE;
using System.Threading;

namespace QCOH
{
    public class uiManager
    {
        public  uiManager(){
            this.partitions = new List<gpt_partition_entry>();


        }
        public enum MessageType { Incoming, Outgoing, Normal, Warning, Error };
        private Color[] MessageColor = { Color.Black, Color.Black, Color.Black, Color.Orange, Color.Red };
        [STAThread]
        public void DisplayData(MessageType type, string msg, bool bold)
        {
            var _displayWindow = this.rtb;
            _displayWindow.Invoke(new EventHandler(delegate
            {
                // _displayWindow.SelectedText = string.Empty;
                _displayWindow.SelectionStart = _displayWindow.TextLength;
                _displayWindow.SelectionLength = 0;
                _displayWindow.SelectionColor = MessageColor[(int)type];
                if (bold)
                {
                    _displayWindow.SelectionFont = new Font(_displayWindow.SelectionFont, FontStyle.Regular);
                }
                else
                {
                    _displayWindow.SelectionFont = new Font(_displayWindow.SelectionFont, FontStyle.Regular);
                }

                _displayWindow.AppendText(msg);
                _displayWindow.ScrollToCaret();
            }));
        }
        public Control.ControlCollection controls;
        public IList<gpt_partition_entry> partitions { get; set; }
        private IList<Button> firehoseButtons
        {
            get
            {
                var x = new List<Button>();
                foreach (Control gb in this.controls)
                {
                    if (gb is GroupBox)
                    {
                        if (gb.Name == "fhGroupBox")
                        {
                            foreach (Button bt in gb.Controls)
                            {

                                x.Add(bt);

                            }
                        }

                    }

                }
                return x;
            }
        }
        public ProgressBar prgBar { get; set; }
        public CheckBox ckbEraseSt { get; set; }
        public CheckBox ckbDwnModem{ get; set; }
        public Button btnConnect { get; set; }
        public ComboBox cmbModel { get; set; }
        public dlgSelectPartition selectPartitionForm { get; set; }
        public StatusStrip stsStrip { get; set; }
        public RichTextBox rtb { get; set; }
        public OpenFileDialog flashFiledialog { get; set; }


        public void addPartitionEntry(gpt_partition_entry x)
        {
            this.partitions.Add(x);

        
           

        }
        public string getSelectedModel()
        {
            var x = "";
            this.cmbModel.Invoke((Action)(() => x = this.cmbModel.SelectedItem.ToString()));
            return x;
        }   
        public void setProgressbar(int value,int finalVal)
        {

            int x = value / finalVal;
            this.prgBar.Invoke((Action)(() => prgBar.Maximum = finalVal));
            this.prgBar.Invoke((Action)(() => prgBar.Value = x));
        }
        public void updateProgressBar()
        {
            if (this.prgBar.Value == 100)
            {
                this.prgBar.Invoke((Action)(() => prgBar.Value = 0));
            }
            else
            {
                this.prgBar.Invoke((Action)(() => prgBar.Value++));
            }
          
        }
        public void clearProgress()
        {

            this.prgBar.Invoke((Action)(() => prgBar.Maximum = 100));
            this.prgBar.Invoke((Action)(() => prgBar.Value = 0));
        }
        public void unlock()
        {

            this.btnConnect.Invoke((Action)(() =>
                  this.btnConnect.Enabled = true

                   ));

            this.cmbModel.Invoke((Action)(() =>
                  this.cmbModel.Enabled = true

                   ));


      
        }

        public void saveLog(string path)
        {
            this.rtb.Invoke((Action)(() => rtb.SaveFile(path, RichTextBoxStreamType.PlainText)));


           
        }
        public void toggleFHButtons()
        {
            var buttonsOn = false;

            if (this.firehoseButtons.First().Enabled == true)
            {
                foreach (var item in this.firehoseButtons)
                {
                    item.Invoke((Action)(() =>
                    item.Enabled = false
    
                    ));
                }
           
            }
            else
            {
                foreach (var item in this.firehoseButtons)
                {
                    item.Invoke((Action)(() => item.Enabled = true));
                }
            }

       

        }
        public void lockUI()
        {

            this.btnConnect.Enabled = false;
           
            this.ckbEraseSt.Enabled = false;
            this.ckbDwnModem.Enabled = false;
            this.cmbModel.Enabled = false;


        }

    }
}
