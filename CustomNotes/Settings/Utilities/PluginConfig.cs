namespace CustomNotes.Settings.Utilities
{
    public class PluginConfig
    {
        public virtual string LastNote { get; set; }

        public virtual float NoteSize { get; set; } = 1;

        public virtual bool HMDOnly { get; set; } = false;
        public virtual bool NoteTrail { get; set; } = true;
        public virtual bool ColoredTrail { get; set; } = true;
        public virtual float TrailWidth { get; set; } = 0.03f;
        public virtual float TrailLength { get; set; } = 3.5f;
    }
}