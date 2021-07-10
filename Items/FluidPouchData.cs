using HardCodedMod.Content.Behaviors;
using DigBuild.Engine.Storage;

namespace HardCodedMod.Content.Items {
    public enum FluidPouchState {
        EMPTY,
        WATER,
    }

    public sealed class FluidPouchData : IData<FluidPouchData>, IFluidPouch {
        public FluidPouchState State { get; private set; }

        public FluidPouchData() {
            State = FluidPouchState.EMPTY;
        }

        public void SetFluid(FluidPouchState type) {
            State = type;
        }

        public FluidPouchData Copy() {
            return new FluidPouchData() {
                State = State
            };
        }
    }
}