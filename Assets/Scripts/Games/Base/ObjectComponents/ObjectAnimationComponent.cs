using UnityEngine;

namespace Ray1Map
{
    public abstract class ObjectAnimationComponent : MonoBehaviour
    {
        public AnimSpeed speed = new AnimSpeed_FrameDelay(1);
        public AnimLoopMode loopMode = AnimLoopMode.Repeat;

        public void Update()
        {
            if (Controller.LoadState != Controller.State.Finished || !Settings.AnimateSprites)
                return;

            UpdateAnimation();
        }

        protected abstract void UpdateAnimation();
    }
}