using System;
using System.Collections.Generic;
using DigBuild.Behaviors;
using HardCodedMod.Content.Behaviors;
using HardCodedMod.Content.Items;
using DigBuild.Engine.Blocks;
using DigBuild.Engine.Items;
using DigBuild.Engine.Math;
using DigBuild.Engine.Registries;
using DigBuild.Items;
using DigBuild.Platform.Resource;

namespace HardCodedMod.Content.Registries {
    public static class ModItems {
        public static Item FluidPouch { get; private set; } = null!;
        public static Item FluidPouchWater { get; private set; } = null!;

        internal static void Register(RegistryBuilder<Item> registry) {
            FluidPouch = registry.Create(new ResourceName(HardCodedMod.Domain, "fluid_pouch"), builder => {
                var data = builder.Add<FluidPouchData>();
                builder.Attach(new FluidPouchBehavior(FluidPouchState.EMPTY), data);
                builder.Attach(new EquippableBehavior(EquippableFlags.Equipment));
            });

            FluidPouchWater = registry.Create(new ResourceName(HardCodedMod.Domain, "fluid_pouch_water"), builder => {
                var data = builder.Add<FluidPouchData>();
                builder.Attach(new FluidPouchBehavior(FluidPouchState.WATER), data);
                builder.Attach(new EquippableBehavior(EquippableFlags.Equipment));
            });
        }
    }
}