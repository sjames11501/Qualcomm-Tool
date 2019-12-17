using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Ports;
using System.Threading.Tasks;

namespace QCOH
{
    public class SerialPortCommuncationManager 
    {

        private readonly object syncObject = new object();
        private SerialPortIO spio;
        private SerialPort sp;
        public byte[] receiveBuffer;


        public void close()
        {
            this.spio.Close();
        }

     public void setupSP(SerialPort x)
        {
            sp = x;
        }
        public SerialPortIO SPIO
        {
            get { return this.spio; }
     
        }

        public void flushBuffers()
        {
            this.spio.ClearInput();
        }
        public void openPort()
        {
            this.spio = new SerialPortIO(sp, 1000);
        }
        public void Send(byte[] request)
        {
            this.spio.Send(request, (int)request.Length);
        }

        public byte[] Receive(int timeSpan)
        {
            Thread.Sleep(timeSpan);
            int numBytes = this.spio.sp.BytesToRead;
            byte[] buffer = new byte[numBytes];
            this.spio.sp.Read(buffer, 0, numBytes);

         
        

            return buffer;
        }

        public byte[] ReciveWithLimit(int timeSpan, int numBytes)
        {
            Thread.Sleep(timeSpan);
            byte[] buffer = new byte[numBytes];
            this.spio.sp.Read(buffer, 0, numBytes);
            spio.sp.DiscardOutBuffer();
            spio.sp.DiscardInBuffer();




            return buffer;
        }

        public byte[] ReciveWithLimitLong(int timeSpan, long numBytes)
        {
            Thread.Sleep(timeSpan);
            byte[] buffer = new byte[numBytes];
            this.spio.sp.Read(buffer, 0, 2147483647);
            spio.sp.DiscardOutBuffer();
            spio.sp.DiscardInBuffer();




            return buffer;
        }






    }
}
