using HMUI;
using CustomNotes.Data;
using CustomNotes.Utilities;
using CustomNotes.Settings.Utilities;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using Zenject;
using CustomNotes.Settings.UI;
using BeatSaberMarkupLanguage.Components.Settings;

namespace CustomNotes.Settings
{
    internal class NoteDetailsViewController : BSMLResourceViewController
    {
        public override string ResourceName => "CustomNotes.Settings.UI.Views.noteDetails.bsml";

        private PluginConfig _pluginConfig;
        private NoteListViewController _listViewController;

        [UIComponent("note-description")]
        public TextPageScrollView noteDescription = null;

        [UIComponent("size-slider")]
        private SliderSetting sizeSlider = null;

        [UIComponent("hmd-checkbox")]
        private ToggleSetting hmdCheckbox = null;

        [UIComponent("note-trail-checkbox")]
        private ToggleSetting noteTrailCheckbox = null;

        [UIComponent("colored-note-trail-checkbox")]
        private ToggleSetting coloredNoteTrailCheckbox = null;

        [UIComponent("note-trail-width")]
        private IncrementSetting noteTrailWidth = null;

        [UIComponent("note-trail-length")]
        private IncrementSetting noteTrailLength = null;

        public void OnNoteWasChanged(CustomNote customNote)
        {
            if (string.IsNullOrWhiteSpace(customNote.ErrorMessage))
            {
                noteDescription.SetText($"{customNote.Descriptor.NoteName}:\n\n{Utils.SafeUnescape(customNote.Descriptor.Description)}");
            }
            else
            {
                noteDescription.SetText(string.Empty);
            }

            if (sizeSlider != null)
            {
                sizeSlider.ReceiveValue();
            }
            if (hmdCheckbox != null)
            {
                hmdCheckbox.ReceiveValue();
            }
            if (noteTrailCheckbox != null)
            {
                noteTrailCheckbox.ReceiveValue();
            }
            if (coloredNoteTrailCheckbox != null)
            {
                coloredNoteTrailCheckbox.ReceiveValue();
            }
            if (noteTrailWidth != null)
            {
                noteTrailWidth.ReceiveValue();
            }
            if (noteTrailLength != null)
            {
                noteTrailLength.ReceiveValue();
            }
        }

        [Inject]
        public void Construct(PluginConfig pluginConfig, NoteListViewController listViewController)
        {
            _pluginConfig = pluginConfig;
            _listViewController = listViewController;
        }

        [UIValue("note-size")]
        public float noteSize
        {
            get { return _pluginConfig.NoteSize; }
            set 
            { 
                _pluginConfig.NoteSize = value;
                _listViewController.ScalePreviewNotes(value);
            }
        }

        [UIValue("hmd-only")]
        public bool hmdOnly 
        {
            get { return _pluginConfig.HMDOnly; }
            set 
            { _pluginConfig.HMDOnly = value; }
        }
        [UIValue("note-trail")]
        public bool noteTrail
        {
            get { return _pluginConfig.NoteTrail; }
            set { _pluginConfig.NoteTrail = value; }
        }
        [UIValue("colored-trail")]
        public bool coloredTrail
        {
            get { return _pluginConfig.ColoredTrail; }
            set { _pluginConfig.ColoredTrail = value; }
        }
        [UIValue("trail-width")]
        public float trailWidth
        {
            get { return _pluginConfig.TrailWidth; }
            set { _pluginConfig.TrailWidth = value; }
        }
        [UIValue("trail-length")]
        public float trailTime
        {
            get { return _pluginConfig.TrailLength; }
            set { _pluginConfig.TrailLength = value; }
        }
    }
}
