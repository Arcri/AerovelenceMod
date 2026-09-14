using System.Collections.Generic;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Items.Ammo;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Glimmerwood;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.CrystalCaverns
{
    public class GlimmerwoodBow : TranslatableModItem
    {
        private const string Description = "Every third shot turns its arrow into a Crystal Drillrow\nDrillrows bore through one section of terrain";
        private int shots;
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/GlimmerwoodBow/GlimmerwoodBow";

        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Glimmerwood Bow", Description)
                .AddName(Language.Spanish, "Arco de Madera Reluciente")
                .AddTooltip(Language.Spanish, "Cada tercer disparo transforma su flecha en una Flecha Taladro de Cristal\nLas flechas taladro perforan una sección de terreno");
            base.SetStaticDefaults();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(line => line.Mod == "Terraria" && line.Name.StartsWith("Tooltip"));
            tooltips.Add(new TooltipLine(Mod, "Tooltip0", Description));
            base.ModifyTooltips(tooltips);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 28;
            Item.height = 34;
            Item.damage = 6;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 1f;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.Arrow;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 6.2f;
            Item.UseSound = SoundID.Item5;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(copper: 30);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            shots = (shots + 1) % 3;
            if (shots != 0)
                return true;
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<CrystalDrillrowShot>(), damage, knockback, player.whoAmI);
            SoundEngine.PlaySound(SoundID.Item28 with { Volume = 0.25f, Pitch = 0.6f }, position);
            for (int i = 0; i < 6; i++)
                Spark(position, velocity.RotatedBy(Main.rand.NextFloat(-0.35f, 0.35f)) * Main.rand.NextFloat(0.15f, 0.4f));
            return false;
        }

        public override void HoldItem(Player player)
        {
            base.HoldItem(player);
            if (Main.dedServ || shots != 2 || player.dead || player.itemAnimation > 0 || Main.GameUpdateCount % 24 != 0)
                return;
            Spark(player.MountedCenter + new Vector2(player.direction * 12f, -7f), -Vector2.UnitY * 0.5f);
        }

        private static void Spark(Vector2 position, Vector2 velocity)
        {
            if (Main.dedServ)
                return;
            Dust spark = Dust.NewDustPerfect(position, ModContent.DustType<GlowPixelCross>(), velocity, newColor: new Color(95, 220, 255), Scale: 0.15f);
            spark.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.05f, preSlowPower: 0.9f, timeBeforeSlow: 3,
                postSlowPower: 0.85f, velToBeginShrink: 1f, fadePower: 0.87f, shouldFadeColor: false);
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<GlimmerwoodItem>(10).AddTile(TileID.WorkBenches).Register();
    }
}
