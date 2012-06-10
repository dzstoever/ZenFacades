using Xbehave;
using Moq;
using Zen.Ux;

namespace Zen.Xunit.Tests.Ux
{
    public class NavigationScenarios : UseLogFixture
    {
        /* Todo...
         *         
        [TestMethod]
		public void ShowHelp_should_call_MainViews_help()
		{
			var factory = new Mock<IViewFactory>();
			var mainView = new Mock<IMainView>();
			factory.Setup(f => f.CreateView<IMainView>()).Returns(mainView.Object);
			mainView.Setup(v => v.ShowHelp()).Verifiable();

			var viewmodel = new MainWindowVM(factory.Object)
			                	{
			                		ParentView = mainView.Object
			                	};
			viewmodel.ShowHelp.Execute(null);

			mainView.VerifyAll();
		}
         * 
        [Scenario]
        public virtual void NavigatorCanLaunchNonViewScreen()
        { }
                
        [Scenario]
        public virtual void NavigatorCanLaunchMainViewWindow()
        { }
        */

        [Scenario]
        public virtual void NavigatorCanLaunchOtherViewWindow()
        {
            var navigator = new Mock<INavigationController>();
            var factory = new Mock<IViewFactory>();
            var mainView = new Mock<IMainView>();
            var quartzView = new Mock<IQuartzView>();

            "Given ".Given(() =>
            {
                factory.Setup(f => f.CreateView<IQuartzView>()).Returns(quartzView.Object);
                quartzView.Setup(v => v.Show()).Verifiable();
            });
            
            "When ".When(() =>
            {
                //var viewmodel = new MainWindowVM(factory.Object);
                //viewmodel.Launch.Execute(null);                
            });
            "Then ".Then(quartzView.VerifyAll);               
            
        }        
    }
}
