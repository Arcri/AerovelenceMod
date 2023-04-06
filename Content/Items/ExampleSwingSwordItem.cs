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
using Terraria.Audio;
using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using AerovelenceMod.Content.Projectiles;
using System;

namespace AerovelenceMod.Content.Items
{
    //NOT DONE
    public class ExampleSwingSwordItem : ModItem
    {
        bool tick = false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Example Swing Sword");
            // Tooltip.SetDefault("Debug/Example Item");
        }
        public override void SetDefaults()
        {
            Item.knockBack = 2f;
            Item.crit = 2;
            Item.damage = 18;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Master;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Shoot;
            
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<ExampleSwingSwordProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            tick = !tick;
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (tick ? 1 : 0));
            return false;
        }

    }

    public class ExampleSwingSwordProj : BaseSwingSwordProj
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void AI()
        {

            easingAdditionAmount = 0.024f;

            StandardHeldProjCode();
            StandardSwingUpdate();

            if (getProgress(easingProgress) > 0.98)
            {
                Main.player[Projectile.owner].itemTime = 0;
                Main.player[Projectile.owner].itemAnimation = 0;
                Projectile.active = false;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Blade = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/ExampleSwingSwordItem");
            Main.spriteBatch.Draw(Blade, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + (Projectile.ai[0] != 1 ? 0 : MathHelper.PiOver2 * 3), Blade.Size() / 2, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f), Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            return false;
        }
    }

}