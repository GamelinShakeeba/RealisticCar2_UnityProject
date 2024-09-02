using System.IO;
using Cinemachine;
using UnityEditor;
using UnityEngine;

namespace MyCar
{
    [CustomEditor(typeof(ArcadeVehicleController))]
    public class ArcadeVehicleControllerEditor : Editor
    {
        private ArcadeVehicleController controller;
        private SerializedObject controllerSO;

        public void OnEnable() 
        {
            controller = (ArcadeVehicleController) target;
            controllerSO = new SerializedObject(target);
        }

        public override void OnInspectorGUI()
        {
            // Define the colors
            Color primaryColor = new Color(0f, 0.78f, 1f);

            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
            headerStyle.fontSize = 27;
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.normal.textColor = primaryColor;
            headerStyle.padding = new RectOffset(1, 1, 1, 1);

            GUIStyle headingStyle = new GUIStyle(EditorStyles.boldLabel);
            headingStyle.normal.textColor = primaryColor;

            // Create the buttons
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.fontSize = 12;
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.padding = new RectOffset(5, 5, 5, 5);

            controllerSO.Update();

            GUILayout.Space(16f);
            GUILayout.Label("Ground settings", headingStyle);
            EditorGUILayout.PropertyField(controllerSO.FindProperty("groundCheck"), new GUIContent("Ground check method", "Choose the method for checking when the vehicle is in the ground."));
            EditorGUILayout.PropertyField(controllerSO.FindProperty("drivableSurface"), new GUIContent("Drivable layer", "Choose the layer where vehicle can drive in."));
            EditorGUILayout.PropertyField(controllerSO.FindProperty("frictionMaterial"), new GUIContent("Friction material"));

            GUILayout.Space(16f);
            GUILayout.Label("Vehicle", headingStyle);
            EditorGUILayout.PropertyField(controllerSO.FindProperty("maxSpeed"), new GUIContent("Max speed"));
            EditorGUILayout.PropertyField(controllerSO.FindProperty("accelaration"), new GUIContent("Acceleration"));
            EditorGUILayout.PropertyField(controllerSO.FindProperty("decelerationMultiplier"), new GUIContent("Deceleration"));
            EditorGUILayout.PropertyField(controllerSO.FindProperty("turn"), new GUIContent("Turning"));
            EditorGUILayout.PropertyField(controllerSO.FindProperty("gravity"), new GUIContent("Gravity"));
            EditorGUILayout.PropertyField(controllerSO.FindProperty("downforce"), new GUIContent("Down force"));
            EditorGUILayout.PropertyField(controllerSO.FindProperty("bodyTilt"), new GUIContent("Body turn tilt"));

            GUILayout.Space(16f);
            GUILayout.Label("Air vehicle controlling", headingStyle);
            EditorGUILayout.PropertyField(controllerSO.FindProperty("airControl"), new GUIContent("Air control enabled", "If enabled, can control vehicle turning in air"));
        
            GUILayout.Space(16f);
            GUILayout.Label("Drift Mode", headingStyle);
            SerializedProperty driftMode = controllerSO.FindProperty("driftMode");
            EditorGUILayout.PropertyField(driftMode, new GUIContent("Drift mode enabled?"));
            if(driftMode.boolValue == true) {
                EditorGUILayout.PropertyField(controllerSO.FindProperty("driftMultiplier"), new GUIContent("Drift multiplier"));
            }

            GUILayout.Space(16f);
            GUILayout.Label("Rigid bodys", headingStyle);
            EditorGUILayout.PropertyField(controllerSO.FindProperty("rb"), new GUIContent("Inner body"));
            EditorGUILayout.PropertyField(controllerSO.FindProperty("carBody"), new GUIContent("Car body"));

            GUILayout.Space(16f);
            GUILayout.Label("Curves", headingStyle);
            EditorGUILayout.PropertyField(controllerSO.FindProperty("frictionCurve"), new GUIContent("Friction curve"));
            EditorGUILayout.PropertyField(controllerSO.FindProperty("turnCurve"), new GUIContent("Turning curve"));

            GUILayout.Space(16f);
            GUILayout.Label("Visual settings", headingStyle);
            EditorGUILayout.PropertyField(controllerSO.FindProperty("bodyMesh"), new GUIContent("Body mesh"));
            EditorGUILayout.PropertyField(controllerSO.FindProperty("frontWheels"), new GUIContent("Front wheels"));
            EditorGUILayout.PropertyField(controllerSO.FindProperty("rearWheels"), new GUIContent("Rear wheels"));

            GUILayout.Space(16f);
            GUILayout.Label("Tire effects", headingStyle);
            SerializedProperty useEffects = controllerSO.FindProperty("useEffects");
            EditorGUILayout.PropertyField(useEffects, new GUIContent("Tire effects enabled?"));
            if(useEffects.boolValue == true) {
                EditorGUILayout.PropertyField(controllerSO.FindProperty("RLSkid"), new GUIContent("Rear left skid"));
                EditorGUILayout.PropertyField(controllerSO.FindProperty("RLSmoke"), new GUIContent("Rear left smoke"));
                EditorGUILayout.PropertyField(controllerSO.FindProperty("RRSkid"), new GUIContent("Rear right skid"));
                EditorGUILayout.PropertyField(controllerSO.FindProperty("RRSmoke"), new GUIContent("Rear right smoke"));
            }

            GUILayout.Space(16f);
            GUILayout.Label("Sound settings", headingStyle);
            EditorGUILayout.PropertyField(controllerSO.FindProperty("engineSound"), new GUIContent("Engine sound"));
            EditorGUILayout.PropertyField(controllerSO.FindProperty("minPitch"), new GUIContent("Minimum pitch"));
            EditorGUILayout.PropertyField(controllerSO.FindProperty("maxPitch"), new GUIContent("Maximum pitch"));
            EditorGUILayout.PropertyField(controllerSO.FindProperty("skidSound"), new GUIContent("Skid sound"));

            controllerSO.ApplyModifiedProperties();
        }
    }
}