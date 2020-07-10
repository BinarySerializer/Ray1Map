using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// A base editor manager
    /// </summary>
    public abstract class BaseEditorManager
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="level">The common level</param>
        /// <param name="context">The context</param>
        /// <param name="des">The event designs</param>
        /// <param name="eta">The event states</param>
        protected BaseEditorManager(Common_Lev level, Context context, IReadOnlyDictionary<string, Common_Design> des, IReadOnlyDictionary<string, Common_EventState[][]> eta)
        {
            // Set properties
            Level = level;
            Context = context;
            DES = des;
            ETA = eta;

            // Helper method for getting the names from an event type enum
            string[] getEventTypeNames()
            {
                // Get the values
                var values = Enum.GetValues(EventTypeEnumType);

                // Get the max value
                var max = values.Cast<ushort>().Max();

                // Create the array
                var names = new string[max + 1];

                // Add every value
                for (int i = 0; i < names.Length; i++)
                    names[i] = Enum.GetName(EventTypeEnumType, i) ?? $"Type_{i}";

                return names;
            }

            // TODO: We should limit the types further (such as for Designer, EDU etc.). We could do this by tagging each event enum value with the platforms it's available on, or have a max value for each platform.
            // Set the available event types
            EventTypes = getEventTypeNames();

            // Load the event info data
            using (var csvFile = File.OpenRead("Events.csv"))
                EventInfoData = GeneralEventInfoData.ReadCSV(csvFile).Where(IsAvailableInWorld).ToArray();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates if the game has 3 palettes it swaps between
        /// </summary>
        public virtual bool Has3Palettes => false;

        /// <summary>
        /// The loaded event info
        /// </summary>
        protected GeneralEventInfoData[] EventInfoData { get; }

        /// <summary>
        /// Indicates if the local commands should be used
        /// </summary>
        protected abstract bool UsesLocalCommands { get; }

        /// <summary>
        /// The type of enum for the event types
        /// </summary>
        public Type EventTypeEnumType => Settings.Game == Game.Rayman2 ? typeof(PS1_R2Demo_EventType) : typeof(EventType);

        /// <summary>
        /// The event designs
        /// </summary>
        public IReadOnlyDictionary<string, Common_Design> DES { get; }

        /// <summary>
        /// The event states
        /// </summary>
        public IReadOnlyDictionary<string, Common_EventState[][]> ETA { get; }

        /// <summary>
        /// The available event type names
        /// </summary>
        public string[] EventTypes { get; }

        /// <summary>
        /// The common level
        /// </summary>
        public Common_Lev Level { get; }

        /// <summary>
        /// The context
        /// </summary>
        public Context Context { get; }

        /// <summary>
        /// The game settings
        /// </summary>
        public GameSettings Settings => Context.Settings;

        /// <summary>
        /// Gets the available event names to add for the current world
        /// </summary>
        /// <returns>The names of the available events to add</returns>
        public string[] GetEvents() => EventInfoData.Select(x => x.Name).ToArray();

        #endregion

        #region Methods

        /// <summary>
        /// Adds a new event to the controller and returns it
        /// </summary>
        /// <param name="index">The event index from the available events</param>
        /// <param name="xPos">The x position</param>
        /// <param name="yPos">The y position</param>
        /// <returns>The event</returns>
        public Editor_EventData AddEvent(int index, short xPos, short yPos)
        {
            // Get the event
            var e = EventInfoData[index];

            // Get the commands and label offsets
            Common_EventCommandCollection cmds;
            ushort[] labelOffsets;

            // If local (non-compiled) commands are used, attempt to get them from the event info or decompile the compiled ones
            if (UsesLocalCommands)
            {
                cmds = e.LocalCommands.Any()
                    ? Common_EventCommandCollection.FromBytes(e.LocalCommands, Settings)
                    : EventCommandCompiler.Decompile(new EventCommandCompiler.CompiledEventCommandData(Common_EventCommandCollection.FromBytes(e.Commands, Settings), e.LabelOffsets), e.Commands);

                // Local commands don't use label offsets
                labelOffsets = new ushort[0];
            }
            else
            {
                if (e.Commands.Any())
                {
                    cmds = Common_EventCommandCollection.FromBytes(e.Commands, Settings);
                    labelOffsets = e.LabelOffsets;
                }
                else
                {
                    var cmdData = EventCommandCompiler.Compile(Common_EventCommandCollection.FromBytes(e.LocalCommands, Settings), e.LocalCommands);
                    cmds = cmdData.Commands;
                    labelOffsets = cmdData.LabelOffsets;
                }
            }

            var eventData = new Editor_EventData(new EventData()
            {
                Etat = (byte)e.Etat,
                SubEtat = (byte)e.SubEtat,
                XPosition = xPos,
                YPosition = yPos,
                OffsetBX = (byte)e.OffsetBX,
                OffsetBY = (byte)e.OffsetBY,
                OffsetHY = (byte)e.OffsetHY,
                FollowSprite = (byte)e.FollowSprite,
                HitPoints = (byte)e.HitPoints,
                Layer = 0,
                HitSprite = (byte)e.HitSprite,
            })
            {
                Type = (Enum)Enum.Parse(EventTypeEnumType, e.Type.ToString()),
                DESKey = GetDesKey(e),
                ETAKey = GetEtaKey(e),
                LabelOffsets = labelOffsets,
                CommandCollection = cmds,
                LinkIndex = 0
            };

            eventData.Data.SetFollowEnabled(Settings, e.FollowEnabled);

            if (EventTypeEnumType == typeof(EventType))
                eventData.Data.Type = (EventType)eventData.Type;

            return eventData;
        }

        /// <summary>
        /// Gets the general event info data which matches the specified values
        /// </summary>
        /// <param name="type"></param>
        /// <param name="etat"></param>
        /// <param name="subEtat"></param>
        /// <param name="desKey"></param>
        /// <param name="etaKey"></param>
        /// <param name="offsetBx"></param>
        /// <param name="offsetBy"></param>
        /// <param name="offsetHy"></param>
        /// <param name="followSprite"></param>
        /// <param name="hitPoints"></param>
        /// <param name="hitSprite"></param>
        /// <param name="followEnabled"></param>
        /// <param name="labelOffsets"></param>
        /// <param name="commands"></param>
        /// <returns></returns>
        public GeneralEventInfoData GetGeneralEventInfo(int type, int etat, int subEtat, string desKey, string etaKey, int offsetBx, int offsetBy, int offsetHy, int followSprite, int hitPoints, int hitSprite, bool followEnabled, ushort[] labelOffsets, byte[] commands)
        {
            // Helper method for comparing the commands
            bool compareCommands(GeneralEventInfoData e)
            {
                if (UsesLocalCommands)
                    return e.LocalCommands.SequenceEqual(commands);
                else
                    return e.LabelOffsets.SequenceEqual(labelOffsets ?? new ushort[0]) &&
                           e.Commands.SequenceEqual(commands ?? new byte[0]);
            }

            // Find a matching item
            var match = EventInfoData.FindItem(x => IsAvailableInWorld(x) &&
                                                    x.Type == type &&
                                                    x.Etat == etat &&
                                                    x.SubEtat == subEtat &&
                                                    GetDesKey(x) == desKey &&
                                                    GetEtaKey(x) == etaKey &&
                                                    x.OffsetBX == offsetBx &&
                                                    x.OffsetBY == offsetBy &&
                                                    x.OffsetHY == offsetHy &&
                                                    x.FollowSprite == followSprite &&
                                                    x.HitPoints == hitPoints &&
                                                    x.HitSprite == hitSprite &&
                                                    x.FollowEnabled == followEnabled &&
                                                    compareCommands(x));

            // Create dummy item if not found
            if (match == null && EventInfoData.Any())
                Debug.LogWarning($"Matching event not found for event with type {type}, etat {etat} & subetat {subEtat} in level {Settings.World}{Settings.Level}");

            // Return the item
            return match;
        }

        public string GetDisplayName(Editor_EventData e)
        {
            // Get the command bytes
            var cmds = e.CommandCollection?.ToBytes(Settings);

            // Find match
            var match = GetGeneralEventInfo(e.TypeValue, e.Data.Etat, e.Data.SubEtat, e.DESKey, e.ETAKey, e.Data.OffsetBX, e.Data.OffsetBY, e.Data.OffsetHY, e.Data.FollowSprite, e.Data.HitPoints, e.Data.HitSprite, e.Data.GetFollowEnabled(Settings), e.LabelOffsets, cmds);

            // Return the editor info
            return match?.Name;
        }

        /// <summary>
        /// Gets the DES key for the specified event data item
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>The DES key</returns>
        public abstract string GetDesKey(GeneralEventInfoData eventInfoData);

        /// <summary>
        /// Gets the ETA key for the specified event data item
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>The ETA key</returns>
        public abstract string GetEtaKey(GeneralEventInfoData eventInfoData);
        /// <summary>
        /// Checks if the event is available in the current world
        /// </summary>
        /// <param name="eventInfoData">The event info data item</param>
        /// <returns>True if it's available, otherwise false</returns>
        public abstract bool IsAvailableInWorld(GeneralEventInfoData eventInfoData);

        /// <summary>
        /// Sets up small and bad Rayman's animations
        /// </summary>
        public void InitializeRayAnim()
        {
            // Hard-code event animations for the different Rayman types
            Common_Design rayDes = null;

            var rayEvent = Level.Rayman ?? Level.EventData.Find(x => x.Type is EventType et && et == EventType.TYPE_RAY_POS || x.Type is PS1_R2Demo_EventType et2 && et2 == PS1_R2Demo_EventType.RaymanPosition);

            if (rayEvent != null)
                rayDes = DES.TryGetItem(rayEvent.DESKey);

            if (rayDes == null)
                return;

            var miniRay = Level.EventData.Find(x => x.Type is EventType et && et == EventType.TYPE_DEMI_RAYMAN);

            if (miniRay != null)
            {
                var miniRayDes = DES.TryGetItem(miniRay.DESKey);

                if (miniRayDes != null)
                {
                    miniRayDes.Animations = rayDes.Animations.Select(anim =>
                    {
                        var newAnim = new Common_Animation
                        {
                            Frames = anim.Frames.Select(x => new Common_AnimFrame()
                            {
                                FrameData = new Common_AnimationFrame
                                {
                                    XPosition = (byte)(x.FrameData.XPosition / 2),
                                    YPosition = (byte)(x.FrameData.YPosition / 2),
                                    Width = (byte)(x.FrameData.Width / 2),
                                    Height = (byte)(x.FrameData.Height / 2)
                                },
                                Layers = x.Layers.Select(l => new Common_AnimationPart()
                                {
                                    ImageIndex = l.ImageIndex,
                                    XPosition = l.XPosition / 2,
                                    YPosition = l.YPosition / 2,
                                    IsFlippedHorizontally = l.IsFlippedHorizontally,
                                    IsFlippedVertically = l.IsFlippedVertically,
                                }).ToArray()
                            }).ToArray()
                        };

                        return newAnim;
                    }).ToList();
                }
            }

            var badRay = Level.EventData.Find(x => x.Type is EventType et && et == EventType.TYPE_BLACK_RAY);

            if (badRay != null)
            {
                var badRayDes = DES.TryGetItem(badRay.DESKey);

                if (badRayDes != null)
                    badRayDes.Animations = rayDes.Animations;
            }
        }

        #endregion
    }
}