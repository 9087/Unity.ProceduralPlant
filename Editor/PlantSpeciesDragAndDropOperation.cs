using System.IO;
using UnityEditor;
using UnityEngine;

namespace ProceduralPlant.Editor
{
    public static class PlantSpeciesDragAndDropToScene
    {
        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            EventType eventType = Event.current.type;
            if (DragAndDrop.objectReferences.Length != 1)
                return;
            var objectReference = DragAndDrop.objectReferences[0];
            if (!(objectReference is PlantSpecies plantSpecies))
                return;
            if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (eventType == EventType.DragPerform)
                {
                    GameObject gameObject = new(GameObjectUtility.GetUniqueNameForSibling(null, Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(plantSpecies))));
                    var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    gameObject.transform.position = Physics.Raycast(ray, out var hit)
                        ? hit.point
                        : ray.origin + ray.direction * 10;
                    
                    var plantComponent = gameObject.AddComponent<Plant>();
                    var serializedObject = new SerializedObject(plantComponent);
                    serializedObject.FindProperty("m_Species").objectReferenceValue = plantSpecies;
                    serializedObject.FindProperty("m_RandomSeed").intValue = Random.Range(1, 65536);
                    serializedObject.ApplyModifiedProperties();
                    Selection.activeObject = gameObject;
                    DragAndDrop.AcceptDrag();
                }
                Event.current.Use();
            }
        }
    }
}