using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture.Items;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Glimmerwood;
using Terraria.Localization;
using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Furniture
{
    //Platform
    #region Platform
    public class GlimmerwoodPlatformTile : ModTile
    {
        public override void SetStaticDefaults() => CommonTileHelper.SetupPlatform(this, new Color(123, 123, 123), ModContent.ItemType<GlimmerwoodPlatformItem>(), DustID.BlueCrystalShard);
        public override void PostSetDefaults() => Main.tileNoSunLight[Type] = false;
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }

    public class GlimmerwoodPlatformItem : ModItem
    {
        public override void SetDefaults() => CommonItemHelper.SetupPlaceableItem(this, 28, 14, 150, ModContent.TileType<GlimmerwoodPlatformTile>());
        public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 8).AddTile(TileID.WorkBenches).Register();
    }
    #endregion

    //Candle
    #region Workbench
    public class GlimmerwoodWorkbenchTile : ModTile { public override void SetStaticDefaults() => CommonTileHelper.SetupWorkbench(this, ModContent.ItemType<GlimmerwoodWorkbenchItem>()); }

    public class GlimmerwoodWorkbenchItem : ModItem
    {
        public override void SetDefaults() => CommonItemHelper.SetupPlaceableItem(this, 32, 16, 150, ModContent.TileType<GlimmerwoodWorkbenchTile>());
        public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 10).Register();
    }
    #endregion

    //Candle
    #region Candle
    public class GlimmerwoodCandleTile : ModTile
    {
        private bool isOn = true;
        public override void SetStaticDefaults() => CommonTileHelper.SetupCandle(this, new Color(123, 123, 123), ModContent.ItemType<GlimmerwoodPlatformItem>(), DustID.BlueCrystalShard);

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (isOn)
            {
                r = 0f;
                g = 0.75f;
                b = 1f;
            }
            else
            {
                r = 0f;
                g = 0f;
                b = 0f;
            }
        }
        public override void HitWire(int i, int j)
        {
            CommonTileHelper.HandleHitWire(i, j, tileWidth: 3, tileHeight: 3);
        }

        public override bool RightClick(int i, int j)
        {
            isOn = !isOn;
            return true;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            CommonTileHelper.HandlePostDraw(ModContent.Request<Texture2D>("AerovelenceMod/Content/Tiles/CrystalCaverns/Furniture/GlimmerwoodCandleTile_Flame"), i, j, spriteBatch, isOn, flameWidth: 54, flameHeight: 54, frameSize: 54);
        }

        public class GlimmerwoodCandleItem : ModItem
        {
            public override void SetDefaults() => CommonItemHelper.SetupPlaceableItem(this, 28, 14, 150, ModContent.TileType<GlimmerwoodCandleTile>());
            public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 8).AddTile(TileID.WorkBenches).Register();
        }
    }

    #endregion

    //Lamp
    #region Lamp
    #endregion

    //Candelabra
    #region Candelabra
    #endregion

    //Chandelier
    #region Chandelier
    public class GlimmerwoodChandelierTile : ModTile
    {
        public override void SetStaticDefaults() => CommonTileHelper.SetupChandelier(this, new Color(123, 123, 123), ModContent.ItemType<GlimmerwoodPlatformItem>(), DustID.BlueCrystalShard);

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].TileFrameX < 18)
            {
                r = 0f;
                g = 0.75f;
                b = 1f;
            }
            else
            {
                r = 0f;
                g = 0f;
                b = 0f;
            }
        }
        public override void HitWire(int i, int j)
        {
            CommonTileHelper.HandleHitWire(i, j, tileWidth: 3, tileHeight: 3);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            bool isOn = tile.TileFrameX < 54;
            CommonTileHelper.HandlePostDraw(ModContent.Request<Texture2D>("AerovelenceMod/Content/Tiles/CrystalCaverns/Furniture/GlimmerwoodChandelierTile_Flame"),i, j, spriteBatch,isOn, flameWidth: 54, flameHeight: 54, frameSize: 54);
        }
    }

    public class GlimmerwoodChandelierItem : ModItem
    {
        public override void SetDefaults() => CommonItemHelper.SetupPlaceableItem(this, 28, 14, 150, ModContent.TileType<GlimmerwoodChandelierTile>());
        public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 8).AddTile(TileID.WorkBenches).Register();
    }
    #endregion

    //Torch
    #region Torch
    #endregion

    //Chair
    #region Chair
    public class GlimmerwoodChairTile : ModTile
    {
        public const int NextStyleHeight = 40;
        public override void SetStaticDefaults() => CommonTileHelper.SetupChair(this, DustID.BlueCrystalShard, ModContent.ItemType<GlimmerwoodChairItem>(), new Color(123, 123, 123));
        public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info) => CommonTileHelper.ModifySittingTargetInfo(i, j, ref info, NextStyleHeight);
        public override bool RightClick(int i, int j) { CommonTileHelper.HandleChairRightClick(this, i, j); return true; }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            {
                return;
            }

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<GlimmerwoodChairItem>();

            if (Main.tile[i, j].TileFrameX / 18 < 1)
            {
                player.cursorItemIconReversed = true;
            }
        }
    }

    public class GlimmerwoodChairItem : ModItem
    {
        public override void SetDefaults() => CommonItemHelper.SetupPlaceableItem(this, 28, 14, 150, ModContent.TileType<GlimmerwoodChairTile>());
        public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 8).AddTile(TileID.WorkBenches).Register();
    }
    #endregion

    //Stool
    #region Stool
    public class GlimmerwoodStoolTile : ModTile
    {
        public const int NextStyleHeight = 40;
        public override void SetStaticDefaults() => CommonTileHelper.SetupChair(this, DustID.BlueCrystalShard, ModContent.ItemType<GlimmerwoodStoolItem>(), new Color(123, 123, 123));
        public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info) => CommonTileHelper.ModifySittingTargetInfo(i, j, ref info, NextStyleHeight);
        public override bool RightClick(int i, int j) { CommonTileHelper.HandleChairRightClick(this, i, j); return true; }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            {
                return;
            }

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<GlimmerwoodStoolItem>();

            if (Main.tile[i, j].TileFrameX / 18 < 1)
            {
                player.cursorItemIconReversed = true;
            }
        }
    }

    public class GlimmerwoodStoolItem : ModItem
    {
        public override void SetDefaults() => CommonItemHelper.SetupPlaceableItem(this, 28, 14, 150, ModContent.TileType<GlimmerwoodStoolTile>());
        public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 8).AddTile(TileID.WorkBenches).Register();
    }
    #endregion

    //Toilet
    #region Toilet
    #endregion

    //Sofa
    #region Sofa
    #endregion

    //Chest
    #region Chest
    public class GlimmerwoodChestTile : ModTile
    {
        public override void SetStaticDefaults() => CommonTileHelper.SetupChest(this, DustID.BlueCrystalShard, ModContent.ItemType<GlimmerwoodChestItem>(), new Color(123, 123, 123), "Glimmerwood Chest");
        public override bool RightClick(int i, int j)
        {
            return CommonTileHelper.HandleRightClick(this, i, j, Main.LocalPlayer, ItemID.GoldenKey);
        }
        public override void MouseOver(int i, int j) => CommonTileHelper.HandleMouseOver(this, i, j, ModContent.ItemType<GlimmerwoodChestItem>(), ItemID.GoldenKey);
    }

    public class GlimmerwoodChestItem : ModItem
    {
        public override void SetDefaults() => CommonItemHelper.SetupPlaceableItem(this, 28, 14, 150, ModContent.TileType<GlimmerwoodChestTile>());
        public override void AddRecipes() { CreateRecipe().AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 8).AddTile(TileID.WorkBenches).Register(); }
    }
    #endregion

    //Dresser
    #region Dresser
    public class GlimmerwoodDresserTile : ModTile
    {
        public override void SetStaticDefaults() => CommonTileHelper.SetupDresser(this, ModContent.ItemType<GlimmerwoodDresserItem>(), new Color(200, 200, 200), DustID.BlueCrystalShard);
        public override LocalizedText DefaultContainerName(int frameX, int frameY) => CreateMapEntryName();
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override void ModifySmartInteractCoords(ref int width, ref int height, ref int frameWidth, ref int frameHeight, ref int extraY)
        {
            width = 3;
            height = 1;
            extraY = 0;
        }
        public override bool RightClick(int i, int j)
        {
            CommonTileHelper.HandleDresserRightClick(i, j);
            return true;
        }
        public override void MouseOver(int i, int j) => CommonTileHelper.HandleMouseOverNearAndFarSharedLogic(Main.LocalPlayer, i, j, ModContent.ItemType<GlimmerwoodDresserItem>());
        public override void MouseOverFar(int i, int j)
        {
            Player player = Main.LocalPlayer;
            CommonTileHelper.HandleMouseOverNearAndFarSharedLogic(player, i, j, ModContent.ItemType<GlimmerwoodDresserItem>());
            if (player.cursorItemIconText == "")
            {
                player.cursorItemIconEnabled = false;
                player.cursorItemIconID = 0;
            }
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override void KillMultiTile(int i, int j, int frameX, int frameY) => Chest.DestroyChest(i, j);
    }

    public class GlimmerwoodDresserItem : ModItem
    {
        public override void SetDefaults() => CommonItemHelper.SetupPlaceableItem(this, 28, 14, 150, ModContent.TileType<GlimmerwoodDresserTile>());
        public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 8).AddTile(TileID.WorkBenches).Register();
    }

    #endregion

    //Piano
    #region Piano

    public class GlimmerwoodPipeOrganTile : ModTile
    {
        public override void SetStaticDefaults() => CommonTileHelper.SetupDecorativeMultiTile(this, "MapObject.PipeOrgan", new Color(123, 123, 123), 3, 2, ModContent.ItemType<GlimmerwoodPipeOrganItem>());
    }

    public class GlimmerwoodPipeOrganItem : ModItem
    {
        public override void SetDefaults() => CommonItemHelper.SetupPlaceableItem(this, 28, 14, 150, ModContent.TileType<GlimmerwoodPipeOrganTile>());
        public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 8).AddTile(TileID.WorkBenches).Register();
    }
    #endregion

    //Clock
    #region Clock
    public class GlimmerwoodClockTile : ModTile
    {
        public override void SetStaticDefaults() => CommonTileHelper.SetupClock(this, DustID.BlueCrystalShard, new Color(123, 123, 123), ModContent.ItemType<GlimmerwoodClockItem>());
        public override bool RightClick(int x, int y) { return CommonTileHelper.HandleClockRightClick(x, y); }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override void NumDust(int i, int j, bool fail, ref int num) { num = fail ? 1 : 3; }
    }

    public class GlimmerwoodClockItem : ModItem
    {
        public override void SetDefaults() => CommonItemHelper.SetupPlaceableItem(this, 28, 14, 150, ModContent.TileType<GlimmerwoodClockTile>());
        public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 8).AddRecipeGroup("AerovelenceMod:IronBars", 6).AddIngredient(ItemID.Glass, 6).AddTile(TileID.WorkBenches).Register();
    }
    #endregion

    //Bed
    #region Bed
    public class GlimmerwoodBedTile : ModTile
    {
        public override void SetStaticDefaults() => CommonTileHelper.SetupBed(this, new Color(123, 123, 123), ModContent.ItemType<GlimmerwoodBedItem>(), DustID.BlueCrystalShard);
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override void ModifySmartInteractCoords(ref int width, ref int height, ref int frameWidth, ref int frameHeight, ref int extraY)
        {
            width = 4;
            height = 2;
        }

        public override void ModifySleepingTargetInfo(int i, int j, ref TileRestingInfo info) => CommonTileHelper.ModifyBedSleepingTargetInfo(i, j, ref info);

        public override void NumDust(int i, int j, bool fail, ref int num) => num = 1;

        public override bool RightClick(int i, int j) => CommonTileHelper.HandleBedRightClick(i, j, ModContent.ItemType<GlimmerwoodBedItem>());

        public override void MouseOver(int i, int j) => CommonTileHelper.HandleBedMouseOver(i, j, ModContent.ItemType<GlimmerwoodBedItem>());
    }

    public class GlimmerwoodBedItem : ModItem
    {
        public override void SetDefaults() => CommonItemHelper.SetupPlaceableItem(this, 32, 22, 150, ModContent.TileType<GlimmerwoodBedTile>());
        public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 15).AddIngredient(ItemID.Silk, 5).AddTile(ModContent.TileType<CrystallineFabricator>()).Register();
    }
    #endregion

    //Door
    #region Door
    public class GlimmerwoodDoorTileOpen : ModTile
    {
        public override void SetStaticDefaults() => CommonTileHelper.SetupOpenDoor(this, ModContent.TileType<GlimmerwoodDoorTileClosed>(), ModContent.ItemType<GlimmerwoodDoorItem>(), new Color(200, 200, 200), DustID.BlueCrystalShard);
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<GlimmerwoodDoorItem>();
        }
    }

    public class GlimmerwoodDoorTileClosed : ModTile
    {
        public override void SetStaticDefaults() => CommonTileHelper.SetupClosedDoor(this, ModContent.TileType<GlimmerwoodDoorTileOpen>(), ModContent.ItemType<GlimmerwoodDoorItem>(), new Color(200, 200, 200), DustID.BlueCrystalShard);
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<GlimmerwoodDoorItem>();
        }
    }

    public class GlimmerwoodDoorItem : ModItem
    {
        public override void SetDefaults() => CommonItemHelper.SetupPlaceableItem(this, 28, 14, 150, ModContent.TileType<GlimmerwoodDoorTileClosed>());
        public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 8).AddTile(TileID.WorkBenches).Register();
    }
    #endregion

    //Sink
    #region Sink
    public class GlimmerwoodSinkTile : ModTile
    {
        public override void SetStaticDefaults() => CommonTileHelper.SetupDecorativeMultiTile(this, "MapObject.Sink", new Color(123, 123, 123), 2, 2, ModContent.ItemType<GlimmerwoodSinkItem>());
    }

    public class GlimmerwoodSinkItem : ModItem
    {
        public override void SetDefaults() => CommonItemHelper.SetupPlaceableItem(this, 28, 14, 150, ModContent.TileType<GlimmerwoodSinkTile>());
        public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 8).AddTile(TileID.WorkBenches).Register();
    }
    #endregion

    //Bookcase
    #region Bookcase
    public class GlimmerwoodBookcaseTile : ModTile { public override void SetStaticDefaults() => CommonTileHelper.SetupBookcase(this, new Color(123, 123, 123), ModContent.ItemType<GlimmerwoodBookcaseItem>()); }
    public class GlimmerwoodBookcaseItem : ModItem
    {
        public override void SetDefaults() => CommonItemHelper.SetupPlaceableItem(this, 32, 22, 150, ModContent.TileType<GlimmerwoodBookcaseTile>());
        public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 15).AddIngredient(ItemID.Silk, 5).AddTile(ModContent.TileType<CrystallineFabricator>()).Register();
    }
    #endregion

    //Table
    #region Table
    public class GlimmerwoodTableTile : ModTile
    {
        public override void SetStaticDefaults() => CommonTileHelper.SetupTable(this, ModContent.ItemType<GlimmerwoodTableItem>());
    }

    public class GlimmerwoodTableItem : ModItem
    {
        public override void SetDefaults() => CommonItemHelper.SetupPlaceableItem(this, 28, 14, 150, ModContent.TileType<GlimmerwoodTableTile>());
        public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 8).AddTile(TileID.WorkBenches).Register();
    }
    #endregion

    //Bathtub
    #region Bathtub
    public class GlimmerwoodBathtubTile : ModTile { public override void SetStaticDefaults() => CommonTileHelper.SetupBathtub(this, new Color(123, 123, 123), ModContent.ItemType<GlimmerwoodBathtubItem>()); }
    public class GlimmerwoodBathtubItem : ModItem
    {
        public override void SetDefaults() => CommonItemHelper.SetupPlaceableItem(this, 28, 14, 150, ModContent.TileType<GlimmerwoodBathtubTile>());
        public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<GlimmerwoodItem>(), 8).AddTile(TileID.WorkBenches).Register();
    }

    
    #endregion
}