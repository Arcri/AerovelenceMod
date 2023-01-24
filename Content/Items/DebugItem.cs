using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Content.Dusts.GlowDusts;
using System;
using Terraria.Audio;
using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns;
using AerovelenceMod.Content.NPCs.Bosses.Rimegeist;
using AerovelenceMod.Content.Items.Weapons.Misc.Magic;
using AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry;
using AerovelenceMod.Content.Items.Weapons.Misc.Magic.FlashLight;

namespace AerovelenceMod.Content.Items
{
    public class DebugItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("DebugItem");
            Tooltip.SetDefault("You shouldn't have this...");
        }
        public override void SetDefaults()
        {
            //Item.UseSound = new SoundStyle("Terraria/Sounds/Item_122") with { Pitch = .86f, };
            Item.crit = 4;
            Item.damage = 22;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 46;
            Item.height = 28;
            Item.useTime = 7;
            Item.useAnimation = 7;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 9, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CyverHyperBeam>();
            //Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 1f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //int a = Projectile.NewProjectile(null, position, velocity.RotatedByRandom(0.5f) * 10f, ModContent.ProjectileType<InkProjTest>(), 10, 0, player.whoAmI);

            for (int i = 0; i < 3; i++)
            {
                int a = Projectile.NewProjectile(null, position + new Vector2(0,-15).RotatedBy(velocity.ToRotation()), velocity, ModContent.ProjectileType<BeaconShot>(), 3, 0, player.whoAmI);
                int b = Projectile.NewProjectile(null, position + new Vector2(0, 15).RotatedBy(velocity.ToRotation()), velocity, ModContent.ProjectileType<BeaconShot>(), 3, 0, player.whoAmI);
                int c = Projectile.NewProjectile(null, position, velocity, ModContent.ProjectileType<BeaconShot>(), 3, 0, player.whoAmI);

            }


            //SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/JuniorShot") with { Pitch = .01f, PitchVariance = .55f, Volume = 0.3f };
            //SoundEngine.PlaySound(style, player.Center);

            /*
            if (Main.rand.NextBool(3))
            {
                int b = Projectile.NewProjectile(null, position, velocity.RotatedByRandom(0.1f) * 10f, ModContent.ProjectileType<InkProjTest>(), 10, 0, player.whoAmI);

                if (Main.projectile[b].ModProjectile is InkProjTest m)
                {
                    m.color = Color.Orange;
                    m.overallSize = 0.15f;
                    m.xScale = Main.rand.NextFloat(0.8f, 1.3f);
                    m.yScale = Main.rand.NextFloat(0.9f, 1.2f);
                }
            }


            if (Main.projectile[a].ModProjectile is InkProjTest i)
            {
                i.color = Color.Orange;
                i.overallSize = 0.25f;
                i.xScale = Main.rand.NextFloat(0.8f, 1.3f);
                i.yScale = Main.rand.NextFloat(0.9f, 1.2f);
            }
            */

            //SoundStyle style = new SoundStyle("Terraria/Sounds/Item_100") with { Volume = .5f, Pitch = .73f, PitchVariance = .22f, };
            //SoundEngine.PlaySound(style);

            //SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_116") with { Volume = .41f, Pitch = .27f, PitchVariance = .2f, };
            //SoundEngine.PlaySound(style2);

            //Projectile.NewProjectile(null, position, velocity, ModContent.ProjectileType<StretchLaser>(), 0, 0, player.whoAmI);
            return false;
        }
    }
}