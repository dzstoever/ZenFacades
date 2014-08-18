using Zen.Ux.WinApp.Mvp.Views;

namespace Zen.Ux.WinApp
{
    public partial class MainForm : BaseForm, IMainView
    {
        public MainForm()
        {
            InitializeComponent();
        }



        public new bool? ShowDialog()
        {
            throw new System.NotImplementedException();
        }
    }
}
