using UnityEngine;

namespace Ray1Map.PSKlonoa
{
    public class CycleObjectsActiveComponent : ObjectAnimationComponent
    {
        public GameObject[] objects;

        protected override void UpdateAnimation()
        {
            if (objects == null)
                return;

            speed.Update(objects.Length, loopMode);

            int frameInt = speed.CurrentFrameInt;

            for (int i = 0; i < objects.Length; i++)
            {
                bool active = i == frameInt;

                if (objects[i].activeSelf != active)
                    objects[i].SetActive(active);
            }
        }
    }
}