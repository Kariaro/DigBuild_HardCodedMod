
using DigBuild.Engine.Items;
using DigBuild.Engine.Registries;
using DigBuild.Platform.Resource;
using DigBuild.Recipes;
using DigBuild.Content.Registries;

namespace HardCodedMod.Content.Registries {
    public static class ModRecipes {
        internal static void Register(RegistryBuilder<ICraftingRecipe> registry) {
            var stoneIngredient = new CraftingIngredient(GameItems.Stone);
            registry.Add(new ResourceName(HardCodedMod.Domain, "craft_fluid_pouch"),
                new CraftingRecipe(
                    new[] {
                        CraftingIngredient.None, CraftingIngredient.None,
                        CraftingIngredient.None, CraftingIngredient.None, CraftingIngredient.None,
                        stoneIngredient, CraftingIngredient.None
                    },
                    new[] {
                        CraftingIngredient.None, CraftingIngredient.None,
                        CraftingIngredient.None, CraftingIngredient.None
                    },
                    CraftingIngredient.None,
                    new ItemInstance(ModItems.FluidPouch, 1)
                )
            );
        }
    }
}