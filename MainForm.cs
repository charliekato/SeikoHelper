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
                // �������J�[�\���iWaitCursor�j��ݒ�
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "�ۑ���̃t�@�C����I�����Ă�������";
                sfd.Filter = "�G�N�Z���t�@�C�� (*.xlsx)|*.xlsx|���ׂẴt�@�C�� (*.*)|*.*";
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
                // �J�[�\�������ɖ߂�
                Cursor.Current = previousCursor;
            }

        }
    }
}
