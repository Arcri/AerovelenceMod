using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;

namespace AerovelenceMod.Content.Items.Weapons.Melee
{
    public class Icebreaker : ModItem
    {
        Random rnd = new Random();
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Icebreaker");
            Tooltip.SetDefault("'A forgotten hero's sword, lost in the tundra'\nHas a chance to rain ice above an enemy when hit");
        }

        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item1;
            Item.crit = 4;
            Item.damage = 10;
            Item.DamageType = DamageClass.Melee;
            Item.width = 50;
            Item.height = 54; 
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 2, 40, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.scale = 1f;
            Item.alpha = 255;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (Main.netMode != NetmodeID.Server && Main.myPlayer == player.whoAmI)
            {
                float variation = rnd.Next(21);
                Projectile.NewProjectile(Item.GetSource_FromThis(), target.Top.X - 150 - 10 + variation, target.Top.Y - 150 + variation, 4f, 4f, ModContent.ProjectileType<IcebreakerIcicle>(), damage, knockBack, player.whoAmI);
                //The -10 is separated to clarify that it is an offset to the variation variable, so a range of negative to positive 10 can be reached. If you just put in rnd.Next(-10, 11) then it only generates positives for some reason
                //Skip the middle variation due to it looking odd
                Projectile.NewProjectile(Item.GetSource_FromThis(), target.Top.X, target.Top.Y - 160, 0f, 4f, ModContent.ProjectileType<IcebreakerIcicle>(), damage, knockBack, player.whoAmI);
                variation = rnd.Next(21);
                Projectile.NewProjectile(Item.GetSource_FromThis(), target.Top.X + 150 - 10 + variation, target.Top.Y - 150 + variation, -4f, 4f, ModContent.ProjectileType<IcebreakerIcicle>(), damage, knockBack, player.whoAmI);
                if (target.type != NPCID.TheHungry) //Balancing for the WoF fight
                {
                    variation = rnd.Next(21);
                    Projectile.NewProjectile(Item.GetSource_FromThis(), target.Top.X + 75 - 10 + variation, target.Top.Y - 160 + variation, -2f, 4f, ModContent.ProjectileType<IcebreakerIcicle>(), damage, knockBack, player.whoAmI);
                    variation = rnd.Next(21);
                    Projectile.NewProjectile(Item.GetSource_FromThis(), target.Top.X - 75 - 10 + variation, target.Top.Y - 160 + variation, 2f, 4f, ModContent.ProjectileType<IcebreakerIcicle>(), damage, knockBack, player.whoAmI);
                }
            }
            base.OnHitNPC(player, target, damage, knockBack, crit);
            SoundEngine.PlaySound(SoundID.Item28, player.position);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = new TooltipLine(Mod, "Icebreaker", "Artifact")
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
            Projectile.maxPenetrate = 3;
            //Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.scale = 0.85f;
            Projectile.alpha = 255;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost, 0f, 0f, 100, Color.Blue, 1f);
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost, 0f, 0f, 100, Color.BlueViolet, 1f);
            base.OnSpawn(source);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, 14, knockback, false);
        }

        public override void Kill(int timeLeft)
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost, 0f, 0f, 100, Color.BlueViolet, 1f);
            base.Kill(timeLeft);
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.rotation -= 1.5708f; //90 degrees is 1.5708 radians

            if (Projectile.alpha > 0) {
                Projectile.alpha -= 15;
            }

            if (Projectile.ai[0] % 6 == 1) {
                Projectile.velocity *= 1.10f;
            }
        }
    }
}
