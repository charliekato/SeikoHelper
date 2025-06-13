namespace SeikoHelper
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        private void GodoCreaterClick(object sender, EventArgs e)
        {

            MainMenu mainMenu = new(GlobalV.EventName);
            mainMenu.Show();
        }

        private void lblExtractNewRecord_Click(object sender, EventArgs e)
        {
            //NewRecordExporter newRecordExporter = new NewRecordExporter();
            NewRecordExporter.UpdateGameRecord();
            NewRecordExporter.ExeExport();

        }

        private void labelEntryList_Click(object sender, EventArgs e)
        {
            Cursor previousCursor = Cursor.Current;

            try
            {
                // 処理中カーソル（WaitCursor）を設定
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "保存先のファイルを選択してください";
                sfd.Filter = "エクセルファイル (*.xlsx)|*.xlsx|すべてのファイル (*.*)|*.*";
                sfd.DefaultExt = "xlsx";

                Cursor.Current = Cursors.WaitCursor;
                //EntryList.CreateEntryList(textBoxEntryList.Text);
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string filePath = sfd.FileName;
                    EntryList.CreateEntryList(filePath);
                }
            }
            finally
            {
                // カーソルを元に戻す
                Cursor.Current = previousCursor;
            }

        }
    }
}
