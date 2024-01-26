using System.Text;
using UnityEditor;

namespace ProceduralPlant.Editor
{
    [CustomEditor(typeof(ProceduralPlant.Plant))]
    public class PlantEditor : UnityEditor.Editor
    {
        private SerializedProperty speciesProperty;
        private SerializedProperty randomSeedProperty;
        private SerializedProperty qualityLevelProperty;

        private void OnEnable()
        {
            speciesProperty = this.serializedObject.FindProperty("m_Species");
            randomSeedProperty = this.serializedObject.FindProperty("m_RandomSeed");
            qualityLevelProperty = this.serializedObject.FindProperty("m_QualityLevel");
        }

        public override void OnInspectorGUI()
        {
            var plantComponent = this.target as Plant;
            if (plantComponent == null) return;
            
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.PropertyField(speciesProperty);
            EditorGUILayout.PropertyField(randomSeedProperty);
            EditorGUILayout.PropertyField(qualityLevelProperty);

            var errorBuilder = new StringBuilder();
            if (EditorGUI.EndChangeCheck() || plantComponent.lindenmayerSystem == null)
            {
                this.serializedObject.ApplyModifiedProperties();
                plantComponent.Refresh(errorBuilder);
            }

            if (errorBuilder.Length > 0)
            {
                errorBuilder.Remove(0, 1);
                string error = errorBuilder.ToString();
                if (!string.IsNullOrEmpty(error))
                {
                    EditorGUILayout.HelpBox(error, MessageType.Error);
                }
            }
        }
    }
}