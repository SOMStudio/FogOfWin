using System;

namespace Data
{
    [Serializable]
    public class Slot
    {
        public SlotWheel[] wheels;

        public Slot(int countWheel, int countPoint)
        {
            wheels = new SlotWheel[countWheel];

            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i] = new SlotWheel(countPoint);
            }
        }

        public int Length => wheels.Length;

        public SlotWheel this[int w]
        {
            get => wheels[w];
            set => wheels[w] = value;
        }

        public WheelCell this[int w, int p]
        {
            get => wheels[w][p];
            set => wheels[w][p] = value;
        }
    }
}