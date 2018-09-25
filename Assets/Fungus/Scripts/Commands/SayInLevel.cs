// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Writes text in a dialog box.
    /// </summary>
    [CommandInfo("Narrative",
                 "SayInLevel",
                 "Writes text to world.")]
    [AddComponentMenu("")]
    public class SayInLevel : Command
    {
        // Removed this tooltip as users's reported it obscures the text box
        [UnityCommon.GameTextIdAutoComplete]
        [SerializeField] protected string storyText = "";

        [SerializeField] protected Transform aboveTransform;

        [Tooltip("Notes about this story text for other authors, localization, etc.")]
        [SerializeField] protected string description = "";

        [Tooltip("Character that is speaking")]
        [SerializeField] protected Character character;

        [Tooltip("Portrait that represents speaking character")]
        [SerializeField] protected Sprite portrait;

        [Tooltip("Voiceover audio to play when writing the text")]
        [SerializeField] protected AudioClip voiceOverClip;

        [Tooltip("Always show this Say text when the command is executed multiple times")]
        [SerializeField] protected bool showAlways = true;

        [Tooltip("Number of times to show this Say text when the command is executed multiple times")]
        [SerializeField] protected int showCount = 1;

        [Tooltip("Type this text in the previous dialog box.")]
        [SerializeField] protected bool extendPrevious = false;

        [Tooltip("Fade out the dialog box when writing has finished and not waiting for input.")]
        [SerializeField] protected bool fadeWhenDone = true;

        [Tooltip("Wait for player to click before continuing.")]
        [SerializeField] protected bool waitForClick = true;

        [Tooltip("Stop playing voiceover when text finishes writing.")]
        [SerializeField] protected bool stopVoiceover = true;

        [Tooltip("Wait for the Voice Over to complete before continuing")]
        [SerializeField] protected bool waitForDuration = true;

        [Tooltip("Wait for the Voice Over to complete before continuing")]
        [SerializeField] protected bool waitForVO = false;
        [SerializeField] protected UINarrativeManager.Characters helchar = UINarrativeManager.Characters.Helena;


        //add wait for vo that overrides stopvo

        [Tooltip("Sets the active Say dialog with a reference to a Say Dialog object in the scene. All story text will now display using this Say Dialog.")]
        [SerializeField] protected SayDialog setSayDialog;

        [Tooltip("Duration to wait for")]
        [SerializeField] protected FloatData _duration = new FloatData(1);

        [SerializeField] protected bool repair_dontdisplay = false;




        protected int executionCount;

        #region Public members

        /// <summary>
        /// Character that is speaking.
        /// </summary>
        public virtual Character _Character { get { return character; } }

        /// <summary>
        /// Portrait that represents speaking character.
        /// </summary>
        public virtual Sprite Portrait { get { return portrait; } set { portrait = value; } }

        /// <summary>
        /// Type this text in the previous dialog box.
        /// </summary>
        public virtual bool ExtendPrevious { get { return extendPrevious; } }

        public override void OnEnter()
        {
            waitForDuration = true;

            if (!showAlways && executionCount >= showCount)
            {
                Continue();
                return;
            }

            executionCount++;

            UINarrativeManager inGamePrompt = GameObject.FindObjectOfType<UINarrativeManager>();
            SayDialog sayDialog = inGamePrompt.ShowPromptMessage(helchar);

            if (sayDialog == null)
            {
                Continue();
                return;
            }

            if (waitForDuration)
            {
                bool useVOTime = waitForVO && voiceOverClip != null;
                float duration = useVOTime ? Mathf.Max(_duration.Value, voiceOverClip.length) : _duration.Value;
                D.Log("NAR: Playing For : ", duration, "s", "Use VO time : ", useVOTime);
                Invoke("OnWaitComplete", duration);
            }

            var flowchart = GetFlowchart();

            sayDialog.SetActive(true);

            string displayText = UnityCommon.GameTextService.Instance.GetText(storyText);

            var activeCustomTags = CustomTag.activeCustomTags;
            for (int i = 0; i < activeCustomTags.Count; i++)
            {
                var ct = activeCustomTags[i];
                displayText = displayText.Replace(ct.TagStartSymbol, ct.ReplaceTagStartWith);
                if (ct.TagEndSymbol != "" && ct.ReplaceTagEndWith != "")
                {
                    displayText = displayText.Replace(ct.TagEndSymbol, ct.ReplaceTagEndWith);
                }
            }

            string subbedText = flowchart.SubstituteVariables(displayText);

            if (waitForDuration)
            {
                sayDialog.Say(subbedText, !extendPrevious, waitForClick, fadeWhenDone, stopVoiceover, waitForVO, voiceOverClip, null, forceNoUI: true);
            }
            else
            {
                sayDialog.Say(subbedText, !extendPrevious, waitForClick, fadeWhenDone, stopVoiceover, waitForVO, voiceOverClip, delegate
                {
                    Continue();
                }, forceNoUI: true);
            }
        }

        public override string GetSummary()
        {
            string namePrefix = "";
            if (character != null)
            {
                namePrefix = character.NameText + ": ";
            }
            if (extendPrevious)
            {
                namePrefix = "EXTEND" + ": ";
            }
#if UNITY_EDITOR
            return namePrefix + "\"" + UnityCommon.GameTextService.GetTextForEditor(storyText) + "\"";
#else
        return "";
#endif
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

        public override void OnReset()
        {
            executionCount = 0;
        }

        public override void OnStopExecuting()
        {
            var sayDialog = SayDialog.GetSayDialog();
            if (sayDialog == null)
            {
                return;
            }

            sayDialog.Stop();
        }

        protected virtual void OnWaitComplete()
        {
            var sayDialog = SayDialog.GetSayDialog();
            if (sayDialog == null)
            {
                return;
            }

            sayDialog.Stop();
            sayDialog.FadeWhenDone = true;
            Continue();
        }

        #endregion

        #region ILocalizable implementation

        public virtual string GetStandardText()
        {
            return UnityCommon.GameTextService.Instance.GetText(storyText);
        }


        public virtual string GetTextID()
        {
            return storyText;
        }

        public virtual void SetStandardText(string standardText)
        {
            storyText = standardText;
        }

        public virtual string GetDescription()
        {
            return description;
        }

        public virtual void SetAudioClip(AudioClip clip)
        {
            voiceOverClip = clip;
        }

        public virtual string GetStringId()
        {
            // String id for Say commands is SAY.<Localization Id>.<Command id>.[Character Name]
            string stringId = "SAY." + GetFlowchartLocalizationId() + "." + itemId + ".";
            if (character != null)
            {
                stringId += character.NameText;
            }

            return stringId;
        }

        #endregion
    }
}