using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Items.Weapons.Aurora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.FeatheredFoe
{
    public partial class FeatheredFoe : ModNPC
    {
        public const string AssetDirectory = "AerovelenceMod/Content/NPCs/Bosses/FeatheredFoe/Assets/";
        public override string Texture => "Terraria/Images/Projectile_0";


        private enum FeatheredFoeState
        {
            BasicAttack = 0,
            SwoopFeatherBehind = 1,
            FiveSpread = 2,
            MartletOrbitFeather = 3,
            CircleBurstFeather = 4,
            SwirlFeather = 5,
            CornerTravelShot = 6
        }

        private FeatheredFoeState CurrentAttack
        {
            get => (FeatheredFoeState)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }

        public override void SetStaticDefaults() { }

        public override bool CheckActive() { return false; }

        public override void SetDefaults()
        {
            NPC.lifeMax = 4000;
            NPC.width = 80;
            NPC.height = 80;
            NPC.damage = 0;

            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;

            NPC.aiStyle = -1;
            NPC.knockBackResist = 0;
            NPC.npcSlots = 10;
        }
        public Player player => Main.player[NPC.target];


        public int timer = 0;
        public int substate = 1;
        public override void AI()
        {
            NPC.dontTakeDamage = false;
            NPC.hide = false;

            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }

            CurrentAttack = FeatheredFoeState.FiveSpread;

            switch (CurrentAttack)
            {
                case FeatheredFoeState.BasicAttack:
                    BasicAttack();
                    break;
                case FeatheredFoeState.SwoopFeatherBehind:
                    SwoopFeatherBehind();
                    break;
                case FeatheredFoeState.FiveSpread:
                    FiveSpread();
                    break;
                case FeatheredFoeState.MartletOrbitFeather:
                    MartletOrbitFeather();
                    break;
                case FeatheredFoeState.CircleBurstFeather:
                    CircleBurstFeather();
                    break;
                case FeatheredFoeState.SwirlFeather:
                    SwirlFeather();
                    break;
                case FeatheredFoeState.CornerTravelShot:
                    CornerTravelShot();
                    break;
            }

            timer++;
        }

    }
}
