
namespace Zen.Data.QueryModel
{
    public enum LockModes : short
    {
        None,
        Read,
        Write,
        Force,
        Upgrade,
        UpgradeNoWait
    }
}