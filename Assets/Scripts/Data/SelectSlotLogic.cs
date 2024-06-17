using System.Collections.Generic;
using System.Linq;

namespace Data
{
    public partial class GameLogic
    {
        public List<WheelCell> SelectSlotCellList(Slot slot, BusterType busterTypeSet, SlotPosition selectSlotPosition)
        {
            switch (busterTypeSet)
            {
                case BusterType.LineHorizontal:
                    return slot.wheels.Select(slotWheel => slotWheel[selectSlotPosition.Cell]).ToList();
                case BusterType.LineVertical:
                    return slot[selectSlotPosition.Wheel].places.ToList();
                default:
                    return new List<WheelCell> { slot[selectSlotPosition.Wheel, selectSlotPosition.Cell] };
            }
        }
    }
}