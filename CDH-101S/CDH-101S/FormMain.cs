using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CDH_101S
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
                
            // ダイアログ設定
            openFileDialogXA.Filter = "XA音声データファイル (*.RTF)|*.*";
            openFileDialogXA.FileName = "*.RTF";
            openFileDialogXA.Title = "XA音声データファイル";
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            openFileDialogXA.ShowDialog(this);
        }

        private void openFileDialogXA_FileOk(object sender, CancelEventArgs e)
        {
            //XAファイル読み込み
            var xa_file = new XA_File(openFileDialogXA.FileName);
            MessageBox.Show(xa_file.getSectorCount() + "セクタ読込完了");

            // CSV出力
            xa_file.OutputCSV();
        }
    }
}
