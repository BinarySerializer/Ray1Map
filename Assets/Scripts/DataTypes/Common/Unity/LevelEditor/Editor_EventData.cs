using System;
using System.Linq;

namespace R1Engine
{
    public class Editor_EventData : EditorWrapper<EventData>
    {
        #region Constructor

        public Editor_EventData(EventData eventData) : base(eventData) { }

        #endregion

        #region Private Fields

        private Enum _type;

        #endregion

        #region Public Properties

        /// <summary>
        /// The event type
        /// </summary>
        public Enum Type
        {
            get => _type;
            set
            {
                _type = value;

                // Update type values
                TypeValue = (ushort)Convert.ChangeType(Type, typeof(ushort));
                TypeInfo = Type.GetAttribute<EventTypeInfoAttribute>();
            }
        }

        /// <summary>
        /// The event type as a ushort
        /// </summary>
        public ushort TypeValue { get; private set; }

        public EventTypeInfoAttribute TypeInfo { get; private set; }

        public string DESKey { get; set; }
        public string ETAKey { get; set; }

        // Rayman 2 only
        public int? MapLayer { get; set; }
        public bool FlipHorizontally { get; set; }

        public ushort[] LabelOffsets { get; set; }
        public Common_EventCommandCollection CommandCollection { get; set; }

        /// <summary>
        /// The link table index
        /// </summary>
        public int LinkIndex { get; set; }

        #endregion

        #region Public Methods

        public bool GetIsFlippedHorizontally()
        {
            // If loading from memory, check runtime flags
            if (Settings.LoadFromMemory)
            {
                if (Data.PC_Flags.HasFlag(EventData.PC_EventFlags.DetectZone))
                    return true;

                // TODO: Check PS1 flags

                return false;
            }

            // Check if we force the event to flip...
            if (FlipHorizontally)
                return true;

            // Check if it's the pin event and if the hp flag is set
            if (Type is EventType et && et == EventType.TYPE_PUNAISE3 && Data.HitPoints == 1)
                return true;

            // If the first command changes its direction to right, flip the event (a bit hacky, but works for trumpets etc.)
            if (CommandCollection?.Commands?.FirstOrDefault()?.Command == EventCommand.GO_RIGHT)
                return true;

            return false;
        }

        public bool GetIsVisible()
        {
            // Get event flag
            var flag = TypeInfo?.Flag ?? EventFlag.Normal;

            // Check runtime flag if loading from memory
            if (Settings.LoadFromMemory)
            {
                // TODO: Check PS1 flags
                // TODO: This flag doesn't always work, for example when you defeat an enemy it stays visible

                return Data.PC_Flags.HasFlag(EventData.PC_EventFlags.SwitchedOn);
            }
            else
            {
                if (flag == EventFlag.Editor)
                    return Settings.ShowEditorEvents;

                if (flag == EventFlag.Always)
                    return Settings.ShowAlwaysEvents;
            }

            // Default to visible
            return true;
        }

        #endregion
    }
}