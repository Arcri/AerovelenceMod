using System;
using System.Collections.Generic;
using AerovelenceMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Magic
{
    public class CrystalGlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Glade");
            Tooltip.SetDefault("Legends say that this was just a rewritten Plant Fiber Cordage");
        }
        public override void SetDefaults()
        {
            Item.crit = 11;
            Item.damage = 97;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 5;
            Item.width = 30;
            Item.height = 34;
            Item.useTime = 7;
            Item.useAnimation = 7;
            Item.UseSound = SoundID.Item21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 20, 50, 0);
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CrystalGladeProj>();
            Item.shootSpeed = 14f;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = new TooltipLine(Mod, "Verbose:RemoveMe", "Why do I exist, dawg");
            tooltips.Add(line);

            line = new TooltipLine(Mod, "Crystal Glade", "Legendary item")
            {
                OverrideColor = new Color(255, 241, 000)
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

    public class CrystalGladeProj : ModProjectile
    {
        private readonly int oneHelixRevolutionInUpdateTicks = 30;
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 18;
            Projectile.alpha = 0;
            Projectile.penetrate = 4;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }
        public override bool PreAI()
        {
            ++Projectile.localAI[0];
            float piFraction = MathHelper.Pi / oneHelixRevolutionInUpdateTicks;
            Vector2 newDustPosition = new Vector2(0, (float)Math.Sin((Projectile.localAI[0] % oneHelixRevolutionInUpdateTicks) * piFraction)) * Projectile.height;
            Dust newDust = Dust.NewDustPerfect(Projectile.Center + newDustPosition.RotatedBy(Projectile.velocity.ToRotation()), 61);
            newDust.noGravity = true;
            newDustPosition.Y *= -1;
            newDust = Dust.NewDustPerfect(Projectile.Center + newDustPosition.RotatedBy(Projectile.velocity.ToRotation()), 61);
            newDust.noGravity = true;
            Projectile.rotation = Projectile.velocity.ToRotation();
            return (false);
        }
		public override bool PreDraw(ref Color lightColor) 
            => this.DrawAroundOrigin(Main.spriteBatch, lightColor * Projectile.Opacity);
    }
}