using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCOH.FIREHOSE
{
  
    public sealed class firehose_pkts
    {
        public string xml;
        public static string pkt_fhConfig()
        {
            var config = new FIREHOSE_CONFIG
            {
                Version = 4,
                MemoryName = "emmc",
                SkipWrite = 0,
                ZLPAwareHost = 0,
                SkipStorageInit = 0,
                ActivePartition = 0,
                MaxPayloadSizeToTargetInBytes = 8192,
                AckRawDataEveryNumPackets = 1
            };

            var x =
              $"<?xml version = \"1.0\"?><data><configure MaxPayloadSizeToTargetInBytes=\"{config.MaxPayloadSizeToTargetInBytes.ToString()}\"/></data>";
            //  var x =
            //    $"<?xml version = \"1.0\"?><data><configure MemoryName=\"{config.MemoryName}\" ZLPAwareHost=\"{config.ZLPAwareHost.ToString()}\" SkipStorageInit=\"{config.SkipStorageInit.ToString()}\" SkipWrite=\"{config.SkipWrite.ToString()}\" MaxPayloadSizeToTargetInBytes=\"{config.MaxPayloadSizeToTargetInBytes.ToString()}\"/></data>";
            return x;
        }
        public static string pkt_setAckRaw(int val)
        {
            var x =
           $"<?xml version=\"1.0\" ?><data><configure AckRawData=\"{val}\"/></data>";
            return x;
        }
        public static string pkt_peekMem(uint address64, int size)
        {
            var x = $" <?xml version=\"1.0\" ?><data><peek address64=\"{address64}\" SizeInBytes=\"{size}\"/></data>";
            return x;
        }
        public static string pkt_pokeMem(uint address64, int size, byte[] val)
        {
            var vl = util.ByteArrayToString(val);
            vl = "0x01";
            string s = "001";
            var x = $" <?xml version=\"1.0\" ?><data><poke address64=\"{address64}\" SizeInBytes=\"{s}\"  value=\"{vl}\"/></data>";
            return x;
        }
        public static string pkt_eMMCinfo()
        {
            var x =
           $"<?xml version=\"1.0\" ?><data><eMMCinfo></data>";
            return x;
        }
        public static string pkt_Program(int nPartSectors,string fileName, int startSector)
        {
           
            var x =  $" <?xml version=\"1.0\" ?><data><program SECTOR_SIZE_IN_BYTES=\"512\" filename=\"{fileName}\" num_partition_sectors=\"{nPartSectors}\" physical_partition_number=\"0\" start_sector=\"{startSector.ToString()}\"/></data>";
            return x;
        }
        //DOES NOT WORK -- REMOVED ON PRODUCTION DEVICES...?
        public static string pkt_readSimLock()
        {
            string pkt = "<?xml version=\"1.0\" ?><data><simlock read=\"1\" len=\"32\"/></data>";

            return pkt;
        }
        //Only in 5044R
        public static string pkt_readSecBoot()
        {
            string pkt = "<?xml version=\"1.0\" ?><data><getSecureBootStatus/></data>";

            return pkt;
        }
        //DOES NOT WORK -- DEPRECIATED?? Read from RAM instead.
        public static string pkt_readSerialNumber()
        {
            string pkt = "<?xml version=\"1.0\" ?><data><getserialnum /></data>";
            return pkt;
        }
        public static string pkt_readIMEI()
        {
            string pkt = "<?xml version=\"1.0\" ?><data><readIMEI len=\"32\"/></data>";
            return pkt;

        }
        public static string pkt_sendNop()
        {
            string nop_pkt = "<?xml version=\"1.0\" ?><data><nop /></data>";
            return nop_pkt;
        }
        public static string pkt_sendReset()
        {
            string reset_pkt = "<?xml version=\"1.0\" ?><data><power value=\"reset\"/></data>";
            return reset_pkt;
        }
        public static string pkt_read(int sectorSize, int numPartitionSectors, int physicalPartNum, int startSector)
        {
            var x =
                $"<?xml version = \"1.0\"?><data><read SECTOR_SIZE_IN_BYTES=\"{sectorSize.ToString()}\" num_partition_sectors=\"{numPartitionSectors.ToString()}\" physical_partition_number=\"{physicalPartNum.ToString()}\" start_sector=\"{startSector.ToString()}\"/> </data>";
            return x;
        }
        public static string pkt_write_simlock(int physicalPartNum, int startSector,int len)
        {
            var x =
           $"<?xml version=\"1.0\" ?><data><simlock SECTOR_SIZE_IN_BYTES=\"512\" num_partition_sectors=\"2048\" physical_partition_number=\"{physicalPartNum.ToString()}\" start_sector=\"{startSector.ToString()}\" len=\"{len.ToString()}\" /> </data>";
            return x;
        }
    }
}
