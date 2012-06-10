using System;

namespace Zen.Ux.WinApp.Mvp.Views
{    
    
    public interface IBaseView
    {

        /// <summary>Close the instance
        /// </summary>
        void Close();

        /// <summary>Confirm user action before proceeding
        /// </summary>        
        bool ConfirmAction(string message);

        /// <summary>Display error to user
        /// </summary>
        /// <param name="exc"></param>
        void ShowError(Exception exc);

        /// <summary>Display info to user
        /// </summary>        
        void ShowMessage(string message);


        /* Note: Events thrown by the presenter can be subscribed to in the IViews's constructor. 
        public View() { Presenter.StatusEvent += StatusEventHandler; } */

        /* Note: Events thrown by the view can be handled by the presenter.        
        event EventHandler LoadCompleteEvent; */

        /* Note: Values that the presenter gets from the view. 
        object SomeUxInput { get; } */

        /* Note: Values we set from the presenter on the view          
        string SomeUxDisplay { set; } */

        /* Note: Methods called by the presenter and implemented by the view. 
         _view.UpdateDisplays();*/
    }
}