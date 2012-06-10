using System;

namespace Zen.Svcs.DataModel
{
    [Serializable] 
    public sealed class FacadeDto 
    {
        public Guid Id { get; set; } 

        // everything the app will need to display the Facade menu or link
        // ...move to repository and sync using DiscoveryProxy & DynamicRouter
        public string Title { get; set; }
        public string MenuName { get; set; }
        public int? ImageIndex { get; set; }        
    }
}