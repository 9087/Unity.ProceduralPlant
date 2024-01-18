using System;
using System.Collections.Generic;
using ProceduralPlant.Core;
using UnityEditor;
using UnityEngine;

namespace ProceduralPlant.Editor
{
    [CustomEditor(typeof(ProceduralPlant.Plant))]
    public class PlantEditor : UnityEditor.Editor
    {
        private SerializedProperty lindenmayerSystemDescriptionProperty;
        private SerializedProperty iterationCountProperty;
        private SerializedProperty parametersInfoProperty;

        private Rect templateDropdownButtonRect;
        private static bool parameterInfoFoldout = true;
        private static bool meshFilterListFoldout = false;
        
        private void OnEnable()
        {
            lindenmayerSystemDescriptionProperty = this.serializedObject.FindProperty("m_LindenmayerSystemDescription");
            iterationCountProperty = this.serializedObject.FindProperty("m_IterationCount");
            parametersInfoProperty = this.serializedObject.FindProperty("m_ParametersInfo");
        }

        public override void OnInspectorGUI()
        {
            bool dirty = false;

            var plantController = this.target as Plant;
            if (plantController == null) return;

            using (var _ = new EditorGUILayout.HorizontalScope())
            {
                #region Lindenmayer System Description

                string lindenmayerSystemDescription =
                    EditorGUILayout.TextField(new GUIContent("Lindenmayer System Description"),
                        lindenmayerSystemDescriptionProperty.stringValue);
                if (lindenmayerSystemDescription != lindenmayerSystemDescriptionProperty.stringValue)
                {
                    dirty = true;
                    lindenmayerSystemDescriptionProperty.stringValue = lindenmayerSystemDescription;
                }

                #endregion

                #region Templates Dropdown Button
                var buttonStyle = new GUIStyle(GUI.skin.button);
                RectOffset padding = buttonStyle.padding;
                padding.left += 2;
                padding.right += 2;
                padding.top += 1;
                padding.bottom += 2;
                buttonStyle.padding = padding;
                buttonStyle.margin.top -= 1;
                buttonStyle.margin.bottom -= 1;
                buttonStyle.fontSize = 10;
                bool templatesButtonClicked = GUILayout.Button(new GUIContent("Templates"), buttonStyle, GUILayout.ExpandWidth(false));
                if (Event.current.type == EventType.Repaint)
                {
                    templateDropdownButtonRect = GUILayoutUtility.GetLastRect();
                }
                if (templatesButtonClicked)
                {
                    void Apply(object data)
                    {
                        var (template, parameterInfo, iterationCount) = ((string, ParametersInfo, int))data;
                        lindenmayerSystemDescriptionProperty.stringValue = template;
                        parametersInfoProperty.FindPropertyRelative("angle").floatValue = parameterInfo.angle;
                        parametersInfoProperty.FindPropertyRelative("length").floatValue = parameterInfo.length;
                        iterationCountProperty.intValue = iterationCount;
                        this.serializedObject.ApplyModifiedProperties();
                        plantController.Refresh();
                        this.Repaint();
                    }
                    GenericMenu menu = new GenericMenu();
                    Dictionary<string, (string, ParametersInfo, int)> templates = new()
                    {
                        {"Plant-like structure (Figure 1.24.c)",
                            ("F;F->FF-[-F+F+F]+[+F-F-F];",
                                new() { angle = 22.5f, length = 5, },
                                3)},
                        {"Plant-like structure (Figure 1.24.f)",
                            ("X;X->F-[[X]+X]+F[+FX]-X;F->FF;",
                                new() { angle = 22.5f, length = 5, },
                                3)},
                        {"A three-dimensional bush-like structure (Figure 1.25)",
                            ("A;A->[&FL!A]/////'[&FL!A]///////'[&FL!A];F->S/////F;S->FL;L->['''^^{-f+f+f-|-f+f+f}];",
                                new() { angle = 22.5f, length = 1, sideCount = 4, thinningRate = 0.5f, },
                                7)},
                    };
                    foreach (var (title, (template, parameterInfo, iterationCount_)) in templates)
                    {
                        menu.AddItem(new GUIContent(title), template == lindenmayerSystemDescriptionProperty.stringValue, Apply, (template, parameterInfo, iterationCount_));
                    }
                    menu.DropDown(templateDropdownButtonRect);
                }
                #endregion
            }

            #region Parameters Info
            parameterInfoFoldout = EditorGUILayout.Foldout(parameterInfoFoldout, new GUIContent("Parameters"));
            if (parameterInfoFoldout)
            {
                EditorGUI.indentLevel += 1;
                EditorGUI.BeginChangeCheck();

                SerializedProperty childProperty = parametersInfoProperty.Copy();
                if (childProperty.Next(true))
                {
                    do
                    {
                        EditorGUILayout.PropertyField(childProperty);
                    }
                    while (childProperty.Next(false));
                }
                dirty |= EditorGUI.EndChangeCheck();
                EditorGUI.indentLevel -= 1;
            }
            #endregion

            #region Iteration Count
            
            int iterationCount = EditorGUILayout.IntSlider(new GUIContent("Iteration Count"),
                iterationCountProperty.intValue, 0, 10);
            if (iterationCount != iterationCountProperty.intValue)
            {
                dirty = true;
                iterationCountProperty.intValue = iterationCount;
            }

            #endregion

            if (dirty || plantController.lindenmayerSystem == null)
            {
                this.serializedObject.ApplyModifiedProperties();
                plantController.Refresh();
            }

            #region MeshFilter List
            
            meshFilterListFoldout = EditorGUILayout.Foldout(meshFilterListFoldout, new GUIContent("MeshFilter List"));
            if (meshFilterListFoldout)
            {
                EditorGUI.indentLevel += 1;
                for (int i = 0; i < plantController.meshFilters.Count; i++)
                {
                    var meshFilter = plantController.meshFilters[i];
                    EditorGUILayout.ObjectField(meshFilter, typeof(MeshFilter));
                }
                EditorGUI.indentLevel -= 1;
            }

            #endregion

            if (plantController.lindenmayerSystem == null)
            {
                EditorGUILayout.HelpBox($"Invalid description: {lindenmayerSystemDescriptionProperty.stringValue}", MessageType.Error);
            }
        }
    }
}