using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Management;

namespace QCOH
{



    public class SerialPortIO
    {
        public SerialPort sp;
        private int customDelay; //Milliseconds- how long to wait before reading data from sport
        public SerialPortIO(SerialPort sp1, int delay)
        {

            this.sp = sp1;

            try
            {
                sp.Open();
            }
              catch(Exception e)
            {
               
 
                sp.Open();
            }
              

    
        


        }


   

        public void ClearInput()
        {
            try
            {
                this.sp.DiscardInBuffer();
                this.sp.DiscardOutBuffer();
                this.sp.BaseStream.Flush();
            } catch(Exception e)
            {

            }
        }

       public void Close()
        {
            try
            {
                this.sp.Dispose();
                this.sp.Close();

            }
            catch (Exception e)
            {

            }
           
        }
        public void Send(byte[] dataToSend, int length)
        {
            try
            {
               // OnSendingEventArgs e = new OnSendingEventArgs(dataToSend);
               // this.HandleOnSending(e);
                this.sp.Write(dataToSend,0,length);
               
            }
            catch (Exception arg)
            {
             
                throw;
            }


        }

    }
}
