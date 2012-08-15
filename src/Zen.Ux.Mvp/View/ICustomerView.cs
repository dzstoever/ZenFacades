namespace Zen.Ux.Mvp.View
{
    /// <summary>
    /// Represents a single customer view
    /// </summary>
    public interface ICustomerView : IView
    {
        int CustomerId { get; set; }
        string Company { get; set; }
        string City { get; set; }
        string Country { get; set; }
        string Version { get; set; }
    }
}
