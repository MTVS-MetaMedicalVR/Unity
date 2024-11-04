#if (UNITY_EDITOR) 
using UnityEngine;
using UnityEditor;

/*
 *  All this code does is to provide the "Build" and "Clear" buttons in the inspector for the 
 *  Elevator Builder script. This is purely cosmetic, since you can achieve the same through
 *  the context menu of the script in the inspector (The same menu of "Remove Component"...)
 *  
 *  So, if you eventually get any warnings or errors, 
 *  consider deleting this file althogheter or moving it temporally out of the \Editor folder
 */

namespace ModernElevator {
    [CustomEditor(typeof(ElevatorBuilder))]
    [HelpURL("https://www.pipasjourney.com/damianGonzalez/modern_elevator/")]

    public class ElevatorBuilderEditor : Editor {
        public Texture editorLogo;
        public override void OnInspectorGUI() {
            //logo
            if (editorLogo != null) {
                GUI.DrawTexture(new Rect(0, 20, Screen.width, 90), editorLogo, ScaleMode.ScaleAndCrop);
                GUILayout.Space(20 + 90 + 20);
            }

            //display ElevatorBuilder exposed variables
            base.OnInspectorGUI();

            //and buttons below
            ElevatorBuilder script = (ElevatorBuilder)target;

            GUILayout.Space(50);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("✗▶  Clear and Build", GUILayout.Height(40))) {
                script.ClearAndBuild();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);


            GUILayout.BeginHorizontal();
            if (GUILayout.Button("✗  Clear", GUILayout.Height(20))) {
                script.ClearAll();
            }
            if (GUILayout.Button("▶  Build", GUILayout.Height(20))) {
                script.Build();
            }
            if (GUILayout.Button("↳  Detach", GUILayout.Height(20))) {
                script.Detach();
            }
            GUILayout.EndHorizontal();

            //help box

            EditorGUILayout.HelpBox("If you need assistance " +
                "from the developer, please visit the online documentation (you can click the info button next to the component name)", MessageType.Info, true);
            

            //margin below
            GUILayout.Space(10);
        }

    }
}
#endif