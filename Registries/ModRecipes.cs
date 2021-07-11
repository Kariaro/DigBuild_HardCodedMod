
using DigBuild.Engine.Items;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;
using DigBuild.Recipes;
using DigBuild.Content.Registries;

namespace HardCodedMod.Content.Registries {
    public static class ModRecipes {
        internal static void Register(RegistryBuilder<ICraftingRecipe> registry) {
            var none = CraftingIngredient.None;
            var STONE = new CraftingIngredient(GameItems.Stone);
            var WOOD = new CraftingIngredient(GameItems.Log);

            registry.Add(new ResourceName(HardCodedMod.Domain, "craft_fluid_pouch"),
                new CraftingRecipe(
                    new[] {
                            none, none,
                        STONE, none, STONE,
                           STONE, STONE
                    },
                    new[] {
                        none, none,
                        none, none
                    },
                    none,
                    new ItemInstance(ModItems.FluidPouch, 1)
                )
            );

            registry.Add(new ResourceName(HardCodedMod.Domain, "craft_wood_door"),
                new CraftingRecipe(
                    new[] {
                           WOOD, WOOD,
                        WOOD, none, none,
                           WOOD, WOOD
                    },
                    new[] {
                        none, none,
                        none, none
                    },
                    none,
                    new ItemInstance(ModItems.WoodDoor, 1)
                )
            );
        }
    }
}