using DigBuild.Blocks;
using DigBuild.Content.Registries;
using DigBuild.Engine.Blocks;
using DigBuild.Engine.Impl.Worlds;
using DigBuild.Engine.Items;
using DigBuild.Engine.Math;
using DigBuild.Engine.Physics;
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

    public class DoorBehavior2 : IBlockBehavior<DoorData> {
        private IDoorMaterial Material { get; }

        private readonly ICollider VoxelNegX = new VoxelCollider(new AABB(0, 0, 0, 0.25f, 1, 1));
        private readonly ICollider VoxelPosX = new VoxelCollider(new AABB(0.75f, 0, 0, 1, 1, 1));
        private readonly ICollider VoxelNegZ = new VoxelCollider(new AABB(0, 0, 0.25f, 0, 1, 1));
        private readonly ICollider VoxelPosZ = new VoxelCollider(new AABB(0, 0, 0.75f, 1, 1, 1));
        private readonly VoxelRayCollider VoxelRayNegX = new VoxelRayCollider(new AABB(0, 0, 0, 0.25f, 1, 1));
        private readonly VoxelRayCollider VoxelRayPosX = new VoxelRayCollider(new AABB(0.75f, 0, 0, 1, 1, 1));
        private readonly VoxelRayCollider VoxelRayNegZ = new VoxelRayCollider(new AABB(0, 0, 0.25f, 0, 1, 1));
        private readonly VoxelRayCollider VoxelRayPosZ = new VoxelRayCollider(new AABB(0, 0, 0.75f, 1, 1, 1));
        private readonly VoxelRayCollider VoxelRayNone = new VoxelRayCollider(new AABB(0, 0, 0, 1, 1, 1));

        public DoorBehavior2(IDoorMaterial material) {
            Material = material;
        }

        public void Build(BlockBehaviorBuilder<DoorData, DoorData> block) {
            block.Add(BlockAttributes.HorizontalDirection, (_, data, _) => data.Direction);
            block.Add(BlockAttributes.Direction, (_, data, _) => data.Direction);
            block.Add(BlockAttributes.Collider, (_, data, _) => data.State == IDoorState.OPEN ? ICollider.None:GetVoxelCollider(data.Direction));
            block.Add(BlockAttributes.RayCollider, (_, data, _) => GetVoxelRayCollider(data.Direction));

            block.Subscribe(OnActivate);
            block.Subscribe(OnPunch);
            block.Subscribe(OnPlaced);
        }

        private void OnPlaced(BlockEvent.Placed evt, DoorData data, Action next) {
            var forward = evt.Player.GetCamera(0).Forward;
            var xzForward = new Vector3(forward.X, 0, forward.Z);
            var direction = Directions.FromOffset(xzForward);
            data.Direction = direction;
        }

        private static Direction RotateOpenX(Direction dir) {
            return dir switch {
                Direction.NegX => Direction.NegZ,
                Direction.NegZ => Direction.PosX,
                Direction.PosX => Direction.PosZ,
                Direction.PosZ => Direction.NegX,
                _ => dir,
            };
        }

        private static Direction RotateCloseX(Direction dir) {
            return dir switch {
                Direction.NegX => Direction.PosZ,
                Direction.NegZ => Direction.NegX,
                Direction.PosX => Direction.NegZ,
                Direction.PosZ => Direction.PosX,
                _ => dir,
            };
        }

        private ICollider GetVoxelCollider(Direction dir) {
            return dir switch {
                Direction.NegX => VoxelNegX,
                Direction.NegZ => VoxelNegZ,
                Direction.PosX => VoxelPosX,
                Direction.PosZ => VoxelPosZ,
                _ => ICollider.None
            };
        }
        
        private VoxelRayCollider GetVoxelRayCollider(Direction dir) {
            return dir switch {
                Direction.NegX => VoxelRayNegX,
                Direction.NegZ => VoxelRayNegZ,
                Direction.PosX => VoxelRayPosX,
                Direction.PosZ => VoxelRayPosZ,
                _ => VoxelRayNone
            };
        }

        private BlockEvent.Activate.Result OnActivate(BlockEvent.Activate evt, DoorData data, Func<BlockEvent.Activate.Result> next) {
            if(data.State == IDoorState.CLOSED) {
                data.Direction = RotateOpenX(data.Direction);
            } else {
                data.Direction = RotateCloseX(data.Direction);
            }
            /*
            if(next_top == null || next_bot == null)
                return BlockEvent.Activate.Result.Fail;

            var pos = evt.Hit.BlockPos;
            var world = evt.Player.Entity.World;

            world.GetBlock(pos).Get<DoorData>()
            
            if(Part == IDoorPart.TOP) {
                pos = pos.Offset(Direction.NegY, 1);
            }

            world.SetBlock(pos, next_bot, true, true);
            world.SetBlock(pos.Offset(Direction.PosY, 1), next_top, true, true);
            */
            return BlockEvent.Activate.Result.Success;
        }

        private BlockEvent.Punch.Result OnPunch(BlockEvent.Punch evt, DoorData data, Func<BlockEvent.Punch.Result> next) {
            var pos = evt.Hit.BlockPos;
            var world = evt.Player.Entity.World;
            
            world.SetBlock(pos, null, true, true);
            /*
            if(Part == IDoorPart.TOP) {
                world.SetBlock(pos.Offset(Direction.NegY, 1), null, true, true);
            } else {
                world.SetBlock(pos.Offset(Direction.PosY, 1), null, true, true);
            }
            */

            evt.World.AddEntity(DigBuild.Registries.GameEntities.Item)
                .WithPosition(((Vector3)evt.Pos) + new Vector3(0.5f, 0, 0.5f))
                .WithItem(new ItemInstance(GameRegistries.Items.GetOrNull(HardCodedMod.Domain, $"{Material}_door")!, 1));

            return BlockEvent.Punch.Result.Success;
        }
    }
}
