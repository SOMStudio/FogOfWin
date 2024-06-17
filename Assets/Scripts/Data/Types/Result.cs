using System.Collections.Generic;

namespace Data
{
    public partial class GameLogic
    {
        public struct Result
        {
            public int NumberPicture;
            public List<SlotPosition> FirstWheel;
            public List<SlotPosition> OtherWheels;
        }
    }
}