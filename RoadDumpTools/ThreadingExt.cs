using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace RoadDumpTools
{
    public class ThreadingExt : LoadingExtensionBase
    {

        public override void OnLevelLoaded(LoadMode mode)
        {
            UIView v = UIView.GetAView();
            UIComponent uic = v.AddUIComponent(typeof(NetDumpPanel));

            NetDumpPanel.instance.Show(); //extra needed to intialize

            GameObject.FindObjectOfType<ToolController>().eventEditPrefabChanged += (info) =>
            {
                Debug.Log("objectype" + info.GetType());

                if (info.GetType().ToString() == "NetInfo")
                {
                    NetDumpPanel.instance.Show();
                }
                else
                {
                    NetDumpPanel.instance.Hide();
                    Debug.Log("not NetInfo");
                }
            };



        }

    }
}
