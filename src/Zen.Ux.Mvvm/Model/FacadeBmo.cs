using System;

namespace Zen.Ux.Mvvm.Model
{
    // The Model's only responsibility is MAINTAINING THE DATA for the ViewModel
    public class FacadeBmo : BaseBmo
    {
        public Guid Id;
        public string Title;
        public string MenuName;
        public int? ImageIndex;
        
    }
}
