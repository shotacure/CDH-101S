using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CDH_101S
{
    internal class XA_Sector
    {
        private const int size_of_sector = 2352;
        private readonly byte[] sync = { 0x00, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00 };

        private int minutes;
        private int seconds;
        private int sectors;
        private int mode;
        private int file_number;
        private int channel;

        private byte submode;
        private bool eof_marker;
        private bool real_time;
        private bool form;
        private bool trigger;
        private bool data;
        private bool audio;
        private bool video;
        private bool end_of_record;
        private enum submode_flag : byte
        {
            eof_marker = 0b1 << 7,
            real_time = 0b1 << 6,
            form = 0b1 << 5,
            trigger = 0b1 << 4,
            data = 0b1 << 3,
            audio = 0b1 << 2,
            video = 0b1 << 1,
            end_of_record = 0b1 << 0,
        }

        private byte coding_info;
        private bool emphasis;
        private int bits_per_sample;
        private int sample_rate;
        private int stereo_channel;
        private enum coding_info_flag : byte
        {
            emphasis = 0b1 << 6,
            bits_per_sample = 0b11 << 4,
            bits_per_sample_4 = 0b00 << 4,
            bits_per_sample_8 = 0b01 << 4,
            sample_rate = 0b11 << 2,
            sample_rate_37800 = 0b00 << 2,
            sample_rate_18900 = 0b01 << 2,
            stereo_channel = 0b11 << 0,
            stereo_channel_1 = 0b00 << 0,
            stereo_channel_2 = 0b01 << 0,
        }

        public XA_Sector(byte[] sector)
        {
            // 同期ビット判定
            IReadOnlyList<byte> segment = new ArraySegment<byte>(sector, 0, sync.Length);
            if (!segment.SequenceEqual<byte>(sync))
            {
                throw new ArgumentException("sync");
            }

            // 分
            int.TryParse(BitConverter.ToString(new byte[1] { sector[12] }), out minutes);

            // 秒
            int.TryParse(BitConverter.ToString(new byte[1] { sector[13] }), out seconds);

            // セクタ
            int.TryParse(BitConverter.ToString(new byte[1] { sector[14] }), out sectors);

            // モード
            mode = (int)sector[15];

            // ファイル番号
            file_number = (int)sector[16];

            // チャンネル
            channel = (int)sector[17];

            // サブモード
            submode = sector[18];
            eof_marker = (submode & (byte)submode_flag.eof_marker) == (byte)submode_flag.eof_marker;
            real_time = (submode & (byte)submode_flag.real_time) == (byte)submode_flag.real_time;
            form = (submode & (byte)submode_flag.form) == (byte)submode_flag.form;
            trigger = (submode & (byte)submode_flag.trigger) == (byte)submode_flag.trigger;
            data = (submode & (byte)submode_flag.data) == (byte)submode_flag.data;
            audio = (submode & (byte)submode_flag.audio) == (byte)submode_flag.audio;
            video = (submode & (byte)submode_flag.video) == (byte)submode_flag.video;
            end_of_record = (submode & (byte)submode_flag.end_of_record) == (byte)submode_flag.end_of_record;

            // 符号化情報
            coding_info = sector[19];
            emphasis = (coding_info & (byte)coding_info_flag.emphasis) == (byte)coding_info_flag.emphasis;
            switch (coding_info & (byte)coding_info_flag.bits_per_sample)
            {
                case (byte)coding_info_flag.bits_per_sample_4:
                    bits_per_sample = 4;
                    break;
                case (byte)coding_info_flag.bits_per_sample_8:
                    bits_per_sample = 8;
                    break;
            }
            switch (coding_info & (byte)coding_info_flag.sample_rate)
            {
                case (byte)coding_info_flag.sample_rate_37800:
                    sample_rate = 37800;
                    break;
                case (byte)coding_info_flag.sample_rate_18900:
                    sample_rate = 18900;
                    break;
            }
            switch (coding_info & (byte)coding_info_flag.stereo_channel)
            {
                case (byte)coding_info_flag.stereo_channel_1:
                    stereo_channel = 1;
                    break;
                case (byte)coding_info_flag.stereo_channel_2:
                    stereo_channel = 2;
                    break;
            }
        }

        public string WriteCSVLine()
        {
            var line = new List<string>
            {
                minutes.ToString(),
                seconds.ToString(),
                sectors.ToString(),
                mode.ToString(),
                file_number.ToString(),
                channel.ToString(),
                eof_marker.ToString(),
                real_time.ToString(),
                form.ToString(),
                trigger.ToString(),
                data.ToString(),
                audio.ToString(),
                video.ToString(),
                end_of_record.ToString(),
                emphasis.ToString(),
                bits_per_sample.ToString(),
                sample_rate.ToString(),
                stereo_channel.ToString(),
            };

            return string.Join(",", line);
        }
    }
}
