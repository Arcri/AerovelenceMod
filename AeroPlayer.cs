using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Generation;
using Terraria.ModLoader.IO;
using System.Reflection;
using Terraria.Utilities;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using AerovelenceMod;

namespace AerovelenceMod
{
    public class AeroPlayer : ModPlayer
    {
        public bool fruit1;
        public bool fruit2;
        public bool fruit3;
		
        public bool QueensStinger;
        public bool EmeraldEmpoweredGem;
        public bool midasCrown;

        public override void Initialize()
		{
            fruit1 = false;
            fruit2 = false;
            fruit3 = false;
			
            midasCrown = false;
            EmeraldEmpoweredGem = false;
            QueensStinger = false;
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (QueensStinger)
            {
                if (proj.type != 181)
                    if (Main.rand.Next(9) == 0) //  1 in 10 chance
                    Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, 181, 3, 2, player.whoAmI);
            }
            if (EmeraldEmpoweredGem)
            {
                target.AddBuff(39, 40, false);
            }
            if (midasCrown)
            {
                target.AddBuff(BuffID.Midas, 900, false);
            }
		}
        public override TagCompound Save()
        {
            List<string> list = new List<string>();
            if (fruit1)
            {
                list.Add("fruit1");
            }
            if (fruit2)
            {
                list.Add("fruit1");
            }
            if (fruit3)
            {
                list.Add("fruit3");
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
            IList<string> list = tag.GetList<string>("perm");
            fruit1 = list.Contains("fruit1");
            fruit2 = list.Contains("fruit2");
            fruit3 = list.Contains("fruit3");
        }
        public override void LoadLegacy(BinaryReader reader)
        {
            int num = reader.ReadInt32();
            if (num == 0)
            {
                BitsByte bitsByte = reader.ReadByte();
                fruit1 = bitsByte[0];
                fruit2 = bitsByte[1];
                fruit3 = bitsByte[2];
            }
        }
        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            switch ((fruit1 ? 1 : 0) + (fruit2 ? 1 : 0) + (fruit3 ? 1 : 0))
            {
                case 1:
                    Main.heart2Texture = mod.GetTexture("ExtraTextures/Heart2");
                    break;
                case 2:
                    Main.heart2Texture = mod.GetTexture("ExtraTextures/Heart3");
                    break;
                case 3:
                    Main.heart2Texture = mod.GetTexture("ExtraTextures/Heart4");
                    break;
                default:
                    Main.heart2Texture = mod.GetTexture("ExtraTextures/HeartOriginal");
                    break;
            }
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
            player.statLifeMax2 += (fruit1 ? 50 : 0) + (fruit2 ? 50 : 0) + (fruit3 ? 50 : 0);
        }
    }
}
