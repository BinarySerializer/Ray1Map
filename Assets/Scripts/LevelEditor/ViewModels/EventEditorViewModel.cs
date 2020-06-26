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

                if (XPosition.HasValue && SelectedEvent != null && XPosition != SelectedEvent.Data.Data.XPosition)
                {
                    SelectedEvent.Data.Data.XPosition = XPosition.Value;
                    SelectedEvent.Data.HasPendingEdits = true;
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

                if (YPosition.HasValue && SelectedEvent != null && YPosition != SelectedEvent.Data.Data.YPosition)
                {
                    SelectedEvent.Data.Data.YPosition = YPosition.Value;
                    SelectedEvent.Data.HasPendingEdits = true;
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
                    SelectedEvent.Data.HasPendingEdits = true;
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
                    SelectedEvent.Data.HasPendingEdits = true;
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

                if (Etat != null && SelectedEvent != null && Etat != SelectedEvent.Data.Data.Etat)
                {
                    SelectedEvent.Data.Data.Etat = Etat.Value;
                    SelectedEvent.Data.Data.RuntimeEtat = Etat.Value;
                    SelectedEvent.RefreshName();
                    SelectedEvent.Data.HasPendingEdits = true;
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

                if (SubEtat != null && SelectedEvent != null && SubEtat != SelectedEvent.Data.Data.SubEtat)
                {
                    SelectedEvent.Data.Data.SubEtat = SubEtat.Value;
                    SelectedEvent.Data.Data.RuntimeSubEtat = SubEtat.Value;
                    SelectedEvent.RefreshName();
                    SelectedEvent.Data.HasPendingEdits = true;
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

                if (OffsetBX.HasValue && SelectedEvent != null && OffsetBX != SelectedEvent.Data.Data.OffsetBX)
                {
                    SelectedEvent.Data.Data.OffsetBX = OffsetBX.Value;
                    SelectedEvent.RefreshName();
                    SelectedEvent.Data.HasPendingEdits = true;
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

                if (OffsetBY.HasValue && SelectedEvent != null && OffsetBY != SelectedEvent.Data.Data.OffsetBY)
                {
                    SelectedEvent.Data.Data.OffsetBY = OffsetBY.Value;
                    SelectedEvent.RefreshName();
                    SelectedEvent.Data.HasPendingEdits = true;
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

                if (OffsetHY.HasValue && SelectedEvent != null && OffsetHY != SelectedEvent.Data.Data.OffsetHY)
                {
                    SelectedEvent.Data.Data.OffsetHY = OffsetHY.Value;
                    SelectedEvent.RefreshName();
                    SelectedEvent.Data.HasPendingEdits = true;
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

                if (FollowSprite.HasValue && SelectedEvent != null && FollowSprite != SelectedEvent.Data.Data.FollowSprite)
                {
                    SelectedEvent.Data.Data.FollowSprite = FollowSprite.Value;
                    SelectedEvent.RefreshName();
                    SelectedEvent.Data.HasPendingEdits = true;
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

                if (HitPoints.HasValue && SelectedEvent != null && HitPoints != SelectedEvent.Data.Data.HitPoints)
                {
                    SelectedEvent.Data.Data.HitPoints = HitPoints.Value;
                    SelectedEvent.Data.Data.RuntimeHitPoints = HitPoints.Value;
                    SelectedEvent.RefreshName();
                    SelectedEvent.Data.HasPendingEdits = true;
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

                if (HitSprite.HasValue && SelectedEvent != null && HitSprite != SelectedEvent.Data.Data.HitSprite)
                {
                    SelectedEvent.Data.Data.HitSprite = HitSprite.Value;
                    SelectedEvent.RefreshName();
                    SelectedEvent.Data.HasPendingEdits = true;
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

                if (FollowEnabled.HasValue && SelectedEvent != null && FollowEnabled != SelectedEvent.Data.Data.GetFollowEnabled(Controller.obj.levelController.EditorManager.Settings))
                {
                    SelectedEvent.Data.Data.SetFollowEnabled(Controller.obj.levelController.EditorManager.Settings, FollowEnabled.Value);
                    SelectedEvent.RefreshName();
                    SelectedEvent.ChangeOffsetVisibility(true);
                    SelectedEvent.Data.HasPendingEdits = true;
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
                    SelectedEvent.Data.HasPendingEdits = true;
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
            XPosition = SelectedEvent?.Data.Data.XPosition;
            YPosition = SelectedEvent?.Data.Data.YPosition;
            DES = SelectedEvent?.Data.DESKey;
            ETA = SelectedEvent?.Data.ETAKey;
            Etat = SelectedEvent?.Data.Data.Etat;
            SubEtat = SelectedEvent?.Data.Data.SubEtat;
            OffsetBX = (byte?)SelectedEvent?.Data.Data.OffsetBX;
            OffsetBY = (byte?)SelectedEvent?.Data.Data.OffsetBY;
            OffsetHY = (byte?)SelectedEvent?.Data.Data.OffsetHY;
            FollowSprite = (byte?)SelectedEvent?.Data.Data.FollowSprite;
            HitPoints = (byte?)SelectedEvent?.Data.Data.HitPoints;
            HitSprite = (byte?)SelectedEvent?.Data.Data.HitSprite;
            FollowEnabled = SelectedEvent?.Data.Data.GetFollowEnabled(Controller.obj.levelController.EditorManager.Settings);
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