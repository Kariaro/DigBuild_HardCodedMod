using System;
using System.Linq;
using DigBuild;
using DigBuild.Content.Registries;
using DigBuild.Engine.Blocks;
using DigBuild.Engine.Impl.Worlds;
using DigBuild.Engine.Items;
using DigBuild.Items;
using DigBuild.Platform.Resource;
using DigBuild.Registries;
using DigBuild.Ui;
using HardCodedMod.Content.Items;

namespace HardCodedMod.Content.Behaviors {
    public interface IFluidPouch {
        FluidPouchState State { get; }
        
        void SetFluid(FluidPouchState type);
    }

    public sealed class FluidPouchBehavior : IItemBehavior<IFluidPouch> {
        private FluidPouchState State { get; }

        public FluidPouchBehavior(FluidPouchState state) {
            State = state;
        }

        public void Build(ItemBehaviorBuilder<IFluidPouch, IFluidPouch> item) {
            item.Subscribe(OnActivate);
            item.Subscribe(OnEquipmentActivate);
        }

        private ItemEvent.Activate.Result OnActivate(ItemEvent.Activate evt, IFluidPouch data, Func<ItemEvent.Activate.Result> next) {
            if (evt.Hit == null)
                return next();

            var pos = evt.Hit.BlockPos.Offset(evt.Hit.Face);
            var world = evt.Player.Entity.World;
            Block? block = ChunkBlocksExtensions.GetBlock(world, pos);

            if(State == FluidPouchState.EMPTY) {
                if(block == null)
                    return ItemEvent.Activate.Result.Fail;

                if(!block.Name.Equals(new ResourceName(DigBuildGame.Domain, "water")))
                    return ItemEvent.Activate.Result.Fail;

                if(!world.SetBlock(pos, null, true, false))
                    return ItemEvent.Activate.Result.Fail;
                
                // Fill with water if looking at water
                evt.Player.Inventory.Hand.TrySetItem(new ItemInstance(GameRegistries.Items.GetOrNull(HardCodedMod.Domain, "fluid_pouch_water")!, 1));
                
            } else {
                if(block != null)
                    return ItemEvent.Activate.Result.Fail;

                if(!world.SetBlock(pos, GameRegistries.Blocks.GetOrNull(DigBuildGame.Domain, "water"), true, false))
                    return ItemEvent.Activate.Result.Fail;
                
                // Empty water always
                evt.Player.Inventory.Hand.TrySetItem(new ItemInstance(GameRegistries.Items.GetOrNull(HardCodedMod.Domain, "fluid_pouch")!, 1));
            }

            return ItemEvent.Activate.Result.Success;
        }

        private void OnEquipmentActivate(ItemEvent.EquipmentActivate evt, IFluidPouch data, Action next) {
            
        }

        public bool Equals(IFluidPouch first, IFluidPouch second) {
            return first.State == second.State;
        }
    }
}