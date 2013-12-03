using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/** Adapted from **/
namespace WindowsFormsApplication3
{
    class Serial
    {
        private static String MSP_HEADER = "$M<";

        private static int
            MSP_IDENT = 100,
            MSP_STATUS = 101,
            MSP_RAW_IMU = 102,
            MSP_SERVO = 103,
            MSP_MOTOR = 104,
            MSP_RC = 105,
            MSP_RAW_GPS = 106,
            MSP_COMP_GPS = 107,
            MSP_ATTITUDE = 108,
            MSP_ALTITUDE = 109,
            MSP_BAT = 110,
            MSP_RC_TUNING = 111,
            MSP_PID = 112,
            MSP_BOX = 113,
            MSP_MISC = 114,
            MSP_MOTOR_PINS = 115,
            MSP_BOXNAMES = 116,
            MSP_PIDNAMES = 117,

            MSP_SET_RAW_RC = 200,
            MSP_SET_RAW_GPS = 201,
            MSP_SET_PID = 202,
            MSP_SET_BOX = 203,
            MSP_SET_RC_TUNING = 204,
            MSP_ACC_CALIBRATION = 205,
            MSP_MAG_CALIBRATION = 206,
            MSP_SET_MISC = 207,
            MSP_RESET_CONF = 208,

            MSP_EEPROM_WRITE = 250,

            MSP_DEBUG = 254
            ;

        public static int
            IDLE = 0,
            HEADER_START = 1,
            HEADER_M = 2,
            HEADER_ARROW = 3,
            HEADER_SIZE = 4,
            HEADER_CMD = 5,
            HEADER_ERR = 6
            ;

        byte[] inBuf = new byte[256];
        int p = 0;

        int read32() { return (inBuf[p++] & 0xff) + ((inBuf[p++] & 0xff) << 8) + ((inBuf[p++] & 0xff) << 16) + ((inBuf[p++] & 0xff) << 24); }
        int read16() { return (inBuf[p++] & 0xff) + ((inBuf[p++]) << 8); }
        int read8() { return inBuf[p++] & 0xff; }

        
    }
}
