using HardCodedMod.Content.Registries;
using DigBuild.Engine.Blocks;
using DigBuild.Engine.Entities;
using DigBuild.Engine.Events;
using DigBuild.Engine.Items;
using DigBuild.Events;
using DigBuild.Modding;
using DigBuild.Recipes;
using ModEntities = HardCodedMod.Content.Registries.ModEntities;

namespace HardCodedMod.Content {
    public sealed class HardCodedMod : IMod {
        public const string Domain = "hardcodedmod";

        public void Setup(EventBus bus) {
            bus.Subscribe<RegistryBuildingEvent<Block>>(evt => ModBlocks.Register(evt.Registry));
            bus.Subscribe<RegistryBuildingEvent<Entity>>(evt => ModEntities.Register(evt.Registry));
            bus.Subscribe<RegistryBuildingEvent<Item>>(evt => ModItems.Register(evt.Registry));
            bus.Subscribe<RegistryBuildingEvent<ICraftingRecipe>>(evt => ModRecipes.Register(evt.Registry));
        }
    }
}