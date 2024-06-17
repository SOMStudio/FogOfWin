using System;

namespace Data
{
    [Serializable]
    public class SlotWheel
    {
        public WheelCell[] places;

        public SlotWheel(int countPlaces)
        {
            places = new WheelCell[countPlaces];
        }

        public int Length => places.Length;

        public WheelCell this[int p]
        {
            get => places[p];
            set => places[p] = value;
        }
    }
}