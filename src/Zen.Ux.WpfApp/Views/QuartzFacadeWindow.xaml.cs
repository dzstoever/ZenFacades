using Zen.Log;
using Zen.Ux.Mvvm.ViewModel;

namespace Zen.Ux.WpfApp.Views
{
    
    public partial class QuartzFacadeWindow : IQuartzView
    {
        const string WindowTitle = "Quartz Facade";

        private readonly ILogger _log = Aspects.GetLogger();
        private QuartzVM _viewModel;// { get { return this.DataContext as QuartzVM; } }

        public QuartzFacadeWindow(QuartzVM viewModel)
        {
            InitializeComponent();            
            Title = WindowTitle;
            _viewModel = viewModel;
        }

        
        
    }
}
