namespace Zen.Ux.Mvp.View
{
    /// <summary>
    /// Respresents login view with credentials.
    /// </summary>
    public interface ILoginView : IView
    {
        string UserName { get; }
        string Password { get; }
    }
}
