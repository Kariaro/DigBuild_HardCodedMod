using HardCodedMod.Content.Behaviors;
using DigBuild.Engine.Storage;
using DigBuild.Engine.Math;

namespace HardCodedMod.Content.Blocks {

    public sealed class DoorData : IData<DoorData> {
        public IDoorPart Part { get; set; }
        public IDoorState State { get; set; }
        public Direction Direction { get; set; }

        public DoorData Copy() {
            return new DoorData() {
                Part = Part,
                State = State,
                Direction = Direction
            };
        }
    }
}