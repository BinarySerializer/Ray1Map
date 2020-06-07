using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace R1Engine
{
    /// <summary>
    /// View model for the event editor
    /// </summary>
    public class EventEditorViewModel : INotifyPropertyChanged
    {
        #region Constructor

        public EventEditorViewModel(IReadOnlyList<Common_Event> events, BaseEditorManager editorManager)
        {
            // Set properties
            Events = events;
            EditorManager = editorManager;
        }

        #endregion

        #region Private Fields

        private Common_Event _selectedEvent;
        private uint? _xPosition;
        private uint? _yPosition;
        private string _des;
        private string _eta;
        private int? _etat;
        private int? _subEtat;
        private byte? _offsetBx;
        private byte? _offsetBy;
        private byte? _offsetHy;
        private byte? _followSprite;
        private byte? _hitPoints;
        private byte? _hitSprite;
        private bool? _followEnabled;
        private Enum _type;

        #endregion

        #region Public Properties

        /// <summary>
        /// The available events
        /// </summary>
        public IReadOnlyList<Common_Event> Events { get; }

        public BaseEditorManager EditorManager { get; }

        /// <summary>
        /// The currently selected event
        /// </summary>
        public Common_Event SelectedEvent
        {
            get => _selectedEvent;
            set
            {
                // Reset values if the selected event is being changed without it first being set to null
                if (value != null && _selectedEvent != null)
                {
                    _selectedEvent = null;
                    Refresh();
                }

                _selectedEvent = value;
                Refresh();
            }
        }

        public string DisplayName => SelectedEvent?.DisplayName ?? String.Empty;

        public uint? XPosition
        {
            get => _xPosition;
            set
            {
                if (XPosition == value || (SelectedEvent == null && value != null))
                    return;

                _xPosition = value;

                if (XPosition.HasValue && SelectedEvent != null && XPosition != SelectedEvent.Data.XPosition)
                {
                    SelectedEvent.Data.XPosition = XPosition.Value;
                    SelectedEvent.UpdateXAndY();
                }

                OnPropertyChanged();
            }
        }

        public uint? YPosition
        {
            get => _yPosition;
            set
            {
                if (YPosition == value || (SelectedEvent == null && value != null))
                    return;

                _yPosition = value;

                if (YPosition.HasValue && SelectedEvent != null && XPosition != SelectedEvent.Data.YPosition)
                {
                    SelectedEvent.Data.YPosition = YPosition.Value;
                    SelectedEvent.UpdateXAndY();
                }

                OnPropertyChanged();
            }
        }

        public string DES
        {
            get => _des;
            set
            {
                if (DES == value || (SelectedEvent == null && value != null))
                    return;

                _des = value;

                if (DES != null && SelectedEvent != null && DES != SelectedEvent.Data.DESKey)
                {
                    SelectedEvent.Data.DESKey = DES;
                    SelectedEvent.RefreshName();
                    SelectedEvent.RefreshVisuals();
                }

                OnPropertyChanged();
            }
        }

        public string ETA
        {
            get => _eta;
            set
            {
                if (ETA == value || (SelectedEvent == null && value != null))
                    return;

                _eta = value;

                if (ETA != null && SelectedEvent != null && ETA != SelectedEvent.Data.ETAKey)
                {
                    SelectedEvent.Data.ETAKey = ETA;
                    SelectedEvent.RefreshName();
                    SelectedEvent.RefreshVisuals();
                }

                OnPropertyChanged();
            }
        }

        public int? Etat
        {
            get => _etat;
            set
            {
                if (Etat == value || (SelectedEvent == null && value != null))
                    return;

                _etat = value;

                if (Etat != null && SelectedEvent != null && Etat != SelectedEvent.Data.Etat)
                {
                    SelectedEvent.Data.Etat = Etat.Value;
                    SelectedEvent.RefreshName();
                    SelectedEvent.RefreshVisuals();
                }

                OnPropertyChanged();
            }
        }

        public int? SubEtat
        {
            get => _subEtat;
            set
            {
                if (SubEtat == value || (SelectedEvent == null && value != null))
                    return;

                _subEtat = value;

                if (SubEtat != null && SelectedEvent != null && SubEtat != SelectedEvent.Data.SubEtat)
                {
                    SelectedEvent.Data.SubEtat = SubEtat.Value;
                    SelectedEvent.RefreshName();
                    SelectedEvent.RefreshVisuals();
                }

                OnPropertyChanged();
            }
        }

        public byte? OffsetBX
        {
            get => _offsetBx;
            set
            {
                if (OffsetBX == value || (SelectedEvent == null && value != null))
                    return;

                _offsetBx = value;

                if (OffsetBX.HasValue && SelectedEvent != null && OffsetBX != SelectedEvent.Data.OffsetBX)
                {
                    SelectedEvent.Data.OffsetBX = OffsetBX.Value;
                    SelectedEvent.RefreshName();
                    SelectedEvent.UpdateOffsetPoints();
                }

                OnPropertyChanged();
            }
        }

        public byte? OffsetBY
        {
            get => _offsetBy;
            set
            {
                if (OffsetBY == value || (SelectedEvent == null && value != null))
                    return;

                _offsetBy = value;

                if (OffsetBY.HasValue && SelectedEvent != null && OffsetBY != SelectedEvent.Data.OffsetBY)
                {
                    SelectedEvent.Data.OffsetBY = OffsetBY.Value;
                    SelectedEvent.RefreshName();
                    SelectedEvent.UpdateOffsetPoints();
                }

                OnPropertyChanged();
            }
        }

        public byte? OffsetHY
        {
            get => _offsetHy;
            set
            {
                if (OffsetHY == value || (SelectedEvent == null && value != null))
                    return;

                _offsetHy = value;

                if (OffsetHY.HasValue && SelectedEvent != null && OffsetHY != SelectedEvent.Data.OffsetHY)
                {
                    SelectedEvent.Data.OffsetHY = OffsetHY.Value;
                    SelectedEvent.RefreshName();
                    SelectedEvent.UpdateOffsetPoints();
                    SelectedEvent.UpdateFollowSpriteLine();
                }

                OnPropertyChanged();
            }
        }

        public byte? FollowSprite
        {
            get => _followSprite;
            set
            {
                if (FollowSprite == value || (SelectedEvent == null && value != null))
                    return;

                _followSprite = value;

                if (FollowSprite.HasValue && SelectedEvent != null && FollowSprite != SelectedEvent.Data.FollowSprite)
                {
                    SelectedEvent.Data.FollowSprite = FollowSprite.Value;
                    SelectedEvent.RefreshName();
                    SelectedEvent.UpdateFollowSpriteLine();
                }

                OnPropertyChanged();
            }
        }

        public byte? HitPoints
        {
            get => _hitPoints;
            set
            {
                if (HitPoints == value || (SelectedEvent == null && value != null))
                    return;

                _hitPoints = value;

                if (HitPoints.HasValue && SelectedEvent != null && HitPoints != SelectedEvent.Data.HitPoints)
                {
                    SelectedEvent.Data.HitPoints = HitPoints.Value;
                    SelectedEvent.RefreshName();
                }

                OnPropertyChanged();
            }
        }

        public byte? HitSprite
        {
            get => _hitSprite;
            set
            {
                if (HitSprite == value || (SelectedEvent == null && value != null))
                    return;

                _hitSprite = value;

                if (HitSprite.HasValue && SelectedEvent != null && HitSprite != SelectedEvent.Data.HitSprite)
                {
                    SelectedEvent.Data.HitSprite = HitSprite.Value;
                    SelectedEvent.RefreshName();
                }

                OnPropertyChanged();
            }
        }

        public bool? FollowEnabled
        {
            get => _followEnabled;
            set
            {
                if (FollowEnabled == value || (SelectedEvent == null && value != null))
                    return;

                _followEnabled = value;

                if (FollowEnabled.HasValue && SelectedEvent != null && FollowEnabled != SelectedEvent.Data.FollowEnabled)
                {
                    SelectedEvent.Data.FollowEnabled = FollowEnabled.Value;
                    SelectedEvent.RefreshName();
                    SelectedEvent.ChangeOffsetVisibility(true);
                }

                OnPropertyChanged();
            }
        }

        public Enum Type
        {
            get => _type;
            set
            {
                if (Type == value || (SelectedEvent == null && value != null))
                    return;

                _type = value;

                if (Type != null && SelectedEvent != null && !Equals(Type, SelectedEvent.Data.Type))
                {
                    SelectedEvent.Data.Type = Type;
                    SelectedEvent.RefreshFlag();
                    SelectedEvent.RefreshName();
                }

                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Refreshes all event properties
        /// </summary>
        protected void Refresh()
        {
            XPosition = SelectedEvent?.Data.XPosition;
            YPosition = SelectedEvent?.Data.YPosition;
            DES = SelectedEvent?.Data.DESKey;
            ETA = SelectedEvent?.Data.ETAKey;
            Etat = SelectedEvent?.Data.Etat;
            SubEtat = SelectedEvent?.Data.SubEtat;
            OffsetBX = (byte?)SelectedEvent?.Data.OffsetBX;
            OffsetBY = (byte?)SelectedEvent?.Data.OffsetBY;
            OffsetHY = (byte?)SelectedEvent?.Data.OffsetHY;
            FollowSprite = (byte?)SelectedEvent?.Data.FollowSprite;
            HitPoints = (byte?)SelectedEvent?.Data.HitPoints;
            HitSprite = (byte?)SelectedEvent?.Data.HitSprite;
            FollowEnabled = SelectedEvent?.Data.FollowEnabled;
            Type = SelectedEvent?.Data.Type;
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}