using System;
using System.Reflection;
using UnityEngine;

namespace RoadImporterXML
{
    //code from RoadImporter: https://github.com/citiesskylines-csur/RoadImporter
    public class RIUtils
    {
        public static void CopyFromGame(object source, object target)
        {
            Debug.Log("Invoke copyfromgame");
            Debug.Log($"source:{source}, target:{target}");
            if (source == null || target == null) return;
            FieldInfo[] fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo fieldInfo in fields)
            {
                FieldInfo gameFieldInfo = source.GetType().GetField(fieldInfo.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                Debug.Log(gameFieldInfo);
                Debug.Log(fieldInfo);
                if (gameFieldInfo.FieldType == typeof(NetInfo.Lane[]))
                {
                    NetInfo.Lane[] gameObjects = (NetInfo.Lane[])gameFieldInfo.GetValue(source);
                    int len = gameObjects.Length;
                    CSNetInfo.Lane[] savedObjects = new CSNetInfo.Lane[len];
                    for (int i = 0; i < len; i++)
                    {
                        savedObjects[i] = new CSNetInfo.Lane();
                        CopyFromGame(gameObjects[i], savedObjects[i]);
                    }
                    fieldInfo.SetValue(target, savedObjects);
                }
                else if (gameFieldInfo.FieldType == typeof(NetInfo.Segment[]))
                {
                    NetInfo.Segment[] gameObjects = (NetInfo.Segment[])gameFieldInfo.GetValue(source);
                    int len = gameObjects.Length;
                    CSNetInfo.Segment[] savedObjects = new CSNetInfo.Segment[len];
                    for (int i = 0; i < len; i++)
                    {
                        savedObjects[i] = new CSNetInfo.Segment();
                        CopyFromGame(gameObjects[i], savedObjects[i]);
                    }
                    fieldInfo.SetValue(target, savedObjects);
                }
                else if (gameFieldInfo.FieldType == typeof(NetInfo.Node[]))
                {
                    NetInfo.Node[] gameObjects = (NetInfo.Node[])gameFieldInfo.GetValue(source);
                    int len = gameObjects.Length;
                    CSNetInfo.Node[] savedObjects = new CSNetInfo.Node[len];
                    for (int i = 0; i < len; i++)
                    {
                        savedObjects[i] = new CSNetInfo.Node();
                        CopyFromGame(gameObjects[i], savedObjects[i]);
                    }
                    fieldInfo.SetValue(target, savedObjects);
                }
                else if (gameFieldInfo.FieldType == typeof(NetLaneProps))
                {
                    NetLaneProps gameObjects = (NetLaneProps)gameFieldInfo.GetValue(source);
                    int len = gameObjects.m_props.Length;
                    CSNetInfo.Prop[] savedObjects = new CSNetInfo.Prop[len];
                    for (int i = 0; i < len; i++)
                    {
                        savedObjects[i] = new CSNetInfo.Prop();
                        CopyFromGame(gameObjects.m_props[i], savedObjects[i]);
                    }
                    fieldInfo.SetValue(target, savedObjects);
                }
                else if (gameFieldInfo.FieldType == typeof(Vector3))
                {
                    Vector3 vec = (Vector3)gameFieldInfo.GetValue(source);
                    float[] vals = { vec[0], vec[1], vec[2] };
                    fieldInfo.SetValue(target, vals);
                }
                else if (gameFieldInfo.FieldType.IsEnum)
                {
                    object val = Enum.ToObject(fieldInfo.FieldType, gameFieldInfo.GetValue(source));
                    fieldInfo.SetValue(target, val);
                }
                else if (gameFieldInfo.FieldType.IsSubclassOf(typeof(PrefabInfo)))
                {
                    PrefabInfo prefab = (PrefabInfo)gameFieldInfo.GetValue(source);
                    if (prefab == null)
                    {
                        fieldInfo.SetValue(target, "");
                    }
                    else
                    {
                        fieldInfo.SetValue(target, prefab.name);
                    }

                }
                else
                {
                    fieldInfo.SetValue(target, gameFieldInfo.GetValue(source));
                }

            }
        }

        public static void RefreshRoadEditor()
        {
            typeof(RoadEditorMainPanel).GetMethod("Clear", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(UnityEngine.Object.FindObjectOfType<RoadEditorMainPanel>(), null);
            typeof(RoadEditorMainPanel).GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(UnityEngine.Object.FindObjectOfType<RoadEditorMainPanel>(), null);
            typeof(RoadEditorPanel).GetMethod("OnObjectModified", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(UnityEngine.Object.FindObjectOfType<RoadEditorPanel>(), null);
        }
    }
}
