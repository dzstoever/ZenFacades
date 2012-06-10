using System;
using System.Windows.Forms;
using Zen.Ux.WinApp.Mvp;
using Zen.Ux.WinApp.Mvp.Views;

namespace Zen.Ux.WinApp
{
    public partial class BaseForm : Form, IBaseView
    {
        public BaseForm()
        {
            InitializeComponent();
        }

        #region IView Members

        public bool ConfirmAction(string message)
        {
            return MessageBox.Show(this, message, "Confirm Action", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes;

        }

        public void ShowError(Exception exc)
        {
            MessageBox.Show(this, exc.Message, "Error from " + Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(this, message, "Message from " + Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        private void view1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Navigation.NavigateTo("view1");
        }

        private void view2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Navigation.NavigateTo("view2");
        }

        private void view3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Navigation.NavigateTo("view3");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Navigation.ConfirmQuit()) Navigation.Quit();
        }
        
    }
}
