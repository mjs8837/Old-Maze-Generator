using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InfinadeckSpawner))]
[CanEditMultipleObjects]
public class InfinadeckSpawnerEditor : Editor
{

    SerializedProperty pluginVersion;
    SerializedProperty cameraRig;
    SerializedProperty headset;
    SerializedProperty firstLevel;
    SerializedProperty movementLevel;
    SerializedProperty guaranteeDestroyOnLoad;
    SerializedProperty speedGain;
    SerializedProperty worldScale;
    SerializedProperty correctPosition;
    SerializedProperty correctRotation;
    SerializedProperty correctScale;

    void OnEnable()
    {
        pluginVersion = serializedObject.FindProperty("pluginVersion");
        cameraRig = serializedObject.FindProperty("cameraRig");
        headset = serializedObject.FindProperty("headset");
        firstLevel = serializedObject.FindProperty("firstLevel");
        movementLevel = serializedObject.FindProperty("movementLevel");
        guaranteeDestroyOnLoad = serializedObject.FindProperty("guaranteeDestroyOnLoad");
        speedGain = serializedObject.FindProperty("speedGain");
        worldScale = serializedObject.FindProperty("worldScale");
        correctPosition = serializedObject.FindProperty("correctPosition");
        correctRotation = serializedObject.FindProperty("correctRotation");
        correctScale = serializedObject.FindProperty("correctScale");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.LabelField("Plugin Version " + pluginVersion.stringValue);
        if (cameraRig.objectReferenceValue && headset.objectReferenceValue)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Key References");
        }
        EditorGUILayout.PropertyField(cameraRig);
        EditorGUILayout.PropertyField(headset);
        if (cameraRig.objectReferenceValue && headset.objectReferenceValue)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Optional Settings");
            EditorGUILayout.PropertyField(firstLevel);
            EditorGUILayout.PropertyField(movementLevel);
            EditorGUILayout.PropertyField(guaranteeDestroyOnLoad);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Advanced Settings");
            EditorGUILayout.PropertyField(speedGain);
            EditorGUILayout.PropertyField(worldScale);
            EditorGUILayout.PropertyField(correctPosition);
            EditorGUILayout.PropertyField(correctRotation);
            EditorGUILayout.PropertyField(correctScale);
        }
            
        serializedObject.ApplyModifiedProperties();
    }
}