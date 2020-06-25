using System;
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

        public EventEditorViewModel(BaseEditorManager editorManager)
        {
            // Set properties
            EditorManager = editorManager;
        }

        #endregion

        #region Private Fields

        private Common_Event _selectedEvent;
        private uint? _xPosition;
        private uint? _yPosition;
        private string _des;
        private string _eta;
        private byte? _etat;
        private byte? _subEtat;
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

                if (XPosition.HasValue && SelectedEvent != null && XPosition != SelectedEvent.Data.EventData.XPosition)
                {
                    SelectedEvent.Data.EventData.XPosition = XPosition.Value;
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

                if (YPosition.HasValue && SelectedEvent != null && XPosition != SelectedEvent.Data.EventData.YPosition)
                {
                    SelectedEvent.Data.EventData.YPosition = YPosition.Value;
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
                }

                OnPropertyChanged();
            }
        }

        public byte? Etat
        {
            get => _etat;
            set
            {
                if (Etat == value || (SelectedEvent == null && value != null))
                    return;

                _etat = value;

                if (Etat != null && SelectedEvent != null && Etat != SelectedEvent.Data.EventData.Etat)
                {
                    SelectedEvent.Data.EventData.Etat = Etat.Value;
                    SelectedEvent.Data.EventData.RuntimeEtat = Etat.Value;
                    SelectedEvent.RefreshName();
                }

                OnPropertyChanged();
            }
        }

        public byte? SubEtat
        {
            get => _subEtat;
            set
            {
                if (SubEtat == value || (SelectedEvent == null && value != null))
                    return;

                _subEtat = value;

                if (SubEtat != null && SelectedEvent != null && SubEtat != SelectedEvent.Data.EventData.SubEtat)
                {
                    SelectedEvent.Data.EventData.SubEtat = SubEtat.Value;
                    SelectedEvent.Data.EventData.RuntimeSubEtat = SubEtat.Value;
                    SelectedEvent.RefreshName();
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

                if (OffsetBX.HasValue && SelectedEvent != null && OffsetBX != SelectedEvent.Data.EventData.OffsetBX)
                {
                    SelectedEvent.Data.EventData.OffsetBX = OffsetBX.Value;
                    SelectedEvent.RefreshName();
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

                if (OffsetBY.HasValue && SelectedEvent != null && OffsetBY != SelectedEvent.Data.EventData.OffsetBY)
                {
                    SelectedEvent.Data.EventData.OffsetBY = OffsetBY.Value;
                    SelectedEvent.RefreshName();
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

                if (OffsetHY.HasValue && SelectedEvent != null && OffsetHY != SelectedEvent.Data.EventData.OffsetHY)
                {
                    SelectedEvent.Data.EventData.OffsetHY = OffsetHY.Value;
                    SelectedEvent.RefreshName();
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

                if (FollowSprite.HasValue && SelectedEvent != null && FollowSprite != SelectedEvent.Data.EventData.FollowSprite)
                {
                    SelectedEvent.Data.EventData.FollowSprite = FollowSprite.Value;
                    SelectedEvent.RefreshName();
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

                if (HitPoints.HasValue && SelectedEvent != null && HitPoints != SelectedEvent.Data.EventData.HitPoints)
                {
                    SelectedEvent.Data.EventData.HitPoints = HitPoints.Value;
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

                if (HitSprite.HasValue && SelectedEvent != null && HitSprite != SelectedEvent.Data.EventData.HitSprite)
                {
                    SelectedEvent.Data.EventData.HitSprite = HitSprite.Value;
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

                if (FollowEnabled.HasValue && SelectedEvent != null && FollowEnabled != SelectedEvent.Data.EventData.GetFollowEnabled(Controller.obj.levelController.EditorManager.Settings))
                {
                    SelectedEvent.Data.EventData.SetFollowEnabled(Controller.obj.levelController.EditorManager.Settings, FollowEnabled.Value);
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
            XPosition = SelectedEvent?.Data.EventData.XPosition;
            YPosition = SelectedEvent?.Data.EventData.YPosition;
            DES = SelectedEvent?.Data.DESKey;
            ETA = SelectedEvent?.Data.ETAKey;
            Etat = SelectedEvent?.Data.EventData.Etat;
            SubEtat = SelectedEvent?.Data.EventData.SubEtat;
            OffsetBX = (byte?)SelectedEvent?.Data.EventData.OffsetBX;
            OffsetBY = (byte?)SelectedEvent?.Data.EventData.OffsetBY;
            OffsetHY = (byte?)SelectedEvent?.Data.EventData.OffsetHY;
            FollowSprite = (byte?)SelectedEvent?.Data.EventData.FollowSprite;
            HitPoints = (byte?)SelectedEvent?.Data.EventData.HitPoints;
            HitSprite = (byte?)SelectedEvent?.Data.EventData.HitSprite;
            FollowEnabled = SelectedEvent?.Data.EventData.GetFollowEnabled(Controller.obj.levelController.EditorManager.Settings);
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