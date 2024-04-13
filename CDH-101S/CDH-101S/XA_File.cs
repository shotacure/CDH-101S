using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Collections.Specialized.BitVector32;
using System.Windows.Forms;

namespace CDH_101S
{
    internal class XA_File
    {
        private const int size_of_file_header = 44;
        private const int size_of_sector = 2352;

        private int size_of_total;
        private int size_of_total_header;
        private int size_of_total_sector;

        private int header_sector_count;

        private byte[] read_header;
        private byte[] read_sector;
        private int sector_count = 0;

        private List<XA_Sector> sectors;

        public XA_File(string FileName)
        {
            FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);

            read_header = new byte[size_of_file_header];

            fs.Read(read_header, 0, size_of_file_header);

            // トータルサイズ
            size_of_total = BitConverter.ToInt32(read_header, 4);

            // ファイルヘッダサイズ
            size_of_total_header = BitConverter.ToInt32(read_header, 16);

            // ファイルデータサイズ
            size_of_total_sector = BitConverter.ToInt32(read_header, 40);

            // ヘッダ上のセクタ数
            header_sector_count = size_of_total_sector / size_of_sector;


            sectors = new List<XA_Sector>();

            while (header_sector_count > sector_count)
            {
                read_sector = new byte[size_of_sector];
                fs.Read(read_sector, 0, size_of_sector);
                var sector = new XA_Sector(read_sector);
                sectors.Add(sector);
                sector_count++;
            }
            fs.Close();
        }

        public void OutputCSV()
        {
            // CSV出力
            string mydoc_path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var sw = new StreamWriter(Path.Combine(mydoc_path, "xa.csv"), false);

            var line = new List<string>
            {
                "minutes",
                "seconds",
                "sectors",
                "mode",
                "file_number",
                "channel",
                "eof_marker",
                "real_time",
                "form",
                "trigger",
                "data",
                "audio",
                "video",
                "end_of_record",
                "emphasis",
                "bits_per_sample",
                "sample_rate",
                "stereo_channel",
            };
            sw.WriteLine(string.Join(",", line));

            foreach (var sector in sectors)
            {
                sw.WriteLine(sector.WriteCSVLine());
            }

            sw.Close();
        }

        public int getSectorCount()
        {
            return sector_count;
        }
    }
}
