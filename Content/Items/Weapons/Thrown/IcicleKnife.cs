using System;
using AerovelenceMod.Content.Items.Others.Crafting;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Thrown
{
    public class IcicleKnife : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Icicle Knife");
            Tooltip.SetDefault("Inflicts frostburn");
        }
        public override void SetDefaults()
        {
            item.UseSound = SoundID.Item1;
            item.crit = 5;
            item.damage = 16;
            item.melee = true;
            item.width = 60;
            item.height = 32;
            item.useTime = 17;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.consumable = true;
            item.maxStack = 999;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 0, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.shoot = mod.ProjectileType("IcicleKnifeProj");
            item.shootSpeed = 16f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<FrostShard>(), 5);
            recipe.AddRecipeGroup("IronBar", 1);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 50);
            recipe.AddRecipe();
        }
    }

    public class IcicleKnifeProj : ModProjectile
    {
        public bool e;
        public float rot = 0.5f;
        public int i;
        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 38;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.hostile = false;
            projectile.melee = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.aiStyle = 2;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 67, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
            Main.PlaySound(SoundID.Item10);
            return true;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 60);
        }
        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                int item =
                    Main.rand.NextBool(18)
                        ? Item.NewItem(projectile.getRect(), ModContent.ItemType<IcicleKnife>())
                        : 0;
                if (Main.netMode == NetmodeID.MultiplayerClient && item >= 0)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item, 1f);
                }
            }
        }
    }
}