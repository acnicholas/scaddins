using System.Windows.Forms;

namespace SCaddins.RoomConvertor
{
    public partial class DesignOptionSelector : Form
    {
        public DesignOptionSelector()
        {
            InitializeComponent();
        }

        public void SetTitle(string title)
        {
            this.Text = title;
        }
    }
}
