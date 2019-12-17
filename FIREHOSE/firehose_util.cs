using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text;
using System.Threading.Tasks;

namespace QCOH.FIREHOSE
{
    struct FIREHOSE_CONN
    {
        public bool status;
        private string imei;
        private SerialPortCommuncationManager _spcm;
        public uiManager _ui;
        public List<gpt_partition_entry> _partitions;
        private int MaxBytesFromPhone;
        private int eMMCSize;

    };
    public struct FIREHOSE_GPT
    {
        public gpt_header header;
        public List<gpt_partition_entry> entries;
    }
    public struct FIREHOSE_DEV_INFO
    {
        public string imei { get; set; }
        public byte[] hwIMEI { get; set; }
        public string pth { get; set; }
        public string hs_pn { get; set; }
        public string prov_id { get; set; } 
        public int emmcSize { get; set; }
    }

    }
    public enum FH_IO
    {
       DISK_SECTOR_SIZE = 512,
       MAX_PACKET_SIZE = 1024 * 1024,
       MAX_SECTORS_TO_READ = 1024
    }

    public struct FIREHOSE_CONFIG
    {
        public int Version;
        public string MemoryName;
        public int SkipWrite;
        public int SkipStorageInit;
        public int ZLPAwareHost;
        public int ActivePartition;
        public int MaxPayloadSizeToTargetInBytes;
        public int AckRawDataEveryNumPackets;
        public int maxPayloadSizeFromTargetInBytes;
    }




    public class FIREHOSE_RESPONSE_TYPE
    {
        public bool isACK { get; set; }
        public bool isLog { get; set; }
    }


    public class FIREHOSE_RESPONSE_PACKET
    {
        public IList<XmlNode> logBufNodes;
        public IList<XmlNode> logNodes;
        public object responseNode;
        public bool success;
    }


    public struct gpt_partition_entry
    {

        public string partTypeGUID;
        public string partID;
        public int first_lba;
        public int last_lba;
        public byte[] flags;
        public string partName;
        public int num512Sectors
        {

            get
            {
                var numSectorsToRead = (this.last_lba - this.first_lba) + 1;
                return numSectorsToRead;
            }
        }
    }



    public struct gpt_header
    {
        // GPT signature 0x5452415020494645
        public string signature;
        // GPT version - 1.0
        public int revision;
        // size of this struct in bytes. Must be > 92 and < logical sctor size
        public int header_size;
        // CRC32 of this struct
        public int crc_header;
        // zero'ed
        public int reserved;
        // lba of this struct
        public int current_lba;
        // lba of the backup struct
        public int backup_lba;
        // first lba that can be used by a partition entry
        public int first_usable_lba;
        // last lba that can be used by a partition entry
        public int last_usable_lba;
        // unique id for the disk
        public byte[] disk_guid;
        // lba address to the first partition entry
        public int starting_lba_pe;
        // number of partitions
        public int number_partitions;
        // size of each partition entry (128 x 2^N - today is 128)
        public int size_partition_entries;
        // CRC32 for the partitions (number_partitions * size_partition_entries)
        int  crc_partition;
        // 420 bytes for a disc with 512b of sector size...
        // it should be sector size - 92
        byte reserved2;
    }
   


