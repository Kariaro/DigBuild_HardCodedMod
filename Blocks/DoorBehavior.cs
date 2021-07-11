using DigBuild.Blocks;
using DigBuild.Content.Registries;
using DigBuild.Engine.Blocks;
using DigBuild.Engine.Impl.Worlds;
using DigBuild.Engine.Items;
using DigBuild.Engine.Math;
using DigBuild.Engine.Ticking;
using DigBuild.Engine.Worlds;
using DigBuild.Registries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HardCodedMod.Content.Blocks {
    public enum IDoorPart {
        TOP,
        BOTTOM
    }

    public enum IDoorState {
        OPEN,
        CLOSED
    }

    public readonly struct IDoorMaterial {
        public static IDoorMaterial WOOD => new IDoorMaterial("wood");

        public readonly string Name;

        private IDoorMaterial(string name) {
            Name = name;
        }

        public bool Equals(IDoorMaterial other) {
            return Name == other.Name;
        }

        public bool Equals(string other) {
            if(other == null) return false;
            return Name == other;
        }

        public override string ToString() {
            return $"{Name}";
        }
    }

    public class DoorBehavior : IBlockBehavior {
        private IDoorPart Part { get; }
        private IDoorState State { get; }
        private IDoorMaterial Material { get; }

        public DoorBehavior(IDoorPart part, IDoorState state, IDoorMaterial material) {
            Part = part;
            State = state;
            Material = material;
        }

        public void Build(BlockBehaviorBuilder<object, object> block) {
            block.Subscribe(OnActivate);
            block.Subscribe(OnPunch);
        }

        private BlockEvent.Activate.Result OnActivate(BlockEvent.Activate evt, object data, Func<BlockEvent.Activate.Result> next) {
            Block? next_top;
            Block? next_bot;
            if(State == IDoorState.CLOSED) {
                next_top = GameRegistries.Blocks.GetOrNull(HardCodedMod.Domain, $"{Material}_door_top_open");
                next_bot = GameRegistries.Blocks.GetOrNull(HardCodedMod.Domain, $"{Material}_door_bottom_open");
            } else {
                next_top = GameRegistries.Blocks.GetOrNull(HardCodedMod.Domain, $"{Material}_door_top");
                next_bot = GameRegistries.Blocks.GetOrNull(HardCodedMod.Domain, $"{Material}_door_bottom");
            }

            if(next_top == null || next_bot == null)
                return BlockEvent.Activate.Result.Fail;

            var pos = evt.Hit.BlockPos;
            var world = evt.Player.Entity.World;
            
            if(Part == IDoorPart.TOP) {
                pos = pos.Offset(Direction.NegY, 1);
            }

            world.SetBlock(pos, next_bot, true, true);
            world.SetBlock(pos.Offset(Direction.PosY, 1), next_top, true, true);

            return BlockEvent.Activate.Result.Success;
        }

        private BlockEvent.Punch.Result OnPunch(BlockEvent.Punch evt, object data, Func<BlockEvent.Punch.Result> next) {
            var pos = evt.Hit.BlockPos;
            var world = evt.Player.Entity.World;
            
            world.SetBlock(pos, null, true, true);
            if(Part == IDoorPart.TOP) {
                world.SetBlock(pos.Offset(Direction.NegY, 1), null, true, true);
            } else {
                world.SetBlock(pos.Offset(Direction.PosY, 1), null, true, true);
            }

            evt.World.AddEntity(DigBuild.Registries.GameEntities.Item)
                .WithPosition(((Vector3)evt.Pos) + new Vector3(0.5f, 0, 0.5f))
                .WithItem(new ItemInstance(GameRegistries.Items.GetOrNull(HardCodedMod.Domain, $"{Material}_door")!, 1));

            return BlockEvent.Punch.Result.Success;
        }
    }
}
