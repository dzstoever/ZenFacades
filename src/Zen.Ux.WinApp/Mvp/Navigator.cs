using System;
using System.Reflection;
using System.Windows.Forms;
using Zen.Ioc;
using Zen.Ux.WinApp.Mvp.Views;

namespace Zen.Ux.WinApp.Mvp
{
    internal class Navigator : INavigator
    {
        /// <summary>Use to get new instances of disposed views
        /// </summary>
        private static readonly IocDI DI;

        static Navigator()
        {
            DI = ZenProvider.GetIocDI();
        }

        private static void ShowFormView(Form form)
        {
            if (!form.Visible) form.Show();
            form.Activate();
        }


        private readonly Form _view1; //main view
        public Form View2 { private get; set; }
        public Form View3 { private get; set; }
        
        public Navigator(IMainView view1)
        {
            _view1 = view1 as Form;            
        }

        public void NavigateTo(string urn)
        {
            switch (urn.ToLower())
            {
                case "view1":
                    ShowFormView(_view1);
                    break;

                case "view2":
                    if (View2 == null || View2.IsDisposed) 
                        View2 = DI.Resolve<IView2>() as Form;                    
                    ShowFormView(View2);
                    break;

                case "view3":
                    if (View3 == null || View3.IsDisposed)
                        View3 = DI.Resolve<IView3>() as Form;                    
                    ShowFormView(View3);
                    break;
                default:
                    throw new ApplicationException("Unrecognized urn.");
            }
        }

        public void NavigateTo(string urn, object[] args)
        {
            switch (urn.ToLower())
            {
                default:
                    NavigateTo(urn);//try with no args
                    break;
            }
        }

        public bool ConfirmQuit()
        {
            return MessageBox.Show("Are you sure you want to exit?", Assembly.GetEntryAssembly().GetName().Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                   == DialogResult.Yes;
        }

        public void Quit()
        {
            Environment.Exit(0);
        }

    }
}