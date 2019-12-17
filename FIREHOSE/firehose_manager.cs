using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QCOH.FIREHOSE;
using System.Diagnostics;
using System.Threading;
using System.Xml;
using System.IO;
using System.IO.Ports;
using QCOH.Crypto;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;


namespace QCOH.FIREHOSE
{
    public class FirehoseManager
    {
        #region VARIABLES --------------------------------------------------
        private string model;
        private bool efsErase = false;
        private FIREHOSE_DEV_INFO device_info;
        private FIREHOSE_GPT gpt;
        private int packetSize = 8192;
        private FIREHOSE_CONFIG firehose_config;
        private FIREHOSE_RESPONSE_PACKET XmlResponse { get; set; }
        public uiManager _uiManager { get; set; }
        private SerialPortCommuncationManager _spcm;
        public bool Status { get; set; }
        private string PartionToDump { get; set; }
        #endregion ----------------------------------------------------------


       
        private void dumpDeviceMemory()
        {

        
            uint startAddr = 0x008000 + 7424;

            var maxDump = 7424;
            var loops = 50;
            int i = 0;
            var offset = startAddr;


            while (loops > 0)
            {


                var bytes = this.peekMem(offset, maxDump);


                if (i == 0)
                {
                    File.WriteAllBytes("/pbl-dump.bin", bytes);

                }
                else
                {
                    util.AppendAllBytes("/pbl-dump.bin", bytes);

                }

                offset = offset + 7424;
                loops = loops - 1;
                i++;
                Thread.Sleep(10);
            }


        }

        private void setModel()
        {
            #region SET MODEL
            if (this.device_info.prov_id.Contains("4060A"))
            {
                model = "4060A";
            }

            if (this.device_info.prov_id.Contains("5098O"))
            {
                model = "4060A";
            }

            if (this.device_info.prov_id.Contains("4044L"))
            {
                model = "4044L";
            }
            if (this.device_info.prov_id.Contains("4044C"))
            {
                model = "4044C";
            }

            if (this.device_info.prov_id.Contains("4044W"))
            {
                model = "4044W";
            }
            if (this.device_info.prov_id.Contains("A576CC"))
            {
                model = "A576CC";
            }
            if (this.device_info.prov_id.Contains("5046S"))
            {
                model = "A576CC";
            }
            if (this.device_info.prov_id.Contains("4044T"))
            {
                model = "4044T";
            }


            if (this.device_info.prov_id.Contains("5044R"))
            {
                model = "5044R";
            }
            if (this.device_info.prov_id.Contains("4044L"))
            {
                model = "4044L";
            }

            if (this.device_info.prov_id.Contains("5054O"))
            {
                model = "5054O";
            }
            #endregion
        }
        public void ConnectToFlashProg()
        {
            if (Status != false) return;

            ////fh, previous command @ 0x0805ad78
            ////logbug @ 0x08054f10

            this.sendConfig();
            this.geteMMCinfo();
            this.read_hw_imei();
            this.ParseGPT();
            this.ParsePartions(this.gpt.header.starting_lba_pe);
            this.ReadDeviceInfo();
            this.setModel();
           
            if (this._uiManager.ckbDwnModem.Checked)
            {
           
            
            }

            if (this._uiManager.ckbEraseSt.Checked)
            {

                if (this.erasePartition("modemst1"))
                {
                    _uiManager.DisplayData(uiManager.MessageType.Outgoing, "Erased MODEMST1 partition.\n", true);
                }

                if (this.erasePartition("modemst2"))
                {
                    _uiManager.DisplayData(uiManager.MessageType.Outgoing, "Erased MODEMST2 partition.\n", true);
                }

                efsErase = true;
            }



        }


    
        #region SIMLOCK functions
    

        public void restoreSimLock()
        {
            /****************************************************************************/
            diskUtil dw = new diskUtil(_spcm);
            var smlP = this.getPartition("simlock");

            var path = "/BACKUPS/" + model + "-" + device_info.imei + ".simlock";
            if (!File.Exists(path))
            {
                this._uiManager.DisplayData(uiManager.MessageType.Outgoing, "No simlock backup found for this imei..\n", true);
                return;
            }


            this._uiManager.DisplayData(uiManager.MessageType.Outgoing, "Found backup, restoring....", true);
            var bytes = File.ReadAllBytes(path);
            dw.programPartition("simlock", smlP.first_lba, bytes);
            this._uiManager.DisplayData(uiManager.MessageType.Outgoing, "OK\n", true);
            /********************************/

        }
        public void writeSimLock()
        {
            /****Backup SIMLOCK partition ****/
            diskUtil dw = new diskUtil(_spcm);
            var smlP = this.getPartition("simlock");
            var path = "/BACKUPS/" + model + "-" + device_info.imei + ".simlock";
            dw.dumpPartition(smlP, path);
            /********************************/
            byte[] slBytes = null;

            #region SET SIMLOCK BYTES
            if (model == "4060A")
            {
                slBytes = null; // provide firehose binary
            }


            if (slBytes == null)
                return;
            #endregion




            this._uiManager.DisplayData(uiManager.MessageType.Outgoing, "Writing SIMLOCK data using T method...", true);

            try
            {
                dw.writeSimLockTCL(slBytes, smlP.first_lba);
                this._uiManager.DisplayData(uiManager.MessageType.Outgoing, "OK\n", true);


                if (efsErase == false)
                {
                    if (this.erasePartition("modemst1"))
                    {
                        _uiManager.DisplayData(uiManager.MessageType.Outgoing, "Erased MODEMST1 partition.\n", true);
                    }

                    if (this.erasePartition("modemst2"))
                    {
                        _uiManager.DisplayData(uiManager.MessageType.Outgoing, "Erased MODEMST2 partition.\n", true);
                    }


   

                    _uiManager.DisplayData(uiManager.MessageType.Outgoing, "SIM UNLOCK complete\n", true);
                }
            }
            catch (Exception e)
            {
                this._uiManager.DisplayData(uiManager.MessageType.Outgoing, "\nERROR writing SIMLOCK.", true);
            }


         //   if (this.erasePartition("config"))
         //   {
         //       _uiManager.DisplayData(uiManager.MessageType.Outgoing, "FRP partition erased.\n", true);
         //   }

            this.SendReset();


        }
    

        #endregion



        public FirehoseManager(SerialPortCommuncationManager spcm)
        {
            this._spcm = spcm;
        
        }

        public FirehoseManager()
        {

        }

        public void setSPCM(SerialPortCommuncationManager x)
        {
            this._spcm = x;
        }
        public void setUIManager(uiManager x)
        {
            this._uiManager = x;
        }
        private void sendConfig()
        {
            var configString = firehose_pkts.pkt_fhConfig();
            diskUtil du = new diskUtil(_spcm);
            du.WriteXml(configString);
            GetConfigResponseXml();
            this.setAckRaw(0);
        }
     

        private void setAckRaw(int val)
        {
            var pkt = firehose_pkts.pkt_setAckRaw(val);
            diskUtil du = new diskUtil(_spcm);
            du.WriteXml(pkt);
        }
        private bool programPartition(string partName,byte[] bytes)
        {
            var p = this.getPartition(partName);
            var offset = 0;
            var completed = false;
            var totalBytes = bytes.Length;
            var bytesWritten = 0;
            var packetSize = 16384;
            var loops = totalBytes / packetSize;
            var n512sectors = totalBytes / 512;
            int i = 0;
            var pkt = firehose_pkts.pkt_Program(n512sectors, partName, p.first_lba);
            diskUtil du = new diskUtil(_spcm);
            du.WriteXml(pkt);//write intital write request packet

            _uiManager.clearProgress();
          
            /*****************************************/

            if (!isACK())
                //error//
                return true;
            else
            {
                while (true)
                {

                 
                    if (bytesWritten == totalBytes)
                    {

                        if (isACK()) // got ACK, continue writing
                        {
                          
                            completed = true;
                            return true; 
                        }
                       //error
                       
                    }
                    else
                    {
                        byte[] cBytes = bytes.Skip(offset).Take(packetSize).ToArray();
                        writeRaw(cBytes);
                        bytesWritten += packetSize;
                        offset += packetSize;
                        _uiManager.setProgressbar(offset, totalBytes);
                        i++;
                     
                    }
                }

            }



        }
        private bool erasePartition(string partName)
        {
            var p = this.getPartition(partName);
            var totalBytes = p.num512Sectors * 512;
            var bytesWritten = 0;
            var completed = false;
            var loops = totalBytes / 8192;
            var pkt = firehose_pkts.pkt_Program(p.num512Sectors, partName, p.first_lba);
            int i = 0;
            diskUtil du = new diskUtil(_spcm);
            du.WriteXml(pkt); //write intital program packet

            if (!isACK())
            {
                return false;
            }

            while (true)
            {
                _uiManager.updateProgressBar();
                if (bytesWritten == totalBytes)
                {
                    if (isACK())
                    {
                        completed = true;
                        break;

                    }

                    //error//
                  
                }
                else
                {
                    byte[] emptyBytes = new byte[32768];
                    writeRaw(emptyBytes);
                    bytesWritten += 32768;
                    i++;
                    continue;
                  
              
              
             
                }
            }

            _uiManager.clearProgress();
            return completed;
            
        }
        #region WRITE FUNCTIONS
 
        private void writeRaw(byte[] bytes)
        {
            byte[] outputBuffer = bytes;
            this._spcm.Send(outputBuffer);
        }
        #endregion

        
        private byte[] peekMem(uint address, int size)
        {
            string s_addr = address.ToString();
            string s_size = size.ToString();
            this._uiManager.DisplayData(uiManager.MessageType.Outgoing, "Attempting to read " + s_size +" bytes at address: " + s_addr + "..", true);

            try
            {
                var p = firehose_pkts.pkt_peekMem(address, size);
                diskUtil du = new diskUtil(_spcm);
                du.WriteXml(p);
                var r = _spcm.Receive(100);
                var ra = Encoding.ASCII.GetString(r);
                var x = xmlEngine.convertFHpeek(ra);

                this._uiManager.DisplayData(uiManager.MessageType.Outgoing, ".OK\n\n\n", true);
                return x;


            } catch(Exception e)
            {
                return null;
            }



         

        }

        private void read_hw_imei()
        {
            diskUtil du = new diskUtil(_spcm);
            du.WriteXml(firehose_pkts.pkt_readIMEI());

            var comBuffer = _spcm.Receive(100);
            var imeiBytes = comBuffer.Take(16).ToArray();
            device_info.hwIMEI = imeiBytes;
            //this._uiManager.DisplayData(uiManager.MessageType.Outgoing, "HW IMEI STORED." + "\n", true);
        }
        private void ReadSerialNumber()
        {
            var x = firehose_pkts.pkt_readSerialNumber();
            diskUtil du = new diskUtil(_spcm);
            du.WriteXml(x);
        }
        public void SendReset()
        {
            this._uiManager.toggleFHButtons();
            this._uiManager.unlock();
            diskUtil du = new diskUtil(_spcm);
            du.WriteXml(firehose_pkts.pkt_sendReset());
            _uiManager.saveLog("/QCOH_LOGS/" + model + "-" + device_info.imei + ".txt");
            this._uiManager.DisplayData(uiManager.MessageType.Outgoing, "===Session complete. Log saved===" + "\n", true);
            this._spcm.close();
        }
        private void geteMMCinfo()
        {
            FIREHOSE_RESPONSE_PACKET fhXml = new FIREHOSE_RESPONSE_PACKET();
            var p = firehose_pkts.pkt_eMMCinfo();
            diskUtil du = new diskUtil(_spcm);
            du.WriteXml(p);
            var r = this._spcm.Receive(100);
            var result = System.Text.Encoding.ASCII.GetString(r);
            string xml = "<firehose-response>" + result + "</firehose-response>";

            if (result.Contains("eMMC"))
            {

                //remove the xml declarations
                xml = xml.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>", "");
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);

                string xpath = "firehose-response";
                var nodes = xmlDoc.SelectSingleNode(xpath).ChildNodes;

                foreach (XmlNode dataNode in nodes)
                {
                    //each childNode is a <data> respone from FH

                    if (dataNode.FirstChild.Name == "log")
                    {
                        var value = dataNode.FirstChild.Attributes["value"].InnerText.ToString();

                        if (value.Contains("size"))
                        {
                            //extract emmc size//
                            value = value.Trim('"');
                            value = value.Replace("eMMC size=", "");
                            device_info.emmcSize = Convert.ToInt32(value);
                        }


                    }
                }



            }

        }

        private bool partitionValid(string x)
        {
            var valid = false;
            var _p = this.gpt.entries.Where(xx => xx.partName == x).Any();
            if (_p) { valid = true; }
            return valid;
        }
        private gpt_partition_entry getPartition(string x)
        {
            var xx = gpt.entries.First(xxx => xxx.partName == x);
            return xx;
        }


       
        //Reads device info from trace partition//
        private void ReadDeviceInfo()
        {

            var pex = partitionValid("traceability");
            if (pex)
            {
                var x = getPartition("traceability");
                SendRead(512, 1, 0, x.first_lba);
                var response = _spcm.Receive(100);
                var ssn = Encoding.ASCII.GetString(response.Skip(79).Take(15).ToArray(), 0, 15);
                var provId = Encoding.ASCII.GetString(response.Skip(233).Take(15).ToArray(), 0, 15);
                if (provId.Contains("\0"))
                {
                    provId = provId.Substring(0, provId.IndexOf('\0'));
                }
         

                this.device_info.prov_id = provId;
                var imei = Encoding.ASCII.GetString(response.Skip(36).Take(15).ToArray(), 0, 15);
                device_info.imei = imei;
                ///////////////////////////////////////////////////////////////////////////////////////
                _uiManager.DisplayData(uiManager.MessageType.Outgoing, "IMEI: " + imei + "\n", true);
                _uiManager.DisplayData(uiManager.MessageType.Outgoing, "Provider ID: " + provId + " \n", true);
                _uiManager.DisplayData(uiManager.MessageType.Outgoing, "SSN: " + ssn + "\n", true);
            }
            else
            {
                this._uiManager.DisplayData(uiManager.MessageType.Error, "traceability partition not found..." + "\n", true);

            }
        }
        private void SendRead(int sectorSize, int numPartitionSectors, int physicalPartNum, int startSector)
        {
            diskUtil du = new diskUtil(_spcm);
            du.WriteXml(firehose_pkts.pkt_read(sectorSize,numPartitionSectors,physicalPartNum,startSector));
        }

        public void Getstorageinfo()
        {
            diskUtil du = new diskUtil(_spcm);
            const string resetPkt = "<?xml version=\"1.0\" ?><data><getstorageinfo value=\"imei\"/></data>";
            du.WriteXml(resetPkt);
            Thread.Sleep(100);
            var x = this._spcm.Receive(50);
            string xml = "<firehose-response>" + x + "</firehose-response>";

            //remove the xml declarations
            xml = xml.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>", "");

        }

        public void largePartitionDump(gpt_partition_entry x)
        {
            var lstBytes = new List<byte[]>();
            /***CONSTANTS***/
            const int SECTOR_SIZE = 512;
            int MAX_PACKET_SIZE = 1024 * 1024;

            /***COFIGURE THE CURRENT DUMP SETTINGS***/
            var SECTORS_TO_READ = (x.last_lba - x.first_lba) + 1;
            long BYTES_TO_READ = SECTORS_TO_READ * 512;
            var STRIDES_NEEDED = BYTES_TO_READ / 8192;
            int eMMC_Offset = x.first_lba;
            long bytesRead = 0;

            /***OFFSETS***/
            int fileOffset = 0;
            int emmcOffset = 0;
            int i = 0;



       

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            _uiManager.toggleFHButtons();
            var path = "/" + model + "_" + device_info.imei + "_" + x.partName + ".bin";
            fileUtil.clearFile(path);
            SendRead(512, SECTORS_TO_READ, 0, eMMC_Offset);

            //loop until zero bytes left to read//
            while (true)
            {
                var buffer = this._spcm.Receive(5);

                if (isFinished(buffer))
                {
                    var bytesLeft = BYTES_TO_READ - fileOffset;
                    var buffer2 = buffer.Take((int)bytesLeft).ToArray();
                    fileUtil.appendToFile(path, buffer2);
                    break;
                }
                

                fileUtil.appendToFile(path, buffer);
                bytesRead += buffer.Length;
                i++;
                fileOffset += buffer.Length;
            }
            var timeElapsed = stopwatch.Elapsed.Duration().ToString();
            this._uiManager.DisplayData(uiManager.MessageType.Outgoing, "[Read Ok] " + x.partName + "\nTime Elapsed: " +timeElapsed + "\n", true);
            _uiManager.toggleFHButtons();
        }
 

        //Needs to be cleaned up//
        public void DumpPartition(string partName)
        {
        
                var longDump = false;
                int sectorsRemaining = 0;
                int i = 0;
                byte[] completeDump = new byte[0];
                var mahByteArray = new List<byte[]>();
                int currentOffset = 0;
                int limit = 1000;
                var pex = partitionValid(partName);
             
                this._spcm.flushBuffers();
                Thread.Sleep(200);
                 this._spcm.flushBuffers();
  
            if (!pex) { return; }

       
                var x = getPartition(partName);
                var numSectorsToRead = (x.last_lba - x.first_lba) + 1; //Should be 3072 for MODEMST1
                sectorsRemaining = numSectorsToRead;
                numSectorsToRead = x.num512Sectors;

                long bytestoRead = ((long)numSectorsToRead * 512);
                //Long read, only read 2 sectors at a time;
                if (partName == "simlock")
                {
                _uiManager.toggleFHButtons();
                //SIMLOCK - 512,8,0,365634
                SendRead(512, 8, 0, x.first_lba);
                    var currentBytes = new byte[512 * numSectorsToRead];
                    currentBytes = this._spcm.ReciveWithLimit(20, 512 * 8);
                    bytestoRead = 4096;
                    mahByteArray.Add(currentBytes);
                    byte[] array = mahByteArray.SelectMany(a => a).ToArray();
                var simlockPath = util.getFilePath(device_info.imei + "_simlock", ".bin");
                var path = simlockPath;
           
                    File.WriteAllBytes(path, array);
                    this._uiManager.DisplayData(uiManager.MessageType.Outgoing, "Finished partition dump." + "\n", true);
                _uiManager.toggleFHButtons();
                return;
                    
                }


            if (numSectorsToRead >= 1024) {largePartitionDump(x);} else
            {
                _uiManager.toggleFHButtons();
                SendRead(512, numSectorsToRead, 0, x.first_lba);
                var currentBytes = new byte[512 * numSectorsToRead];
                currentBytes = this._spcm.ReciveWithLimit(20, 512 * numSectorsToRead);
                mahByteArray.Add(currentBytes);
                byte[] array = mahByteArray.SelectMany(a => a).ToArray();
                File.WriteAllBytes("E:/" + device_info.prov_id + "_" + device_info.imei + "_" + partName + ".qcoh.img", array);
                this._uiManager.DisplayData(uiManager.MessageType.Outgoing, "Finished partition dump." + "\n", true);
                _uiManager.toggleFHButtons();
           

            }
             
            
                    

             
 }
        public void ParsePartions(int startingLbaPe)
        {

            this.SendRead(512, 40, 0, startingLbaPe);
            var comBuffer = this._spcm.Receive(100);
            var i = 0;
            this.gpt.entries = new List<gpt_partition_entry>();
            while (true)
            {
                try
                {
                    gpt_partition_entry gptEntry = new gpt_partition_entry();
                  
                    if(i > 41)
                    {
                        break;
                    }
                 

                    if (i == 0)
                    {
                        gptEntry.partTypeGUID = System.Text.Encoding.UTF8.GetString(comBuffer.Skip(0).Take(16).ToArray(), 0, 16);
                        gptEntry.partID = System.Text.Encoding.UTF8.GetString(comBuffer.Skip(16).Take(16).ToArray(), 0, 16);
                        gptEntry.first_lba = BitConverter.ToInt32(comBuffer.Skip(32).Take(8).ToArray(), 0);
                        gptEntry.last_lba = BitConverter.ToInt32(comBuffer.Skip(40).Take(8).ToArray(), 0);
                        gptEntry.flags = comBuffer.Skip(48).Take(8).ToArray();
                        gptEntry.partName = System.Text.Encoding.UTF8.GetString(comBuffer.Skip(56).Take(72).ToArray(), 0, 72).Trim('\0');
                        gptEntry.partName = gptEntry.partName.Replace("\0", "");
                        //this._uiManager.DisplayData(uiManager.MessageType.Outgoing, "[+]Found Partition: " + gptEntry.partName + "\n", true);
                    }
                    else
                    {

                        int startOffset = i * 128;
                        gptEntry.partTypeGUID = System.Text.Encoding.UTF8.GetString(comBuffer.Skip(startOffset).Take(16).ToArray(), 0, 16);
                        gptEntry.partID = System.Text.Encoding.UTF8.GetString(comBuffer.Skip(startOffset + 16).Take(16).ToArray(), 0, 16);
                        gptEntry.first_lba = BitConverter.ToInt32(comBuffer.Skip(startOffset + 32).Take(8).ToArray(), 0);
                        gptEntry.last_lba = BitConverter.ToInt32(comBuffer.Skip(startOffset + 40).Take(8).ToArray(), 0);
                        gptEntry.flags = comBuffer.Skip(startOffset + 48).Take(8).ToArray();
                        gptEntry.partName = Encoding.ASCII.GetString(comBuffer.Skip(startOffset + 56).Take(72).ToArray()).Trim('\0');
                        gptEntry.partName = gptEntry.partName.Replace("\0", "");
                      
                        // System.Text.Encoding.UTFC.GetString(comBuffer.Skip(startOffset+56).Take(72).ToArray(), 0, 72);
                    }

                    if (gptEntry.partName == "")
                    {
                        break;
                    }
                        if (gptEntry.partName != "")
                    {
                       // this._uiManager.DisplayData(uiManager.MessageType.Outgoing, "[+]Found Partition: " + gptEntry.partName + "\n", true);
                        this._uiManager.addPartitionEntry(gptEntry);
                        gpt.entries.Add(gptEntry);
                        i++;
                        continue;
                    }
      
                    
                } catch(Exception e)
                {
                    break;
                }
                break;
            }
        }
        public void ParseGPT()
        {
            this.SendRead(512, 1, 0,1);
            var comBuffer = this._spcm.Receive(100);


            //Read GPT header
            if (comBuffer.Length > 200)
            {
                string s = System.Text.Encoding.UTF8.GetString(comBuffer, 0, comBuffer.Length);
                if (!s.Contains("EFI")) return;
                //parse GPT HEADER
                this.gpt.header = new gpt_header
                {
                    signature = System.Text.Encoding.UTF8.GetString(comBuffer.Skip(0).Take(8).ToArray(), 0, 8),
                    revision = BitConverter.ToInt32(comBuffer.Skip(8).Take(4).ToArray(), 0),
                    header_size = BitConverter.ToInt32(comBuffer.Skip(12).Take(4).ToArray(), 0),
                    crc_header = BitConverter.ToInt32(comBuffer.Skip(16).Take(4).ToArray(), 0),
                    reserved = BitConverter.ToInt32(comBuffer.Skip(20).Take(4).ToArray(), 0),
                    current_lba = BitConverter.ToInt32(comBuffer.Skip(24).Take(8).ToArray(), 0),
                    backup_lba = BitConverter.ToInt32(comBuffer.Skip(32).Take(8).ToArray(), 0),
                    first_usable_lba = BitConverter.ToInt32(comBuffer.Skip(40).Take(8).ToArray(), 0),
                    last_usable_lba = BitConverter.ToInt32(comBuffer.Skip(48).Take(8).ToArray(), 0),
                    disk_guid = comBuffer.Skip(56).Take(16).ToArray(),
                    starting_lba_pe = BitConverter.ToInt32(comBuffer.Skip(72).Take(8).ToArray(), 0)
                };

                byte[] numParts = comBuffer.Skip(80).Take(4).ToArray();
                gpt.header.number_partitions = BitConverter.ToInt32(comBuffer.Skip(80).Take(4).ToArray(), 0);
                gpt.header.size_partition_entries = BitConverter.ToInt32(comBuffer.Skip(84).Take(4).ToArray(), 0);
                decimal hardDriveSizeBytes = (decimal)gpt.header.last_usable_lba * (decimal)512;
                decimal hardDriveSizeGb = hardDriveSizeBytes / (1073741824);
                hardDriveSizeGb = Math.Round(hardDriveSizeGb, 2);
                _uiManager.DisplayData(uiManager.MessageType.Outgoing, "Found eMMC " + hardDriveSizeGb + "GB" + "\n", false);

                //read gpt partition entries,
               // this.ParsePartions(gpt.header.starting_lba_pe);       

            }
        }
        private bool isFinished(byte[] bytes)
        {

            FIREHOSE_RESPONSE_PACKET fhXml = new FIREHOSE_RESPONSE_PACKET();
          
            var x = System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);

            if (x.Contains("log value") && x.Contains("Finished"))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        private bool isACK()
        {
            FIREHOSE_RESPONSE_PACKET fhXml = new FIREHOSE_RESPONSE_PACKET();
            byte[] comBuffer = _spcm.Receive(300);
            var x = System.Text.Encoding.UTF8.GetString(comBuffer, 0, comBuffer.Length);

            if (x.Contains("\"ACK\"")){ 
                return true;
            }
            else
            {
                return false;
            }
        }
        private void GetConfigResponseXml()
        {

            FIREHOSE_RESPONSE_PACKET fhXml = new FIREHOSE_RESPONSE_PACKET();

            byte[] comBuffer = _spcm.Receive(100);
            var x = System.Text.Encoding.UTF8.GetString(comBuffer, 0, comBuffer.Length);
            string xml = "<firehose-response>" + x + "</firehose-response>";
      
            //remove the xml declarations
            xml = xml.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>", "");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            string xpath = "firehose-response";
            var nodes = xmlDoc.SelectSingleNode(xpath).ChildNodes;

            foreach (XmlNode dataNode in nodes)
            {
             //each childNode is a <data> respone from FH

               if(dataNode.FirstChild.Name == "log")
                {
                    var value = dataNode.FirstChild.Attributes["value"].InnerText.ToString();

                    if (value.Contains("@"))
                    {//ignore log buffer location
                        fhXml.logBufNodes = new List<XmlNode>();
                        fhXml.logBufNodes.Add(dataNode.FirstChild);
                    }
                    else
                    {
                        fhXml.logNodes = new List<XmlNode>();
                        fhXml.logNodes.Add(dataNode.FirstChild);
                    }

                 }
                else if(dataNode.FirstChild.Name == "response") //our ACK of NAK 
                {
                    var attributes = dataNode.FirstChild.Attributes;
                    var maxPayloadSizeFromTargetInBytes = dataNode.FirstChild.Attributes["MaxPayloadSizeFromTargetInBytes"].InnerText.ToString();

                    if(maxPayloadSizeFromTargetInBytes != null)
                    {
                        this.firehose_config.MaxPayloadSizeToTargetInBytes = Convert.ToInt32(maxPayloadSizeFromTargetInBytes);
                    }
                    var value = dataNode.FirstChild.Attributes["value"].InnerText.ToString();


                    fhXml.responseNode = dataNode;
                    if (value == "ACK")
                    {

                        Status = true;
                    }
                    else
                    {
                        Status = false;
                    }

                }
               }
          
        }
   

    }
}

