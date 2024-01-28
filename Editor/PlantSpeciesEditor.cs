using UnityEditor;
using UnityEngine;

namespace ProceduralPlant.Editor
{
    [CustomEditor(typeof(ProceduralPlant.PlantSpecies))]
    public class PlantSpeciesEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var plantSpecies = this.serializedObject.targetObject as PlantSpecies;
            if (plantSpecies == null) return;
            this.serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_Rule"));
            
            #region Parameters
            EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
            EditorGUI.indentLevel += 1;
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_Angle"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_Length"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_ThinningRate"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_InitialDiameter"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_IterationCount"));
            EditorGUI.indentLevel -= 1;
            #endregion
            
            #region Materials
            EditorGUILayout.LabelField("Materials", EditorStyles.boldLabel);
            EditorGUI.indentLevel += 1;
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_BranchMaterial"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_LeafMaterial"));
            EditorGUI.indentLevel -= 1;
            #endregion

            plantSpecies!.dirty |= EditorGUI.EndChangeCheck();
            if (!plantSpecies.dirty)
            {
                return;
            }

            this.serializedObject.ApplyModifiedProperties();
            EditorApplication.delayCall += () =>
            {
                foreach (var plant in FindObjectsOfType<Plant>())
                {
                    if (plant.species != this.target)
                    {
                        continue;
                    }
                    plant.Refresh();
                }
            };
            plantSpecies.dirty = false;
        }
    }
}
