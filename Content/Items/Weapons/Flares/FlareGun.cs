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

namespace AerovelenceMod.Content.Items.Weapons.Flares
{
    public class FlareGun : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flare Gun");
            Tooltip.SetDefault("4% summon tag crit chance \n ");
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item110;
            Item.crit = 4;
            Item.damage = 20;
            Item.DamageType = DamageClass.Summon;
            Item.width = 46;
            Item.height = 28;
            Item.useTime = 70;
            Item.useAnimation = 70;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 9, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FireFlare>();
            //Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 13f * 1.5f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

            /*
            float aim = velocity.ToRotation() + MathHelper.Pi;

            for (int m = 0; m < 14; m++) // m < 9
            {
                float dustRot = aim + 1.57f * 1.5f + Main.rand.NextFloat(-0.4f, 0.4f);


                Dust d = GlowDustHelper.DrawGlowDustPerfect(player.Center - aim.ToRotationVector2() * 20, ModContent.DustType<GlowCircleDust>(), Vector2.One.RotatedBy(dustRot) * (Main.rand.NextFloat(3) + 1),
                    new Color(255, 75, 50), 0.40f + Main.rand.NextFloat(0,0.3f), 0.65f, 0f, dustShader); // 0.6
                d.velocity *= 0.75f;
            }
            */
            return true;
        }
    }
}