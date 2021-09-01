using UnityEngine;
using UnityEditor;

namespace Ballistics
{
    [CustomEditor(typeof(LivingEntity), true)]
    public class LivingEntityEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Set me as ParentLivingEntity for child-LivingEntityColliders"))
            {
                foreach (Object obj in targets)
                {
                    LivingEntity me = (LivingEntity)obj;
                    LivingEntityCollider[] childs = me.transform.GetComponentsInChildren<LivingEntityCollider>();

                    for (int i = 0; i < childs.Length; i++)
                    {
                        childs[i].ParentLivingEntity = me;
                    }
                }
            }
        }
    }
}