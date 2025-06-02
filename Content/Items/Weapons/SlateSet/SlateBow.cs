/*
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.ObjectModel;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using AerovelenceMod.Common.Utilities;

namespace AerovelenceMod.Content.Items.Weapons.SlateSet
{

    public class SlateBow : ModItem
    {
        float fadeIn = 0;
        private int timer = 0;
        private int myFrame = 0;
        Color sigilColor = Color.LightGreen;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slate Bow");
            Tooltip.SetDefault("Deals high damage, but fires slowly \n" +
                "Fires faster if you are standing still"); 
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item5;
            Item.crit = 0;
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 54;
            Item.useTime = 67;
            Item.useAnimation = 67;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shoot = AmmoID.Arrow;
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 12f;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0, 0);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 2; i++)
            { //0.3f | 25,25
                Projectile.NewProjectile(source, player.Center, (velocity * (0.4f + Main.rand.NextFloat(-0.1f, 0.1f))).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-15,15))), ModContent.ProjectileType<SlateChunk>(), damage / 3, 1, Main.myPlayer, 0, 2);
            }
            return true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            //For the record, this is incredibly stupid. But, it works

            Texture2D testTex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/SlateSet/SlateSigil").Value; ;

            //Main.GameZoomTarget gets the player's zoom (ranges from 1-2)
            //We need this b/c for some reason the scale of the drawn texture is based off the zoom as well as the adjusted position
            //Main.NewText(Main.GameZoomTarget);

            if (Main.player[Main.myPlayer].velocity == Vector2.Zero && Main.player[Main.myPlayer].inventory[Main.player[Main.myPlayer].selectedItem].type == ModContent.ItemType<SlateBow>())
            {
                fadeIn = Math.Clamp(MathHelper.Lerp(fadeIn, 1.5f, 0.01f), 0f, 1f);

                if (Math.Sin(timer / 120) >= 0)
                {
                    sigilColor = Color.Lerp(sigilColor, Color.DarkGreen, 0.008f);
                }
                else
                    sigilColor = Color.Lerp(sigilColor, Color.LightGreen, 0.008f);

                //Color sigilColor = Color.Lerp(Color.Green, Color.DarkGreen, (float)Math.Sin(timer / 70) * 0.1f);
                //Main.NewText(sigilColor);

                Main.spriteBatch.Draw(testTex, Main.player[Main.myPlayer].Center - Main.screenPosition + new Vector2(0, 45 * Main.GameZoomTarget), testTex.Frame(1, 4, 0, myFrame), sigilColor * fadeIn, 0, testTex.Size() / 2, 1f * Main.GameZoomTarget, SpriteEffects.None, 0f);
                Lighting.AddLight(Main.player[Main.myPlayer].Center + new Vector2(10, 0), new Vector3(sigilColor.R * fadeIn, sigilColor.G * fadeIn, sigilColor.B * fadeIn) * 0.0025f);

                //Main.autoPause

                if (timer % 20 == 0 && timer != 0)
                {
                    //Dust d = GlowDustHelper.DrawGlowDustPerfect(player.Center - aim.ToRotationVector2() * 35, ModContent.DustType<GlowCircleDust>(), Vector2.One.RotatedBy(dustRot) * (Main.rand.NextFloat(3) + 1),
                        //Color.HotPink, 0.40f + Main.rand.NextFloat(0,0.2f), 0.7f, 0f, dustShader); // 0.6

                    Vector2 offset = new Vector2(Main.rand.NextFloat(-20,21), 0);
                    float DustScale = Main.rand.NextFloat(.15f, 0.35f);
                    //Dust d = Dust.NewDustPerfect(Main.player[Main.myPlayer].Center + offset, ModContent.DustType<RisingSmokeDust>(), Scale: DustScale, newColor: sigilColor);
                    //d.shader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/TestShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic").UseColor(sigilColor);
                }

                if (timer % 8 == 0)
                {
                    myFrame++;
                    if (myFrame == 4) myFrame = 0;

                }
                timer++;
            }
            else
            {
                fadeIn = 0;
                timer = 0;
            }

            return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }
    }
}
*/