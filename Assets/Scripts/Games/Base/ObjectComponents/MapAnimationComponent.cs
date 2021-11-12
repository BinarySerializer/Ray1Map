using UnityEngine;

namespace Ray1Map
{
    public abstract class MapAnimationComponent : MonoBehaviour
    {
        public AnimSpeed speed = new AnimSpeed_FrameDelay(1);
        public AnimLoopMode loopMode = AnimLoopMode.Repeat;

        public void Update()
        {
            if (Controller.LoadState != Controller.State.Finished || !Settings.AnimateTiles)
                return;

            UpdateAnimation();
        }

        protected abstract void UpdateAnimation();
    }
}