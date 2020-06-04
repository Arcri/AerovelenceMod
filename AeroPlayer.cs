using AerovelenceMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod
{
    public class AeroPlayer : ModPlayer
    {
        public bool SoulFig;
        public bool KnowledgeFruit;
        public bool DevilsBounty;

		public bool SoulFire;
        public bool badHeal;

        public bool QueensStinger;
        public bool EmeraldEmpoweredGem;
        public bool MidasCrown;
        public static bool AdobeHelmet;

        public static bool Setbonus = false;

        private Texture2D originalHeartTexture;
        private Texture2D originalManaTexture;

        public override void Initialize()
        {
            SoulFig = false;
            KnowledgeFruit = false;
            DevilsBounty = false;
            Setbonus = false;
            AdobeHelmet = false;

            MidasCrown = false;
            EmeraldEmpoweredGem = false;
            QueensStinger = false;
        }

        public override void UpdateDead()
        {
            SoulFire = false;
            badHeal = false;
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (QueensStinger)
            {
                if (proj.type != 181)
                    if (Main.rand.NextBool(10)) //  1 in 10 chance
                        Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ProjectileID.Bee, 3, 2, player.whoAmI);
            }
            if (EmeraldEmpoweredGem)
            {
                target.AddBuff(39, 40, false);
            }
            if (MidasCrown)
            {
                target.AddBuff(BuffID.Midas, 900, false);
            }
        }

        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (SoulFire)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, DustType<WispDust>(), player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
                r *= 0.1f;
                g *= 0.2f;
                b *= 0.7f;
                fullBright = true;
            }
        }

        public override void ResetEffects()
        {
            Setbonus = false;
            badHeal = false;
        }

              public static readonly PlayerLayer MiscEffects = new PlayerLayer("ExampleMod", "MiscEffects", PlayerLayer.MiscEffectsFront, delegate (PlayerDrawInfo drawInfo) {
            if (drawInfo.shadow != 0f)
            {
                return;
            }
            Player drawPlayer = drawInfo.drawPlayer;
            Mod mod = ModLoader.GetMod("ExampleMod");
            AeroPlayer modPlayer = drawPlayer.GetModPlayer<AeroPlayer>();
            if (modPlayer.badHeal)
            {
                Texture2D texture = mod.GetTexture("Buffs/Skull");
                int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X);
                int drawY = (int)(drawInfo.position.Y - 4f - Main.screenPosition.Y);
                DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Lighting.GetColor((int)((drawInfo.position.X + drawPlayer.width / 2f) / 16f), (int)((drawInfo.position.Y - 4f - texture.Height / 2f) / 16f)), 0f, new Vector2(texture.Width / 2f, texture.Height), 1f, SpriteEffects.None, 0);
                Main.playerDrawData.Add(data);
                for (int k = 0; k < 2; k++)
                {
                    int dust = Dust.NewDust(new Vector2(drawInfo.position.X + drawPlayer.width / 2f - texture.Width / 2f, drawInfo.position.Y - 4f - texture.Height), texture.Width, texture.Height, DustType<Smoke>(), 0f, 0f, 0, Color.Black);
                    Main.dust[dust].velocity += drawPlayer.velocity * 0.25f;
                    Main.playerDrawDust.Add(dust);
                }
            }
        });


        public override TagCompound Save()
        {
            List<string> list = new List<string>();

            ResetResourceTextures();

            if (SoulFig)
            {
                list.Add("SoulFig");
            }
            if (KnowledgeFruit)
            {
                list.Add("SoulFig");
            }
            if (DevilsBounty)
            {
                list.Add("DevilsBounty");
            }
            return new TagCompound
            {
                {
                    "perm",
                    list
                }
            };
        }

        public override void Load(TagCompound tag)
        {
            SetOriginalResourceTextures();

            IList<string> list = tag.GetList<string>("perm");
            SoulFig = list.Contains("SoulFig");
            KnowledgeFruit = list.Contains("KnowledgeFruit");
            DevilsBounty = list.Contains("DevilsBounty");
        }

        public override void LoadLegacy(BinaryReader reader)
        {
            int num = reader.ReadInt32();
            if (num == 0)
            {
                BitsByte bitsByte = reader.ReadByte();
                SoulFig = bitsByte[0];
                KnowledgeFruit = bitsByte[1];
                DevilsBounty = bitsByte[2];
            }
        }

        public override void PreUpdate()
        {
            SetResourceTexturesBasedOnModPowerups();
        }

        public bool bossPresent
        {
            get
            {
                return Main.npc.ToList().Any(npc => npc.boss & npc.active);
            }
        }

        public NPC GetFarthestBoss
        {
            get
            {
                //Make sure that when checking for bosses you also make sure they're active
                List<NPC> npcList = Main.npc.ToList();
                List<NPC> bosses = npcList.Where(npc => npc.boss & npc.active).Where(me => Vector2.Distance(me.Center, player.Center) == npcList.Where(npc => npc.boss & npc.active).Max(boss => Vector2.Distance(boss.Center, player.Center)) & me.active).ToList();
                return bosses[0];
            }
        }

        public override void PostUpdateMiscEffects()
        {
            player.statLifeMax2 += (SoulFig ? 50 : 0) + (KnowledgeFruit ? 50 : 0) + (DevilsBounty ? 50 : 0);
        }

        /// <summary>
        /// Resets vanilla resource textures to their defaults.
        /// </summary>
        private void ResetResourceTextures()
        {
            Main.heart2Texture = originalHeartTexture;
            Main.manaTexture = originalManaTexture;
        }

        /// <summary>
        /// Only call this in ModPlayer.Load().
        /// </summary>
        private void SetOriginalResourceTextures()
        {
            originalHeartTexture = Main.heart2Texture;
            originalManaTexture = Main.manaTexture;
        }

        /// <summary>
        /// Sets resource textures based on the players consumed mod powerups
        /// </summary>
        private void SetResourceTexturesBasedOnModPowerups()
        {
            int heartTextureControl = (SoulFig ? 1 : 0) + (KnowledgeFruit ? 1 : 0) + (DevilsBounty ? 1 : 0);

            switch (heartTextureControl)
            {
                case 1:
                    Main.heart2Texture = mod.GetTexture("ExtraTextures/SoulFigHeart");
                    break;
                case 2:
                    Main.heart2Texture = mod.GetTexture("ExtraTextures/KnowledgeFruitHeart");
                    break;
                case 3:
                    Main.heart2Texture = mod.GetTexture("ExtraTextures/DevilsBountyHeart");
                    break;
                default:
                    break;
            }
        }
    }
}
