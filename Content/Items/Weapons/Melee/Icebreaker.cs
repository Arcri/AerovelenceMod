using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class Icebreaker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Icebreaker");
            Tooltip.SetDefault("'A forgotten hero's sword, lost in the tundra'\nHas a chance to rain ice above an enemy when hit");
            Tooltip.SetDefault("This item is unfinished!");
        }

        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item1;
            Item.crit = 8;
            Item.damage = 14;
            Item.DamageType = DamageClass.Melee;
            Item.width = 50;
            Item.height = 54; 
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = Item.sellPrice(0, 0, 40, 20);
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            Vector2 position = target.position - Vector2.UnitY * 60;
            Vector2 velocity = target.position * 5;
            Projectile.NewProjectile(position, velocity, ModContent.ProjectileType<IcebreakerIcicle>(), damage, knockBack, player.whoAmI);
            base.OnHitNPC(player, target, damage, knockBack, crit);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = new TooltipLine(Mod, "Verbose:RemoveMe", "This is pretty wwwwwwwwoooooeeeeedfdoah");
            tooltips.Add(line);

            line = new TooltipLine(Mod, "Icebreaker", "Artifact")
            {
                overrideColor = new Color(255, 241, 000)
            };
            tooltips.Add(line);
            foreach (TooltipLine line2 in tooltips)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.OverrideColor = new Color(255, 132, 000);
                }
            }
            tooltips.RemoveAll(l => l.Name.EndsWith(":RemoveMe"));
        }
    }

    public class IcebreakerIcicle : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.aiStyle = -1;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.alpha = 255;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity.Y += 3;
            Projectile.rotation += Projectile.velocity.X * 0.01f;
            DelegateMethods.v3_1 = new Vector3(0.6f, 1f, 1f) * 0.2f;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * 10f, 8f, DelegateMethods.CastLightOpen);
            if (Projectile.alpha > 0)
            {
                SoundEngine.PlaySound(SoundID.Item9, Projectile.Center);
                Projectile.alpha = 0;
                Projectile.scale = 1.1f;
                float num101 = 16f;
                for (int num102 = 0; num102 < num101; num102++)
                {
                    Vector2 spinningpoint5 = Vector2.UnitX * 0f;
                    spinningpoint5 += -Vector2.UnitY.RotatedBy((float)num102 * ((float)Math.PI * 2f / num101)) * new Vector2(1f, 4f);
                    spinningpoint5 = spinningpoint5.RotatedBy(Projectile.velocity.ToRotation());
                    int num103 = Dust.NewDust(Projectile.Center, 0, 0, 180);
                    Main.dust[num103].scale = 1.5f;
                    Main.dust[num103].noGravity = true;
                    Main.dust[num103].position = Projectile.Center + spinningpoint5;
                    Main.dust[num103].velocity = Projectile.velocity * 0f + spinningpoint5.SafeNormalize(Vector2.UnitY) * 1f;
                }
            }
        }
    }
}