namespace Zen.Ux
{
    /// <summary>
    /// Interface for a central console that holds all the logic
    /// needed for determining the next view in an application.
    /// </summary>
    public interface INavigationController 
    {        
        //IViewFactory ViewFactory { get; }
       
        void NavigateTo(string urn);

        void NavigateTo(string urn, object[] args);

        bool ConfirmQuit();

        void Quit();

        //bool ConfirmAction(string message);

        //void ShowAbout();

        //void ShowHelp();

        //void ShowMessage(string message);

        //void ShowError(Exception exc);
    }
}   