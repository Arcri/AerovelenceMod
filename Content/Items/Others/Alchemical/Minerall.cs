using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria;

namespace AerovelenceMod.Content.Items.Others.Alchemical
{
    public class Minerall : TranslatableModItem
    {
        public override string Texture => "AerovelenceMod/Content/Items/Others/Alchemical/Minerall";
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Minerall", "Increases mining speed by 15%, movement speed by 10%, and mining reach by 1\nLasts 5 minutes and stacks with Mining Potion")
                .AddName(global::AerovelenceMod.Common.Systems.Language.Language.Spanish, "Minerall")
                .AddTooltip(global::AerovelenceMod.Common.Systems.Language.Language.Spanish, "Aumenta un 15% la velocidad de minería, un 10% la velocidad de movimiento y en 1 el alcance de minería\nDura 5 minutos y se acumula con la poción de minería");
            
            base.SetStaticDefaults();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(line => line.Mod == "Terraria" && line.Name.StartsWith("Tooltip"));
            tooltips.Add(new TooltipLine(Mod, "Tooltip0", "Increases mining speed by 15%, movement speed by 10%, and mining reach by 1\nLasts 5 minutes and stacks with Mining Potion"));
            base.ModifyTooltips(tooltips);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 20;
            Item.rare = ItemRarityID.Blue;

            Item.value = Item.sellPrice(silver: 2);
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTime = Item.useAnimation = 17;
            Item.UseSound = SoundID.Item3;
            Item.buffType = ModContent.BuffType<MinerallBoost>();
            Item.buffTime = 18000;
            
        }
        public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.MiningPotion).AddIngredient<CavernCrystalItem>(2).AddTile(TileID.Bottles).Register();
        

    }
    public class MinerallBoost : ModBuff
    {
        public override string Texture => "AerovelenceMod/Content/Items/Others/Alchemical/MinerallBoost";
        public override LocalizedText DisplayName => Terraria.Localization.Language.GetOrRegister("Mods.AerovelenceMod.Buffs.MinerallBoost.DisplayName", () => "Minerall");
        public override LocalizedText Description => Terraria.Localization.Language.GetOrRegister("Mods.AerovelenceMod.Buffs.MinerallBoost.Description", () => "15% faster mining, 10% movement speed, and 1 extra tile of mining reach");
        public override void Update(Player player, ref int buffIndex)
        {
            player.pickSpeed -= .15f;
            player.moveSpeed += .1f;
            player.blockRange++;
            if (player.velocity.LengthSquared() > 4f && Main.GameUpdateCount % 16 == 0)
                MinerallVFX.Spark(player.Bottom, -player.velocity * 0.1f, 0.13f, new Color(235, 192, 90));
        }
    }

    internal static class MinerallVFX
    {
        internal static readonly Color Aqua = new(85, 218, 255);

        internal static void Spark(Vector2 center, Vector2 velocity, float scale = 0.2f, Color? color = null)
        {
            if (Main.dedServ)
                return;
            Dust dust = Dust.NewDustPerfect(center, ModContent.DustType<GlowPixelCross>(), velocity, newColor: color ?? Aqua, Scale: scale);
            dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.1f, preSlowPower: 0.95f,
                timeBeforeSlow: 6, postSlowPower: 0.86f, velToBeginShrink: 1f, fadePower: 0.86f, shouldFadeColor: false);
        }
    }
}
