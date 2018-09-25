// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Fungus.EditorUtils
{
    [CustomEditor(typeof(SayInLevel))]
    public class SayInLevelEditor : CommandEditor
    {
        public static bool showTagHelp;
        public Texture2D blackTex;

        public static void DrawTagHelpLabel()
        {
            string tagsText = TextTagParser.GetTagHelp();

            if (CustomTag.activeCustomTags.Count > 0)
            {
                tagsText += "\n\n\t-------- CUSTOM TAGS --------";
                List<Transform> activeCustomTagGroup = new List<Transform>();
                foreach (CustomTag ct in CustomTag.activeCustomTags)
                {
                    if (ct.transform.parent != null)
                    {
                        if (!activeCustomTagGroup.Contains(ct.transform.parent.transform))
                        {
                            activeCustomTagGroup.Add(ct.transform.parent.transform);
                        }
                    }
                    else
                    {
                        activeCustomTagGroup.Add(ct.transform);
                    }
                }
                foreach (Transform parent in activeCustomTagGroup)
                {
                    string tagName = parent.name;
                    string tagStartSymbol = "";
                    string tagEndSymbol = "";
                    CustomTag parentTag = parent.GetComponent<CustomTag>();
                    if (parentTag != null)
                    {
                        tagName = parentTag.name;
                        tagStartSymbol = parentTag.TagStartSymbol;
                        tagEndSymbol = parentTag.TagEndSymbol;
                    }
                    tagsText += "\n\n\t" + tagStartSymbol + " " + tagName + " " + tagEndSymbol;
                    foreach (Transform child in parent)
                    {
                        tagName = child.name;
                        tagStartSymbol = "";
                        tagEndSymbol = "";
                        CustomTag childTag = child.GetComponent<CustomTag>();
                        if (childTag != null)
                        {
                            tagName = childTag.name;
                            tagStartSymbol = childTag.TagStartSymbol;
                            tagEndSymbol = childTag.TagEndSymbol;
                        }
                        tagsText += "\n\t      " + tagStartSymbol + " " + tagName + " " + tagEndSymbol;
                    }
                }
            }
            tagsText += "\n";
            float pixelHeight = EditorStyles.miniLabel.CalcHeight(new GUIContent(tagsText), EditorGUIUtility.currentViewWidth);
            EditorGUILayout.SelectableLabel(tagsText, GUI.skin.GetStyle("HelpBox"), GUILayout.MinHeight(pixelHeight));
        }

        protected SerializedProperty characterProp;
        protected SerializedProperty portraitProp;
        protected SerializedProperty storyTextProp;
        protected SerializedProperty descriptionProp;
        protected SerializedProperty voiceOverClipProp;
        protected SerializedProperty showAlwaysProp;
        protected SerializedProperty showCountProp;
        protected SerializedProperty extendPreviousProp;
        protected SerializedProperty fadeWhenDoneProp;
        protected SerializedProperty waitForClickProp;
        protected SerializedProperty stopVoiceoverProp;
        protected SerializedProperty setSayDialogProp;
        protected SerializedProperty waitForVOProp;
        protected SerializedProperty waitForDurationProp;
        protected SerializedProperty durationProp;
        protected SerializedProperty repair_dontDisplay;
        protected SerializedProperty aboveTransformProp;
        protected SerializedProperty helCharProp;


        protected virtual void OnEnable()
        {
            if (NullTargetCheck()) // Check for an orphaned editor instance
                return;

            characterProp = serializedObject.FindProperty("character");
            portraitProp = serializedObject.FindProperty("portrait");
            storyTextProp = serializedObject.FindProperty("storyText");
            descriptionProp = serializedObject.FindProperty("description");
            voiceOverClipProp = serializedObject.FindProperty("voiceOverClip");
            showAlwaysProp = serializedObject.FindProperty("showAlways");
            showCountProp = serializedObject.FindProperty("showCount");
            extendPreviousProp = serializedObject.FindProperty("extendPrevious");
            fadeWhenDoneProp = serializedObject.FindProperty("fadeWhenDone");
            waitForClickProp = serializedObject.FindProperty("waitForClick");
            stopVoiceoverProp = serializedObject.FindProperty("stopVoiceover");
            setSayDialogProp = serializedObject.FindProperty("setSayDialog");
            waitForVOProp = serializedObject.FindProperty("waitForVO");
            waitForDurationProp = serializedObject.FindProperty("waitForDuration");
            durationProp = serializedObject.FindProperty("_duration");
            repair_dontDisplay = serializedObject.FindProperty("repair_dontdisplay");
            aboveTransformProp = serializedObject.FindProperty("aboveTransform");
            helCharProp = serializedObject.FindProperty("helchar");


            if (blackTex == null)
            {
                blackTex = CustomGUI.CreateBlackTexture();
            }
        }

        protected virtual void OnDisable()
        {
            DestroyImmediate(blackTex);
        }

        public override void DrawCommandGUI()
        {
            serializedObject.Update();


            EditorGUILayout.PropertyField(storyTextProp);

            GUI.enabled = false;
            EditorGUILayout.TextArea(UnityCommon.GameTextService.GetTextForEditor(storyTextProp.stringValue));
            GUI.enabled = true;

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(voiceOverClipProp,
                                          new GUIContent("Voice Over Clip", "Voice over audio to play when the text is displayed"));

            EditorGUILayout.PropertyField(helCharProp,
                                          new GUIContent("Character"));

            /*EditorGUILayout.PropertyField(showAlwaysProp);

            if (showAlwaysProp.boolValue == false)
            {
                EditorGUILayout.PropertyField(showCountProp);
            }*/

            GUIStyle centeredLabel = new GUIStyle(EditorStyles.label);
            centeredLabel.alignment = TextAnchor.MiddleCenter;
            GUIStyle leftButton = new GUIStyle(EditorStyles.miniButtonLeft);
            leftButton.fontSize = 10;
            leftButton.font = EditorStyles.toolbarButton.font;
            GUIStyle rightButton = new GUIStyle(EditorStyles.miniButtonRight);
            rightButton.fontSize = 10;
            rightButton.font = EditorStyles.toolbarButton.font;

            //  EditorGUILayout.PropertyField(fadeWhenDoneProp);
            // EditorGUILayout.PropertyField(waitForClickProp);
            // EditorGUILayout.PropertyField(stopVoiceoverProp);
            // EditorGUILayout.PropertyField(setSayDialogProp);
            //EditorGUILayout.PropertyField(waitForVOProp);
            //EditorGUILayout.PropertyField(waitForDurationProp);
            //EditorGUILayout.PropertyField(durationProp);
            // EditorGUILayout.PropertyField(repair_dontDisplay);

            serializedObject.ApplyModifiedProperties();
        }
    }
}