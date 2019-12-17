using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace QCOH.SAHARA
{

    /*******SAHARA HEADER: Every request contains this**********/
    [Serializable]
    public struct SAHARA_HEADER
    {
        public SAHARA_CMD command;
        public int size;
    }
    /************************************************************/

    public struct SAHARA_PBL_INFO
    {
        public string serial;
        public string msm_id;
        public string pk_hash;
        public int pbl_sw;
    }

    public enum SAHARA_MODE
    {
        SAHARA_MODE_IMAGE_PENDING = 0x00, //IF RESPONDING TO HELLO WITH THIS: SaharaCommandReadData will be returned
        SAHARA_MODE_IMAGE_TX_COMPLETE = 0x01,
        SAHARA_MODE_MEMDEBUG = 0x02,
        SAHARA_MODE_COMMAND = 0x03 //IF RESPONDING TO HELLO WITH THIS: SaharaCommandReady will be returned
    };


    //**CLIENT CMDS**/

    public enum SAHARA_EXEC_CMD
    {
        SAHARA_EXEC_CMD_NOP = 0x00,
        SAHARA_EXEC_CMD_SERIAL_NUM_READ = 0x01,
        SAHARA_EXEC_CMD_MSM_HW_ID_READ = 0x02,
        SAHARA_EXEC_CMD_OEM_PK_HASH_READ = 0x03,
        SAHARA_EXEC_CMD_SWITCH_TO_DMSS_DLOAD = 0x04,
        SAHARA_EXEC_CMD_SWITCH_TO_STREAM_DLOAD = 0x05,
        SAHARA_EXEC_CMD_READ_DEBUG_DATA = 0x06,
        SAHARA_EXEC_CMD_GET_SOFTWARE_VERSION_SBL = 0x07,

        /* place all new commands above this */
        SAHARA_EXEC_CMD_LAST,
        SAHARA_EXEC_CMD_MAX = 0x7FFFFFFF
    };

    /************************************************************/

    public struct SAHARA_REQUESTS_END_IMG_TRSFR_PACKET
    {
        public SAHARA_HEADER header;              
        int image_id;                // ID of image to be transferred
        int status;                  // OK or error condition
    };


    public struct SAHARA_SWITCH_PACKET
    {
        public SAHARA_HEADER header;
        public SAHARA_MODE mode;
    }


    // DONE packet type - sent from host to target
    //   indicates end of single image transfer
    public struct SAHARA_REQUESTS_IMGDONE_PACKET //0x05
    {
        public SAHARA_HEADER header; 
    };


    // DONE_RESP packet type - sent from target to host
    //   indicates end of all image transfers
    public struct SAHARA_RESPONSE_IMGDONE_PACKET //0x06
    {
        public SAHARA_HEADER header;
        public SAHARA_STATUS status;       // indicates if all images have been
                                              // transferred;
                                              // 0 = IMAGE_TX_PENDING
                                              // 1 = IMAGE_TX_COMPLETE
    };

    /*******Everu SAHARA PACKET  begins with one of these************************************************************/
    public enum SAHARA_CMD
    {
        SAHARA_CMD_HELLO_REQ = 0x01, // Initialize connection and protocol
        SAHARA_CMD_HELLO_RESP = 0x02, // Acknowledge connection/protocol, mode of operation
        SAHARA_CMD_READY = 0x0B,       // Indicate to host: target ready to receive client commands,
        SAHARA_CMD_EXECUTE_REQ = 0x0D, // Indicate to host: to execute a given client command
        SAHARA_CMD_EXECUTE_RESPONSE = 0x0E, // Indicate to host: target command execution status
        SAHARA_CMD_EXECUTE_DATA = 0x0F, //Executed the client cmd
        SAHARA_CMD_READ_DATA = 0x03, // Read specified number of bytes from host
        SAHARA_CMD_IMG_END_TRSFR = 0x04, // image transfer end / target transfer failure
        SAHARA_CMD_IMG_DONE_REQ = 0x05, // Acknowledgement: image transfer is complete
        SAHARA_CMD_IMG_DONE_RESP = 0x06, // Target is exiting protocol
        kSaharaCommandReset = 0x07, // Instruct target to perform a reset
        kSaharaCommandResetResponse = 0x08, // Indicate to host that target is about to reset
        kSaharaCommandMemoryDebug = 0x09, // Indicate to host: target debug mode & ready to transfer memory content
        kSaharaCommandMemoryRead = 0x0A, // Read number of bytes, starting from a specified address

        SAHARA_CMD_SWITCH_MODE  = 0x0C, // Switch to a mode defined in enum SAHARA_MODE
  

        kSaharaCommandMemoryDebug64 = 0x10,
        kSaharaCommandMemoryRead64 = 0x11,
    };

    /****************************************************************************************************************/

    public enum SAHARA_IMAGE_ID
    {
        kMbnImageNone = 0x00,
        kMbnImageOemSbl = 0x01,
        kMbnImageAmss = 0x02,
        kMbnImageOcbl = 0x03,
        kMbnImageHash = 0x04,
        kMbnImageAppbl = 0x05,
        kMbnImageApps = 0x06,
        kMbnImageHostDl = 0x07,
        kMbnImageDsp1 = 0x08,
        kMbnImageFsbl = 0x09,
        kMbnImageDbl = 0x0A,
        kMbnImageOsbl = 0x0B,
        kMbnImageDsp2 = 0x0C,
        kMbnImageEhostdl = 0x0D,
        SAHARA_IMAGE_FIREHOSE = 0x0E,
        kMbnImageNorprg = 0x0F,
        kMbnImageRamfs1 = 0x10,
        kMbnImageRamfs2 = 0x11,
        kMbnImageAdspQ5 = 0x12,
        kMbnImageAppsKernel = 0x13,
        kMbnImageBackupRamfs = 0x14,
        kMbnImageSbl1 = 0x15,
        kMbnImageSbl2 = 0x16,
        kMbnImageRpm = 0x17,
        kMbnImageSbl3 = 0x18,
        kMbnImageTz = 0x19,
        kMbnImageSsdKeys = 0x1A,
        kMbnImageGen = 0x1B,
        kMbnImageDsp3 = 0x1C,
        kMbnImageAcdb = 0x1D,
        kMbnImageWdt = 0x1E,
        kMbnImageMba = 0x1F,
        kMbnImageLast = kMbnImageMba
    };


    /***AFTER PORT IS OPENED, SAHARA SENDS THIS BACK************/
    [Serializable]
    public struct SAHARA_REQUESTS_HELLO //0x01
    {
        public SAHARA_HEADER header;
        public int version;
        public int minVersion;
        public int maxCommandPacketSize;
        public SAHARA_MODE mode;
        public int res1;
        public int res2;
        public int res3;
        public int res4;
        public int res5;
        public int res6;

      
    }
    /******************************************************************/
    public struct GPT_PARTITION_ENTRY
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public Guid partType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public Guid partId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public int firstLBA;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public int lastLBA;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 72)]
        public string partName;
    }
    
       
      
    
/*******************************************************************
* SaharaReadDataRequest
*
* When the device sends this packet
* it is requesting an image transfer and an initial
* chunk of the file for validation
********************************************************************/
    public struct SAHARA_REQUESTS_READDATA
    {
      
        public SAHARA_HEADER header;
        public SAHARA_IMAGE_ID imageID;
        public int offset;
        public int size;
    }

/*****************************************************************/


   public struct SAHARA_REQUESTS_IMG_DONE //OUGOING
    {
        public SAHARA_HEADER header;
    }

    /***RESPONSE TO SAHARA HELLO REQUEST************/
    [Serializable]
    public struct SAHARA_RESPONSE_HELLO //0x02
    {
        public SAHARA_HEADER header;
        public int version;
        public int minVersion;
        public int status; //0x00
        public SAHARA_MODE mode;
        public int res1;
        public int res2;
        public int res3;
        public int res4;
        public int res5;
        public int res6;


    }


    public struct SAHARA_RESPONSE_EXECCMD_RESPONSE
    {
        public SAHARA_HEADER header;
        public SAHARA_EXEC_CMD cmd;
        public int size;
    }
    /**********************************************************/


    public struct SAHARA_REQUEST_EXE_CMD
    {
        public SAHARA_HEADER header;
        public SAHARA_EXEC_CMD clientCmd;
      
        
    };

    public enum SAHARA_STATUS
    {
        // Success
        SAHARA_STATUS_SUCCESS = 0x00,

        // Invalid command received in current state
        SAHARA_NAK_INVALID_CMD = 0x01,

        // Protocol mismatch between host and target
        SAHARA_NAK_PROTOCOL_MISMATCH = 0x02,

        // Invalid target protocol version
        SAHARA_NAK_INVALID_TARGET_PROTOCOL = 0x03,

        // Invalid host protocol version
        SAHARA_NAK_INVALID_HOST_PROTOCOL = 0x04,

        // Invalid packet size received
        SAHARA_NAK_INVALID_PACKET_SIZE = 0x05,

        // Unexpected image ID received
        SAHARA_NAK_UNEXPECTED_IMAGE_ID = 0x06,

        // Invalid image header size received
        SAHARA_NAK_INVALID_HEADER_SIZE = 0x07,

        // Invalid image data size received
        SAHARA_NAK_INVALID_DATA_SIZE = 0x08,

        // Invalid image type received
        SAHARA_NAK_INVALID_IMAGE_TYPE = 0x09,

        // Invalid tranmission length
        SAHARA_NAK_INVALID_TX_LENGTH = 0x0A,

        // Invalid reception length
        SAHARA_NAK_INVALID_RX_LENGTH = 0x0B,

        // General transmission or reception error
        SAHARA_NAK_GENERAL_TX_RX_ERROR = 0x0C,

        // Error while transmitting READ_DATA packet
        SAHARA_NAK_READ_DATA_ERROR = 0x0D,

        // Cannot receive specified number of program headers
        SAHARA_NAK_UNSUPPORTED_NUM_PHDRS = 0x0E,

        // Invalid data length received for program headers
        SAHARA_NAK_INVALID_PDHR_SIZE = 0x0F,

        // Multiple shared segments found in ELF image
        SAHARA_NAK_MULTIPLE_SHARED_SEG = 0x10,

        // Uninitialized program header location
        SAHARA_NAK_UNINIT_PHDR_LOC = 0x11,

        // Invalid destination address
        SAHARA_NAK_INVALID_DEST_ADDR = 0x12,

        // Invalid data size receieved in image header
        SAHARA_NAK_INVALID_IMG_HDR_DATA_SIZE = 0x13,

        // Invalid ELF header received
        SAHARA_NAK_INVALID_ELF_HDR = 0x14,

        // Unknown host error received in HELLO_RESP
        SAHARA_NAK_UNKNOWN_HOST_ERROR = 0x15,

        // Timeout while receiving data
        SAHARA_NAK_TIMEOUT_RX = 0x16,

        // Timeout while transmitting data
        SAHARA_NAK_TIMEOUT_TX = 0x17,

        // Invalid mode received from host
        SAHARA_NAK_INVALID_HOST_MODE = 0x18,

        // Invalid memory read access
        SAHARA_NAK_INVALID_MEMORY_READ = 0x19,

        // Host cannot handle read data size requested
        SAHARA_NAK_INVALID_DATA_SIZE_REQUEST = 0x1A,

        // Memory debug not supported
        SAHARA_NAK_MEMORY_DEBUG_NOT_SUPPORTED = 0x1B,

        // Invalid mode switch
        SAHARA_NAK_INVALID_MODE_SWITCH = 0x1C,

        // Failed to execute command
        SAHARA_NAK_CMD_EXEC_FAILURE = 0x1D,

        // Invalid parameter passed to command execution
        SAHARA_NAK_EXEC_CMD_INVALID_PARAM = 0x1E,

        // Unsupported client command received
        SAHARA_NAK_EXEC_CMD_UNSUPPORTED = 0x1F,

        // Invalid client command received for data response
        SAHARA_NAK_EXEC_DATA_INVALID_CLIENT_CMD = 0x20,

        // Failed to authenticate hash table
        SAHARA_NAK_HASH_TABLE_AUTH_FAILURE = 0x21,

        // Failed to verify hash for a given segment of ELF image
        SAHARA_NAK_HASH_VERIFICATION_FAILURE = 0x22,

        // Failed to find hash table in ELF image
        SAHARA_NAK_HASH_TABLE_NOT_FOUND = 0x23,

        // Place all new error codes above this
        SAHARA_NAK_LAST_CODE,

        SAHARA_NAK_MAX_CODE = 0x7FFFFFFF // To ensure 32-bits wide
    };

}