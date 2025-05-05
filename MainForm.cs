namespace SeikoHelper
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        private void buttonEntryList_Click(object sender, EventArgs e)
        {
            Cursor previousCursor = Cursor.Current;

            try
            {
                // �������J�[�\���iWaitCursor�j��ݒ�
                Cursor.Current = Cursors.WaitCursor;

                EntryList.CreateEntryList(textBoxEntryList.Text);
            }
            finally
            {
                // �J�[�\�������ɖ߂�
                Cursor.Current = previousCursor;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

            MainMenu mainMenu = new(GlobalV.EventName);
            mainMenu.Show();
        }
    }
}
