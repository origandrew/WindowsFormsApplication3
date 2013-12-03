﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
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

        String key = "waiting...";
        System.Threading.Thread thread;

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // If there is an image and it has a location,  
            // paint it when the Form is repainted. 
            base.OnPaint(e);
            DrawString();
        }

        public void DrawString()
        {
            System.Drawing.Graphics formGraphics = this.CreateGraphics();
            string drawString = key;
            System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 16);
            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            float x = 150.0F;
            float y = 50.0F;
            System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
            formGraphics.DrawString(drawString, drawFont, drawBrush, x, y, drawFormat);
            drawFont.Dispose();
            drawBrush.Dispose();
            formGraphics.Dispose();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.A:
                    key = "left";
                    this.Refresh();
                    break;
                case Keys.D:
                    key = "right";
                    this.Refresh();
                    break;
                case Keys.W:
                    key = "forward";
                    this.Refresh();
                    break;
                default:
                    break;
            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            key = "waiting...";
            this.Refresh();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            thread = new System.Threading.Thread(SerialSender);
            thread.Start();
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            thread.Join();
        }

        public void SerialSender()
        {
            //create an Serial Port object
            SerialPort serialPort = new SerialPort();
            serialPort.BaudRate = 9600;
            serialPort.DataBits = 8;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            //open serial port
            serialPort.Open();

            while (true)
            {
                Console.WriteLine(key);
                short roll = ((short) 0).LimitToRange(1000,2000);
                short pitch = ((short)0).LimitToRange(1000, 2000);
                short yaw = ((short)0).LimitToRange(1000, 2000);
                short throttle = ((short)2000).LimitToRange(1000, 2000);
                MemoryStream stream = new MemoryStream();
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(roll);
                    writer.Write(pitch);
                    writer.Write(yaw);
                    writer.Write(throttle);
                    writer.Write((short)0);
                    writer.Write((short)0);
                    writer.Write((short)0);
                    writer.Write((short)0);
                }
                sendRequestMSP(serialPort, generateRequestMSP(MSP_SET_RAW_RC, stream.ToArray()));
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void sendRequestMSP(SerialPort serialPort, List<Byte> bytes)
        {
            serialPort.Write(bytes.ToArray(), 0, bytes.Count);
            Console.WriteLine(bytes.ToString());
        }

        private List<Byte> generateRequestMSP(int msp, byte[] bytes)
        {
            if (msp < 0)
            {
                return null;
            }

            List<Byte> message = new List<Byte>();
            message.AddRange(Encoding.ASCII.GetBytes(MSP_HEADER));
            byte checksum = 0;

            byte size = (byte)((bytes != null ? bytes.Length : 0) & 0xFF);
            message.Add(size);
            checksum ^= (byte)(size & 0xFF);

            message.Add((byte)(msp & 0xFF));
            checksum ^= (byte)(msp & 0xFF);

            if (bytes != null) {
                foreach (byte b in bytes){
                  message.Add((byte)(b & 0xFF));
                  checksum ^= (byte)(b & 0xFF);
                }
          }
          message.Add(checksum);

          return message;
        }
    }

    public static class InputExtensions
    {
        public static short LimitToRange(
            this short value, short inclusiveMinimum, short inclusiveMaximum)
        {
            if (value < inclusiveMinimum) { return inclusiveMinimum; }
            if (value > inclusiveMaximum) { return inclusiveMaximum; }
            return value;
        }
    }
}
