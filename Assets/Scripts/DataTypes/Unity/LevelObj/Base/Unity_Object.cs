using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public abstract class Unity_Object
    {
        // Position
        public abstract Vector3 Position { get; set; }

        // Editor
        public virtual string DebugText => String.Empty;
        public bool HasPendingEdits { get; set; }
        public virtual BinarySerializable SerializableData => null;
        public virtual BinarySerializable[] AdditionalSerializableDatas => null;

        public virtual IEnumerable<int> GetLocIndices => new int[0];

        // Attributes
        public virtual Unity_ObjectType Type => Unity_ObjectType.Object;
        public virtual bool IsAlways => false;
        public virtual bool IsEditor => false;
        public virtual bool IsActive => true;
        public virtual int? ObjectGroupIndex => null;

        // Display properties
        public abstract string PrimaryName { get; } // Official
        public virtual string SecondaryName => null; // Unofficial
        public string Name => PrimaryName ?? SecondaryName;
        public virtual bool IsVisible
        {
            get
            {
                if (LevelEditorData.Level.Rayman == this)
                    return Settings.ShowRayman;

                if (IsEditor)
                    return Settings.ShowEditorObjects;

                if (IsAlways)
                    return Settings.ShowAlwaysObjects || (Settings.LoadFromMemory && IsActive);

                // Default to visible
                return true;
            }
        }
        public virtual bool IsDisabled => Settings.LoadFromMemory && !IsActive;

        // Events
        public virtual void OnUpdate() { }

        // UI States 
        public string[] UIStateNames 
        {
            get 
            {
                if (!IsUIStateArrayUpToDate)
                    RecalculateUIStates();

                return UIStates.Select(x => x.DisplayName).ToArray();
            }
        }
        public int CurrentUIState 
        {
            get 
            {
                if (!IsUIStateArrayUpToDate)
                    RecalculateUIStates();

                var i = UIStates.FindItemIndex(x => x.IsCurrentState(this));

                return i == -1 ? 0 : i;
            }
            set 
            {
                if (value == CurrentUIState || UIStates == null || value >= UIStates.Length || value < 0)
                    return;

                UIStates[value]?.Apply(this);
            }
        }
        public int? OverrideAnimIndex { get; set; }
        protected abstract bool IsUIStateArrayUpToDate { get; }
        protected UIState[] UIStates { get; set; }
        protected abstract void RecalculateUIStates();

        protected abstract class UIState 
        {
            protected UIState(string displayName) 
            {
                DisplayName = displayName;
                IsState = true;
            }
            protected UIState(string displayName, int animIndex) 
            {
                DisplayName = displayName;
                IsState = false;
                AnimIndex = animIndex;
            }

            public string DisplayName { get; protected set; }
            public bool IsState { get; protected set; }
            public int AnimIndex { get; protected set; }

            public abstract void Apply(Unity_Object obj);
            public abstract bool IsCurrentState(Unity_Object obj);
        }
    }
}