using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BinToIntelHex
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (string binFilePath in openFileDialog1.FileNames)
                {
                    ConvertBinToIntelHex(binFilePath, binFilePath + ".hex");
                }
            }

            this.Close();
        }

        private void ConvertBinToIntelHex(string binFilePath, string hexFilePath)
        {
            byte[] binData = File.ReadAllBytes(binFilePath);
            using (StreamWriter writer = new StreamWriter(hexFilePath))
            {
                int address = 0;
                int recordLength = 16;

                for (int i = 0; i < binData.Length; i += recordLength)
                {
                    int len = Math.Min(recordLength, binData.Length - i);
                    string record = CreateHexRecord(address, binData, i, len);
                    writer.WriteLine(record);
                    address += len;
                }

                writer.WriteLine(":00000001FF"); // End of File Record
            }
        }

        private string CreateHexRecord(int address, byte[] data, int start, int length)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(":{0:X2}{1:X4}00", length, address);

            int checksum = length + (address >> 8) + (address & 0xFF);

            for (int i = start; i < start + length; i++)
            {
                byte b = data[i];
                sb.AppendFormat("{0:X2}", b);
                checksum += b;
            }

            checksum = (-checksum) & 0xFF;
            sb.AppendFormat("{0:X2}", checksum);

            return sb.ToString();
        }
    }
}
