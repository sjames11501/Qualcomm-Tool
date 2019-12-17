/*****************************************************************************
 *
 * STRUCTS AND CLASSES USED FOR QC SAHARA PROTOCOL
 *
 *****************************************************************************/
/*===========================================================================*/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using QCOH.FIREHOSE;

namespace QCOH.SAHARA
{



    public class SAHARA_MANAGER
    {



        #region HELPER FUNCTIONS, bytes-to-string, bytes-to-struct,etc
        /**HELPER FUNCTIONS, bytes-to-string, bytes-to-struct**/
        private SAHARA_MODE mode { get; set; }
        private static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        public static object RawDeserialize(byte[] rawData, int position, Type
       anyType)
        {
            int rawsize = Marshal.SizeOf(anyType);
            if (rawsize > rawData.Length)
                return null;
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.Copy(rawData, position, buffer, rawsize);
            object retobj = Marshal.PtrToStructure(buffer, anyType);
            Marshal.FreeHGlobal(buffer);
            return retobj;
        }
        public static Byte[] SerializeMessage<T>(T msg) where T : struct
        {
            int objsize = Marshal.SizeOf(typeof(T));
            Byte[] ret = new Byte[objsize];
            IntPtr buff = Marshal.AllocHGlobal(objsize);
            Marshal.StructureToPtr(msg, buff, true);
            Marshal.Copy(buff, ret, 0, objsize);
            Marshal.FreeHGlobal(buff);
            return ret;
            /******************************************************/
            #endregion

        }


        private SerialPortCommuncationManager _spcm { get; set; }
        private SAHARA_PBL_INFO _pblInfo = new SAHARA_PBL_INFO();
        private uiManager _ui { get; set; }
        private SAHARA_MODE deviceMode { get; set; }
        public bool connected { get; set; }
        public QCOH.FIREHOSE.FirehoseManager _fhManager { get; set; }

        public SAHARA_MANAGER(uiManager ui,FirehoseManager fhm, SerialPortCommuncationManager spcm)
        {
            this._spcm = spcm;
            this._fhManager = fhm;
            this._ui = ui;
            openPort();

        }
        private void sendBytes(byte[] bytes)
        {
            _spcm.Send(bytes);
            Thread.Sleep(80);
        }



        private bool validateResponse(SAHARA_CMD expectedCMD, byte[] response)
        {
            int numBytes = response.Length;
         

            if (expectedCMD == SAHARA_CMD.SAHARA_CMD_READ_DATA && numBytes == 20)
            {
                return true;

            }


            if (expectedCMD == SAHARA_CMD.SAHARA_CMD_HELLO_REQ && numBytes == 48)
            {
                return true;

            }


            if (expectedCMD == SAHARA_CMD.SAHARA_CMD_IMG_END_TRSFR && numBytes == 16)
            {
                return true;

            }

            if (expectedCMD == SAHARA_CMD.SAHARA_CMD_IMG_DONE_RESP && numBytes == 16)
            {
                var x = RawDeserialize(response, 0, typeof(SAHARA_RESPONSE_IMGDONE_PACKET));

            }

            if (expectedCMD == SAHARA_CMD.SAHARA_CMD_READY && numBytes == 8)
            {
                var x = RawDeserialize(response, 0, typeof(SAHARA_RESPONSE_IMGDONE_PACKET));
                return true;

            }
            if (expectedCMD == SAHARA_CMD.SAHARA_CMD_EXECUTE_RESPONSE && numBytes == 16)
            {
              
                return true;

            }
            return false;

        }
        public bool validateClientCmd(SAHARA_CMD expectedCMD, byte[] response )
        {

  
            int expectedLen = Marshal.SizeOf(typeof(SAHARA_RESPONSE_EXECCMD_RESPONSE));
            //incorrect len
            if (response.Length != expectedLen)
            {
                return false;
            }

            if(response.Length == expectedLen )
            {
                var respStruct  = (SAHARA_RESPONSE_EXECCMD_RESPONSE)RawDeserialize(response, 0, typeof(SAHARA_RESPONSE_EXECCMD_RESPONSE));
                if(respStruct.header.command == expectedCMD)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            else
            {
                return false;
            }


        }


      
        private byte[] getFH()
        {
            var model = this._ui.getSelectedModel();

            if (model == "LG D500")
            {
                var x = Properties.Resources.LG_D500;
                return x;
            }

            if (model.Contains("5044R"))
            {
                var x = Properties.Resources.FH_5044R;
                return x;
            }

            if (model.Contains("4044C"))
            {
                var x = Properties.Resources.FH_4044C;
                return x;
            }
            if (model.Contains("4044L"))
            {
                var x = Properties.Resources.FH_4044L;
                return x;
            }

            if (model.Contains("5098O"))
            {
                var x = Properties.Resources.FH_5098O;
                return x;
            }

            if (model.Contains("5046S"))
            {
                var x = Properties.Resources.FH_5046S;
                return x;
            }

            if (model.Contains("A576CC"))
            {
                var x = Properties.Resources.FH_A576CC;
                return x;
            }

            if (model.Contains("4044T"))
            {
                var x = Properties.Resources.FH_4044T_3;
                return x;
            }
            if (model.Contains("4044W"))
            {
                var x = Properties.Resources.FH_4044W;
                return x;
            }
            if (model.Contains("4060W"))
            {
                var x = Properties.Resources._4060W;
                return x;
            }
            if (model.Contains("4060A"))
            {
                var x = Properties.Resources.FH_4060A;
                return x;
            }

            if (model.Contains("4044L"))
            {
                var x = Properties.Resources.FH_4044L;
                return x;
            }
            return null;
        }
        private  void sendFlashLoader(SAHARA_REQUESTS_READDATA initReq)
        {
            // 1. RESPOND TO READDATA with initial 52 byes, loop until finished.
            // 2. RECEIVE 0x04 SAHARA_END_IMAGE_TX packet
            // 3. RESPOND WITH 0x05 SAHARA_DONE paket
            // 4. RECEIVE 0x06 DONE RESP

            //initReq is the initial FlashProgram req from the device//
            int bytesSent = 0;
            int i = 1;
            bool doneSending = false;


            //Continue our loop until done sending flash loader
            while(!doneSending){
                
                int bytesToSend = initReq.size;
                int offset = initReq.offset;


                var fh = this.getFH();
                Stream stream = new MemoryStream(fh);

                using (BinaryReader reader = new BinaryReader(stream))
                {
                    if(initReq.offset > 8192)
                    {

                    }
                    //respond to REQ - sent requested data//
                    byte[] buffer = new byte[bytesToSend];
                    reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                    reader.Read(buffer, 0, bytesToSend);
                    _spcm.Send(buffer);
                    bytesSent += bytesToSend;
                    //read next req and loop
                    var response = _spcm.Receive(200);


                    if (i > 41)
                    {

                    }
                    //Empty buffer for some reason, delay and try again
                    if (response.Length == 0 && i > 40)
                    {
                        Thread.Sleep(5);
                        response = _spcm.Receive(0);
                    }



                    if (validateResponse(SAHARA_CMD.SAHARA_CMD_READ_DATA, response))
                    {
                        var response2 = (SAHARA_REQUESTS_READDATA)RawDeserialize(response, 0, typeof(SAHARA_REQUESTS_READDATA));
                        initReq = response2;
                        this._ui.updateProgressBar();
                        i++; continue;
                    }


                    /////////////////////////////////////////////////////////////Image transfer is done

                    else if (validateResponse(SAHARA_CMD.SAHARA_CMD_IMG_END_TRSFR, response))
                    {

                        //this._ui.DisplayData(uiManager.MessageType.Incoming, "RECEIVED END IMG TRANSFER PAKCET " + "\n", false);
                        //this._ui.DisplayData(uiManager.MessageType.Outgoing, "SENDING DONE REQ " + "\n", false);
                        SAHARA_REQUESTS_IMG_DONE imgDonePkt = new SAHARA_REQUESTS_IMG_DONE();
                        imgDonePkt.header.command = SAHARA_CMD.SAHARA_CMD_IMG_DONE_REQ;
                        imgDonePkt.header.size = Marshal.SizeOf(typeof(SAHARA_REQUESTS_IMG_DONE));
                        _spcm.Send(SerializeMessage(imgDonePkt));
                        Thread.Sleep(81);
                        //////////////////////////////////////////////////////////////////////////////|

                        var comBuffer = _spcm.Receive(100);
                        var doneResp = (SAHARA_RESPONSE_IMGDONE_PACKET)RawDeserialize(comBuffer, 0, typeof(SAHARA_RESPONSE_IMGDONE_PACKET));
                        if (doneResp.status == SAHARA_STATUS.SAHARA_STATUS_SUCCESS)
                        {
                            this._ui.clearProgress();
                            this._ui.DisplayData(uiManager.MessageType.Outgoing,"..OK\n", false);

                            this.mode = SAHARA_MODE.SAHARA_MODE_IMAGE_TX_COMPLETE;
                            this._ui.toggleFHButtons();
                            this._fhManager.setSPCM(this._spcm);
                            this._fhManager.setUIManager(this._ui);

                            this._fhManager.ConnectToFlashProg();
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
           
        }

  
        private void SendHelloResponse(SAHARA_REQUESTS_HELLO pkt, SAHARA_MODE mode)
        {
            //mode = desired mode

            if (connected)
            {

                if (mode == SAHARA_MODE.SAHARA_MODE_IMAGE_PENDING)
                {
                    pkt.header.command = SAHARA_CMD.SAHARA_CMD_HELLO_RESP;
                    pkt.mode = SAHARA_MODE.SAHARA_MODE_IMAGE_PENDING;
                    pkt.header.size = Marshal.SizeOf(typeof(SAHARA_REQUESTS_HELLO));
                    pkt.maxCommandPacketSize = 0;
                    this.mode = SAHARA_MODE.SAHARA_MODE_IMAGE_PENDING;
                    this._ui.DisplayData(uiManager.MessageType.Outgoing, "Sending Hello response." + "\n", false);
                    byte[] bytes = SerializeMessage(pkt);
                    sendBytes(bytes);
                }

                if (mode == SAHARA_MODE.SAHARA_MODE_COMMAND)
                {
                    pkt.header.command = SAHARA_CMD.SAHARA_CMD_HELLO_RESP;
                    pkt.mode = SAHARA_MODE.SAHARA_MODE_COMMAND;
                    pkt.header.size = Marshal.SizeOf(typeof(SAHARA_REQUESTS_HELLO));
                    pkt.maxCommandPacketSize = 0;
                    this.mode = SAHARA_MODE.SAHARA_MODE_COMMAND;
                    //this._ui.DisplayData(uiManager.MessageType.Outgoing, "Sending Hello response." + "\n", false);
                    byte[] bytes = SerializeMessage(pkt);
                    sendBytes(bytes);
                   

                }

                var responseBytes = _spcm.Receive(0);
                //rdy for client commands
                if (validateResponse(SAHARA_CMD.SAHARA_CMD_READY, responseBytes)) 
                {
                    this.dumpDeviceInfo();
                    switchMode(SAHARA_MODE.SAHARA_MODE_IMAGE_PENDING);
                   
                    return;

                }
                
                
                //Sahara is requesting image
                if (validateResponse(SAHARA_CMD.SAHARA_CMD_READ_DATA, responseBytes))
                {

                    this.mode = SAHARA_MODE.SAHARA_MODE_IMAGE_PENDING;
                    var rdq = (SAHARA_REQUESTS_READDATA)RawDeserialize(responseBytes, 0, typeof(SAHARA_REQUESTS_READDATA));
                    this._ui.DisplayData(uiManager.MessageType.Outgoing, "Sending Firehose loader..." + "", false);
                    sendFlashLoader(rdq);

                }

         
                   
                

            }
        }
        private void dumpDeviceInfo()
        {
            ReadData(SAHARA_EXEC_CMD.SAHARA_EXEC_CMD_MSM_HW_ID_READ);
            ReadData(SAHARA_EXEC_CMD.SAHARA_EXEC_CMD_SERIAL_NUM_READ);
            ReadData(SAHARA_EXEC_CMD.SAHARA_EXEC_CMD_OEM_PK_HASH_READ);
        }
        private void ReadData(SAHARA_EXEC_CMD cmd)
        {
            //send req pkt - MSM HW ID
            var newPkt = new SAHARA_REQUEST_EXE_CMD();
            newPkt.header.command = SAHARA_CMD.SAHARA_CMD_EXECUTE_REQ;
            newPkt.header.size = Marshal.SizeOf(typeof(SAHARA_REQUEST_EXE_CMD));
            newPkt.clientCmd = cmd;
            byte[] bytes = SerializeMessage(newPkt);
            sendBytes(bytes);
            var response1 = _spcm.Receive(10);

            if (validateResponse((SAHARA_CMD.SAHARA_CMD_EXECUTE_RESPONSE), response1))
            {
                var responsePkt = (SAHARA_RESPONSE_EXECCMD_RESPONSE)RawDeserialize(response1, 0, typeof(SAHARA_RESPONSE_EXECCMD_RESPONSE));
                var size = responsePkt.size;
                newPkt.header.command = SAHARA_CMD.SAHARA_CMD_EXECUTE_DATA;
                byte[] bytes2 = SerializeMessage(newPkt);
                sendBytes(bytes2);
               
            }
            else
            {
                //error
            }

            var response2 = _spcm.Receive(10);

            if (cmd == SAHARA_EXEC_CMD.SAHARA_EXEC_CMD_MSM_HW_ID_READ)
            {
                Array.Reverse(response2, 0,response2.Length);

                var hwid = BitConverter.ToString(response2).Replace("-", string.Empty);
                hwid =  hwid.Substring(0, 14);
                this._pblInfo.msm_id = hwid;
                this._ui.DisplayData(uiManager.MessageType.Outgoing, "MSM_HW_ID: " + hwid + "\n", false);

            }

            if (cmd == SAHARA_EXEC_CMD.SAHARA_EXEC_CMD_OEM_PK_HASH_READ)
            {
                var sn = BitConverter.ToString(response2).Replace("-", string.Empty);

                this._pblInfo.pk_hash = sn;
                //this._ui.DisplayData(uiManager.MessageType.Outgoing, "OEM PK_HASH: " + sn + "\n", false);
            }

            if (cmd == SAHARA_EXEC_CMD.SAHARA_EXEC_CMD_SERIAL_NUM_READ)
            {
                var sn = BitConverter.ToString(response2).Replace("-", string.Empty);
             
                this._pblInfo.serial = sn;
                this._ui.DisplayData(uiManager.MessageType.Outgoing, "Serial Number: " + sn + "\n", false);
            }
        }
        private bool switchMode(SAHARA_MODE mode)
        {
            SAHARA_SWITCH_PACKET pkt = new SAHARA_SWITCH_PACKET();
            pkt.header.command = SAHARA_CMD.SAHARA_CMD_SWITCH_MODE;
            pkt.header.size = Marshal.SizeOf(typeof(SAHARA_SWITCH_PACKET));
            pkt.mode = SAHARA_MODE.SAHARA_MODE_IMAGE_PENDING;
            byte[] bytes = SerializeMessage(pkt);
            sendBytes(bytes);
            var responseBytes = _spcm.Receive(0);

            if (validateResponse(SAHARA_CMD.SAHARA_CMD_HELLO_REQ, responseBytes))
            {
                var data2 = RawDeserialize(responseBytes, 0, typeof(SAHARA_REQUESTS_HELLO));
                var pkt2 = (SAHARA_REQUESTS_HELLO)data2;
                if(mode == SAHARA_MODE.SAHARA_MODE_IMAGE_PENDING)
                {
                    SendHelloResponse(pkt2, SAHARA_MODE.SAHARA_MODE_IMAGE_PENDING);
                }
              

            }
            return false;
        }
        private void hangHack(SAHARA_MODE mode)
        { 
                  
            //No response after opening port, try to send hello response
         
                SAHARA_SWITCH_PACKET pkt = new SAHARA_SWITCH_PACKET();
                pkt.header.command = SAHARA_CMD.SAHARA_CMD_SWITCH_MODE;
                pkt.header.size = Marshal.SizeOf(typeof(SAHARA_SWITCH_PACKET));
                pkt.mode = SAHARA_MODE.SAHARA_MODE_IMAGE_PENDING;
                byte[] bytes = SerializeMessage(pkt);
                _spcm.Send(bytes);
                var responseBytes = _spcm.Receive(0);
            
        }
        private void openPort()
        {
            this._spcm.openPort();
     
            if (this.connected) return;


            var  response = _spcm.Receive(50);
            if (response.Length == 0)
            {
                hangHack(SAHARA_MODE.SAHARA_MODE_COMMAND);
                
            }

          if (response.Length == Marshal.SizeOf(typeof(SAHARA_REQUESTS_HELLO)))
            {

                this.connected = true;


                this._ui.DisplayData(uiManager.MessageType.Outgoing, "Received Hello" + "\n", false);
                var data2 = RawDeserialize(response, 0, typeof(SAHARA_REQUESTS_HELLO));
                var pkt = (SAHARA_REQUESTS_HELLO)data2;

                this.SendHelloResponse(pkt, SAHARA_MODE.SAHARA_MODE_COMMAND); //switch to cmd mode
          


            }
            else if (response.Length == 0)
            {

                this._ui.DisplayData(uiManager.MessageType.Outgoing, "No response from device. Try restarting the device." + "\n", false);
                this._ui.unlock();
                this._spcm.close();



            }

        }
       

    

       
    }




 
    #region STRUCT FOR PBL INFO
    public struct SAHARA_PBLIFNO
    {
        int serial;
        int msm_id;
        string pk_hash;
        int pbl_sw;
    }
    #endregion







}
