using System.IO;
using UnityEditor;
using UnityEngine;

namespace ProceduralPlant
{
    public class PlantSpecies : ScriptableObject
    {
        [SerializeField] [TextArea(3, 32)] private string m_Rule            = "F;F->FF-[-F+F+F]+[+F-F-F];";
        [SerializeField] [Range(0, 180)]   private float  m_Angle           = 22.5f;
        [SerializeField] [Min(0)]          private float  m_Length          = 3;
        [SerializeField] [Range(0, 1)]     private float  m_ThinningRate    = 0.3f;
        [SerializeField] [Min(0)]          private float  m_InitialDiameter = 1.0f;
        [SerializeField] [Min(1)]          private int    m_IterationCount  = 5;
        
        public string description     => m_Rule;
        public float  angle           => m_Angle;
        public float  length          => m_Length;
        public float  thinningRate    => m_ThinningRate;
        public float  initialDiameter => m_InitialDiameter;
        public int    iterationCount  => m_IterationCount;

        [SerializeField] private Material m_BranchMaterial = null;
        public Material branchMaterial => m_BranchMaterial;
        
        [SerializeField] private Material m_LeafMaterial = null;
        public Material leafMaterial   => m_LeafMaterial;
        
        #if UNITY_EDITOR

        public bool dirty = false;

        [MenuItem("Assets/Create/Plant/Plant Species")]
        public static void Create()
        {
            const string defaultFilePath = "New Plant Species.asset";
            var filePath = Selection.assetGUIDs.Length == 0
                ? Path.Combine("Assets", defaultFilePath)
                : AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            filePath = Path.Combine(Directory.Exists(filePath) ? filePath : Path.GetDirectoryName(filePath), defaultFilePath);
            filePath = AssetDatabase.GenerateUniqueAssetPath(filePath);

            PlantSpecies plantSpecies = ScriptableObject.CreateInstance<PlantSpecies>();
            plantSpecies.Figure_1_25();
            AssetDatabase.CreateAsset(plantSpecies, filePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(plantSpecies));
            EditorUtility.FocusProjectWindow();
            EditorGUIUtility.PingObject(plantSpecies);
        }

        [ContextMenu("Templates/Plant-like structure (Figure 1.24.c)")]
        void Figure_1_24_C()
        {
            Undo.RecordObject(this, "Apply template");
            m_Rule     = "F;\n" + 
                                "F->FF-[-F+F+F]+[+F-F-F];";
            m_Angle           = 22.5f;
            m_Length          = 1.35f;
            m_ThinningRate    = 0.3f;
            m_InitialDiameter = 0.5f;
            m_IterationCount  = 3;
            this.dirty = true;
        }

        [ContextMenu("Templates/Plant-like structure (Figure 1.24.f)")]
        void Figure_1_24_F()
        {
            Undo.RecordObject(this, "Apply template");
            m_Rule     = "X;\n" + 
                                "X->F-[[X]+X]+F[+FX]-X;\n" + 
                                "F->FF;";
            m_Angle           = 22.5f;
            m_Length          = 2f;
            m_ThinningRate    = 0.3f;
            m_InitialDiameter = 0.5f;
            m_IterationCount  = 3;
            this.dirty = true;
        }

        [ContextMenu("Templates/A three-dimensional bush-like structure (Figure 1.25)")]
        void Figure_1_25()
        {
            Undo.RecordObject(this, "Apply template");
            m_Rule     = "A;\n" +
                                "A->[&FL!A]/////'[&FL!A]///////'[&FL!A];\n" +
                                "F->S/////F;\n" +
                                "S->FL;\n" +
                                "L->['''^^{-f+f+f-|-f+f+f}];";
            m_Angle           = 22.5f;
            m_Length          = 1f;
            m_ThinningRate    = 0.5f;
            m_InitialDiameter = 1.0f;
            m_IterationCount  = 7;
            this.dirty = true;
        }

        [ContextMenu("Templates/A plant generated by an L-system (Figure 1.26)")]
        void Figure_1_26()
        {
            Undo.RecordObject(this, "Apply template");
            m_Rule     = "\"plant\";\n" +
                         "\"plant\"->\"internode\"+[\"plant\"+\"flower\"]--//[--\"leaf\"]\"internode\"[++\"leaf\"]-[\"plant\"\"flower\"]++\"plant\"\"flower\";\n" +
                         "\"internode\"->F\"seg\"[//&&\"leaf\"][//^^\"leaf\"]F\"seg\";\n" +
                         "\"seg\"-0.1>\"seg\"[//&&\"leaf\"][//^^leaf]F\"seg\";\n" +
                         "\"seg\"-0.7>\"seg\"F\"seg\";\n" +
                         "\"seg\"-0.2>\"seg\";\n" +
                         "\"leaf\"->['{+f-ff-f+|+f-ff-f}];\n" +
                         "\"flower\"->[&&&\"pedicel\"'/\"wedge\"////\"wedge\"////\"wedge\"////\"wedge\"////\"wedge\"];\n" +
                         "\"pedicel\"->FF;\n" +
                         "\"wedge\"->['^F][{&&&&-f+f|-f+f}];";
            m_Angle           = 18f;
            m_Length          = 0.6f;
            m_ThinningRate    = 0.5f;
            m_InitialDiameter = 0.1f;
            m_IterationCount  = 5;
            this.dirty = true;
        }
        
        #endif
    }
}
