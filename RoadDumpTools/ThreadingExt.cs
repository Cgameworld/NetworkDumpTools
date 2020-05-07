using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using System.Reflection;
using UnityEngine;

namespace RoadDumpTools
{
    public class ThreadingExt : LoadingExtensionBase
    {

        public override void OnLevelLoaded(LoadMode mode)
        {
            //only start loading in asset editor
            if (mode == LoadMode.LoadAsset || mode == LoadMode.NewAsset)
            {
                UIView v = UIView.GetAView();
                UIComponent uic = v.AddUIComponent(typeof(NetDumpPanel));

                NetDumpPanel.instance.Show(); //extra needed to intialize
                RoadExtrasAlert.instance.Show(); //init

                GameObject.FindObjectOfType<ToolController>().eventEditPrefabChanged += (info) =>
                {
                    if (info.GetType().ToString() == "NetInfo")
                    {
                        var texQual = typeof(OptionsGraphicsPanel).GetField("m_TexturesQuality", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<OptionsGraphicsPanel>.instance) as SavedInt;
                        if (texQual.value != 2)
                        {
                            Lib.ExtraUtils.ShowAlertWindow("Network Dump Tools", "Warning: \"Texture Quality\" in the vanilla options is not set to high, change to dump textures at full resolution");
                        }
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
}
