using System;

namespace R1Engine
{
    /// <summary>
    /// Extension methods for <see cref="World"/> and <see cref="EventWorld"/>
    /// </summary>
    public static class WorldExtensions
    {
        /// <summary>
        /// Converts the world to an event world
        /// </summary>
        /// <param name="eventWorld">The world to convert</param>
        /// <returns>The event world</returns>
        public static EventWorld ToEventWorld(this World eventWorld)
        {
            switch (eventWorld)
            {
                case World.Jungle:
                    return EventWorld.Jungle;

                case World.Music:
                    return EventWorld.Music;

                case World.Mountain:
                    return EventWorld.Mountain;

                case World.Image:
                    return EventWorld.Image;

                case World.Cave:
                    return EventWorld.Cave;

                case World.Cake:
                    return EventWorld.Cake;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Converts an event world to a world
        /// </summary>
        /// <param name="eventWorld">The event world to convert</param>
        /// <returns>The world</returns>
        public static World ToWorld(this EventWorld eventWorld)
        {
            switch (eventWorld)
            {
                case EventWorld.Jungle:
                    return World.Jungle;

                case EventWorld.Music:
                    return World.Music;

                case EventWorld.Mountain:
                    return World.Mountain;

                case EventWorld.Image:
                    return World.Image;

                case EventWorld.Cave:
                    return World.Cave;

                case EventWorld.Cake:
                    return World.Cake;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}