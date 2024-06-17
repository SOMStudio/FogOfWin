using System;

namespace Data
{
    public partial class GameLogic
    {
        public struct SlotPosition
        {
            public int Wheel;
            public int Cell;

            public static bool operator ==(SlotPosition check1, SlotPosition check2)
            {
                return check1.Wheel == check2.Wheel && check1.Cell == check2.Cell;
            }

            public static bool operator !=(SlotPosition check1, SlotPosition check2)
            {
                return !(check1 == check2);
            }

            public static SlotPosition operator +(SlotPosition addTo, SlotPosition addPosition)
            {
                return new SlotPosition()
                    { Wheel = addTo.Wheel + addPosition.Wheel, Cell = addTo.Cell + addPosition.Cell };
            }

            public static SlotPosition operator +(SlotPosition addTo, int[] addPosition)
            {
                return new SlotPosition() { Wheel = addTo.Wheel + addPosition[0], Cell = addTo.Cell + addPosition[1] };
            }

            public bool Equals(SlotPosition other)
            {
                return Wheel == other.Wheel && Cell == other.Cell;
            }

            public override bool Equals(object obj)
            {
                return obj is SlotPosition other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Wheel, Cell);
            }
        }
    }
}