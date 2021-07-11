using DigBuild.Behaviors;
using DigBuild.Engine.Blocks;
using DigBuild.Engine.Items;
using DigBuild.Engine.Math;
using DigBuild.Engine.Physics;
using DigBuild.Engine.Registries;
using HardCodedMod.Content.Blocks;
using System;

namespace HardCodedMod.Content.Registries {
    public static class ModBlocks {
        public static Block WoodDoorTop { get; private set; } = null!;
        public static Block WoodDoorBottom { get; private set; } = null!;

        internal static void Register(RegistryBuilder<Block> registry) {
            RegisterDoor(registry, IDoorMaterial.WOOD, out Block woodDoorTop, out Block woodDoorBottom, () => ModItems.WoodDoor);
            WoodDoorTop = woodDoorTop;
            WoodDoorBottom = woodDoorBottom;
        }

        private static void RegisterDoor(RegistryBuilder<Block> registry, IDoorMaterial material, out Block DoorTop, out Block DoorBottom, Func<Item> itemSupplier) {
            string door_material = material.Name;

            DoorTop = registry.Create(HardCodedMod.Domain, $"{door_material}_door_top", builder => {
                    builder.Attach(new ColliderBehavior(new VoxelCollider(new AABB(0, 0, 0, 0.25f, 1, 1))));
                    builder.Attach(new RayColliderBehavior(new VoxelRayCollider(new AABB(0, 0, 0, 0.25f, 1, 1))));
                    builder.Attach(new NonSolidBehavior());
                    builder.Attach(new DoorBehavior(IDoorPart.TOP, IDoorState.CLOSED, material));
                },
                Drops(itemSupplier)
            );
            DoorBottom = registry.Create(HardCodedMod.Domain, $"{door_material}_door_bottom", builder => {
                    builder.Attach(new ColliderBehavior(new VoxelCollider(new AABB(0, 0, 0, 0.25f, 1, 1))));
                    builder.Attach(new RayColliderBehavior(new VoxelRayCollider(new AABB(0, 0, 0, 0.25f, 1, 1))));
                    builder.Attach(new NonSolidBehavior());
                    builder.Attach(new DoorBehavior(IDoorPart.BOTTOM, IDoorState.CLOSED, material));
                },
                Drops(itemSupplier)
            );
            registry.Create(HardCodedMod.Domain, $"{door_material}_door_top_open", builder => {
                    builder.Attach(new ColliderBehavior(ICollider.None));
                    builder.Attach(new RayColliderBehavior(new VoxelRayCollider(new AABB(0, 0, 0, 1, 1, 0.25f))));
                    builder.Attach(new NonSolidBehavior());
                    builder.Attach(new DoorBehavior(IDoorPart.TOP, IDoorState.OPEN, material));
                },
                Drops(itemSupplier)
            );
            registry.Create(HardCodedMod.Domain, $"{door_material}_door_bottom_open", builder => {
                    builder.Attach(new ColliderBehavior(ICollider.None));
                    builder.Attach(new RayColliderBehavior(new VoxelRayCollider(new AABB(0, 0, 0, 1, 1, 0.25f))));
                    builder.Attach(new NonSolidBehavior());
                    builder.Attach(new DoorBehavior(IDoorPart.BOTTOM, IDoorState.OPEN, material));
                },
                Drops(itemSupplier)
            );
        }

        private static Action<BlockBuilder> Drops(Func<Item> itemSupplier, ushort amount = 1, float probability = 1) {
            return builder => {
                builder.Attach(new DropItemBehavior(() => new ItemInstance(itemSupplier(), amount), probability));
            };
        }
    }
}