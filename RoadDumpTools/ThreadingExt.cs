using ColossalFramework.UI;
using ICities;

namespace RoadDumpTools
{
    public class ThreadingExt : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
           if (mode == LoadMode.LoadAsset || mode == LoadMode.NewAsset)
            {
               UIView v = UIView.GetAView();
               UIComponent uic = v.AddUIComponent(typeof(NetDumpPanel));   
            }

        }

    }
}
