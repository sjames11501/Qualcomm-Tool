using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QCOH.FIREHOSE;
using System.Threading;

namespace QCOH
{

    //**** Handles read/write from eMMC, and writing to SP*****/
    public class diskUtil
    {
        private SerialPortCommuncationManager _spcm { get; set; }

        public diskUtil(SerialPortCommuncationManager spcm)
        {
            this._spcm = spcm;
        }

        public bool saveFile(string path, byte[] bytes)
        {
            try
            {
                fileUtil.clearFile(path);
                fileUtil.appendToFile(path, bytes);
                return true;
            } catch(Exception e)
            {
                return false;
            }
            
        }
        public bool programPartition(string partName,int start_sec, byte[] bytes)
        {
            var offset = 0;
            var totalBytes = bytes.Length;
            var bytesWritten = 0;
            var packetSize = 16384;
            var loops = totalBytes / packetSize;
            var n512sectors = totalBytes / 512;
            int i = 0;
            var pkt = firehose_pkts.pkt_Program(n512sectors, partName, start_sec);
            WriteXml(pkt);//write intital write request packet

            //_uiManager.clearProgress();

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

                        if (isACK()) 
                        {
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
                        //_uiManager.setProgressbar(offset, totalBytes);
                        i++;

                    }
                }

            }



        }
        public byte[] readPartition(gpt_partition_entry x)
        {
            /***CONSTANTS***/
            const int SECTOR_SIZE = 512;
            List<byte[]> byteList = new List<byte[]>();
            /***COFIGURE THE CURRENT DUMP SETTINGS***/
            var SECTORS_TO_READ = (x.last_lba - x.first_lba) + 1;
            long BYTES_TO_READ = SECTORS_TO_READ * 512;
            int eMMC_Offset = x.first_lba;
            long bytesRead = 0;
            int fileOffset = 0;
            int i = 0;
            WriteXml(firehose_pkts.pkt_read(512, SECTORS_TO_READ, 0, x.first_lba));

            //loop until zero bytes left to read//
            while (true)
            {
                var buffer = this._spcm.Receive(5);

                if (isFinished(buffer))
                {
                    var bytesLeft = BYTES_TO_READ - fileOffset;
                    var buffer2 = buffer.Take((int)bytesLeft).ToArray();
                    byteList.Add(buffer2);
                    byte[] array = byteList.SelectMany(a => a).ToArray();
                    return array;
                }
                byteList.Add(buffer);
                bytesRead += buffer.Length;
                i++;
                fileOffset += buffer.Length;
            }
            var response = _spcm.Receive(100);
            return response;
        }
        public byte[] dumpPartition(gpt_partition_entry x,string path)
        {
            /***CONSTANTS***/
            const int SECTOR_SIZE = 512;

            /***COFIGURE THE CURRENT DUMP SETTINGS***/
            var SECTORS_TO_READ = (x.last_lba - x.first_lba) + 1;
            long BYTES_TO_READ = SECTORS_TO_READ * 512;
            int eMMC_Offset = x.first_lba;
            long bytesRead = 0;

            /***OFFSETS***/
            int fileOffset = 0;
            int emmcOffset = 0;
            int i = 0;
           
            WriteXml(firehose_pkts.pkt_read(512,SECTORS_TO_READ, 0, x.first_lba));
            fileUtil.clearFile(path);
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
            var response = _spcm.Receive(100);
            return response;
        }
    
        public  void writeSimLockTCL(byte[] bytes,int startSec)
        {
            var x = bytes;
            int len = bytes.Length;
            int ppn = 0;
            var pkt = firehose_pkts.pkt_write_simlock(ppn, startSec, len);
            WriteXml(pkt);
            writeRaw(x);
        }

        private void writeRaw(byte[] bytes)
        {
            SerialPortCommuncationManager _sp = _spcm;
            byte[] outputBuffer = bytes;
            _spcm.Send(outputBuffer);
            _spcm.flushBuffers();
        }

        public void WriteXml(string xml)
        {
            _spcm.flushBuffers();
            byte[] outputBuffer = Encoding.ASCII.GetBytes(xml);

            try
            {
             
             _spcm.Send(outputBuffer);
            } catch(Exception e)
            {

            }
            
        }
        private bool isACK()
        {
            FIREHOSE_RESPONSE_PACKET fhXml = new FIREHOSE_RESPONSE_PACKET();
            byte[] comBuffer = _spcm.Receive(300);
            var x = System.Text.Encoding.UTF8.GetString(comBuffer, 0, comBuffer.Length);

            if (x.Contains("\"ACK\""))
            {
                return true;
            }
            else
            {
                return false;
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
    }
}
