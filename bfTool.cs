using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QCOH.Crypto;
using System.Threading;

namespace QCOH
{
    class bfUtil //bruteforce methods
    {
        public bool finished = false;
        public int startingN;
        Object o = new Object();
        private int sn;
        private uiManager _ui;
        private int _sID;
        int tries = 0;

        public void setUIManager(uiManager x)
        {
            this._ui = x;
        }
        public int startBF(byte[] compBytes)
        {
            var _numThreads = 10;
            var _activeThreads = 0;

            //
            var i1 = (int)Int32.MaxValue - 1;
            /**************Create threads**********************************************/
            for (int i = 0; i < _numThreads; i++)
            {
                int sn = i1 - (i * 1000000);
                Thread t = new Thread(() => bruteForce(compBytes, sn));
                t.Start();
                _activeThreads++;
            }
           /***************************************************************************/

            while (true)
            {
                if (finished)
                {
                    return _sID;
                    break;
                }
                 
            }
          
        }
        private void bruteForce(byte[] compBytes,int startingN)
        {
            var max = startingN;
            int i = startingN;
         

            /*************************************************************************************/
            while (i > startingN - 1000000)
            {

                if (finished)
                    return;

                cryptoUtil cUtil = new cryptoUtil();
                var x = Convert.ToString(i);
                var key = Encoding.ASCII.GetBytes(x);
                var encVars = cUtil.tEncrypt(key);
                var xBytes = new byte[16];//16 blank bytes
                //xor data before encryption
                var xordBytes = cUtil.xor(xBytes, encVars.encryptedSerial2, 16);
                var encryptesBytes = cUtil.encrypt(xordBytes, encVars.encryptedSerial);
                if (util.Equality(compBytes, encryptesBytes))
                {
                    lock (o)
                    {
                        finished = true;
                        _sID = i;
                        break;
                    }
               
                }

                lock (o)
                {
                    i -= 1;
                    tries++;
                    _ui.DisplayData(uiManager.MessageType.Outgoing, i + "\n", true);
                }
            }
            /*************************************************************************************/
          
        }
    }
}
