﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;

namespace AerovelenceMod.Content.NPCs
{
    public class MovingPlatform : CollideableNPC
    {
        //public override void SetStaticDefaults() => DisplayName.SetDefault("PLATFORM TEST");
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanBeHitByProjectile(Projectile projectile) => false;
        public override bool? CanBeHitByItem(Player player, Item item) => false;
        public override bool CheckActive() => false;
        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.lifeMax = 10;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.netAlways = true;
        }

        Vector2 prevPos;
        public override void AI()
        {
            NPC.velocity = new Vector2(1,0);
            
            base.AI();

            float yDistTraveled = NPC.position.Y - prevPos.Y;
            foreach (Player player in Main.player)
            {
                if (!player.active || player.dead || player.GoingDownWithGrapple || player.GetModPlayer<AeroPlayer>().PlatformTimer > 0)
                    continue;

                Rectangle playerRect = new Rectangle((int)player.position.X, (int)player.position.Y + (player.height), player.width, 1);
                Rectangle npcRect = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, 8 + (player.velocity.Y > 0 ? (int)player.velocity.Y : 0) + (int)Math.Abs(yDistTraveled));

                if (playerRect.Intersects(npcRect) && player.position.Y <= NPC.position.Y)
                {
                    if (!player.justJumped && player.velocity.Y >= 0)
                    {
                        player.velocity.Y = 0;
                        //player.position.X = NPC.position.X;
                        player.position.Y = NPC.position.Y - player.height + 4;
                        player.position += (NPC.velocity * 0);
                    }
                }
            }

            prevPos = NPC.position;
        }
    }
}