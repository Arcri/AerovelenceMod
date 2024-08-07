﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using AerovelenceMod.Content.NPCs;

namespace AerovelenceMod.Common
{
    public class CustomCollision
    {
        public static void Player_UpdateNPCCollision(Terraria.On_Player.orig_Update_NPCCollision orig, Player self)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                // We don't want to include Moving Platforms for collision because this detour will block the FallThrough needed
                if (npc.ModNPC is CollideableNPC && npc.active && npc.ModNPC != null && !(npc.ModNPC is MovingPlatform))
                {
                    Rectangle PlayerBottom = new Rectangle((int)self.position.X, (int)self.position.Y + self.height, self.width, 1);
                    Rectangle NPCTop = new Rectangle((int)npc.position.X, (int)npc.position.Y - (int)npc.velocity.Y, npc.width, 8 + (int)Math.Max(self.velocity.Y, 0));

                    if (PlayerBottom.Intersects(NPCTop))
                    {
                        if (self.position.Y <= npc.position.Y && !self.justJumped && self.velocity.Y >= 0)
                        {
                            self.gfxOffY = npc.gfxOffY;
                            self.position.Y = npc.position.Y - self.height + 4;
                            self.velocity.Y = 0;

                            self.position += npc.velocity;
                            self.fallStart = (int)(self.position.Y / 16f);

                            if (self == Main.LocalPlayer)
                                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Main.LocalPlayer.whoAmI);

                            orig(self);
                        }
                    }

                }
            }

            orig(self);
        }

        // Detour for moving platforms
        public static void Player_PlatformCollision(Terraria.On_Player.orig_SlopingCollision orig, Player self, bool fallThrough, bool ignorePlats)
        {
            if (self.GetModPlayer<AeroPlayer>().PlatformTimer > 0)
            {
                orig(self, fallThrough, ignorePlats);
                return;
            }

            if (self.GoingDownWithGrapple)
            {
                if (self.grapCount == 1)
                {
                    foreach (int PlayerGrappleIndex in self.grappling)
                    {
                        if (PlayerGrappleIndex < 0 || PlayerGrappleIndex > Main.maxProjectiles)
                            continue;

                        Projectile GrappleHook = Main.projectile[PlayerGrappleIndex];

                        foreach (NPC npc in Main.npc)
                        {
                            if (!npc.active || npc.ModNPC == null || !(npc.ModNPC is MovingPlatform))
                                continue;

                            if (GrappleHook.active && npc.Hitbox.Intersects(GrappleHook.Hitbox) && self.Hitbox.Intersects(GrappleHook.Hitbox))
                            {
                                self.position = GrappleHook.position + new Vector2(GrappleHook.width / 2 - self.width / 2, GrappleHook.height / 2 - self.height / 2);
                                self.position += npc.velocity;

                                self.velocity.Y = 0;
                                self.jump = 0;

                                self.fallStart = (int)(self.position.Y / 16f);
                            }
                        }
                    }
                }

                orig(self, fallThrough, ignorePlats);
                return;
            }

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active || npc.ModNPC == null || !(npc.ModNPC is MovingPlatform))
                    continue;

                Rectangle PlayerRect = new Rectangle((int)self.position.X, (int)self.position.Y + (self.height), self.width, 1);
                Rectangle NPCRect = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, 8 + (self.velocity.Y > 0 ? (int)self.velocity.Y : 0));

                if (self.grapCount == 1 && npc.velocity.Y != 0)
                {
                    //if the player is using a single grappling hook we can check if they are colliding with it and its embedded in the moving platform, while its changing Y position so we can give the player their jump back
                    foreach (int eachGrappleIndex in self.grappling)
                    {
                        if (eachGrappleIndex < 0 || eachGrappleIndex > Main.maxProjectiles)//somehow this can be invalid at this point?
                            continue;

                        Projectile grappleHookProj = Main.projectile[eachGrappleIndex];
                        if (grappleHookProj.active && npc.Hitbox.Intersects(grappleHookProj.Hitbox) && self.Hitbox.Intersects(grappleHookProj.Hitbox))
                        {
                            self.position = grappleHookProj.position + new Vector2(grappleHookProj.width / 2 - self.width / 2, grappleHookProj.height / 2 - self.height / 2);
                            self.position += npc.velocity;

                            self.velocity.Y = 0;
                            self.jump = 0;

                            self.fallStart = (int)(self.position.Y / 16f);
                        }
                    }
                }
                else if (PlayerRect.Intersects(NPCRect) && self.position.Y <= npc.position.Y)
                {
                    if (!self.justJumped && self.velocity.Y >= 0)
                    {
                        if (fallThrough) self.GetModPlayer<AeroPlayer>().PlatformTimer = 10;

                        self.position.Y = npc.position.Y - self.height + 4;
                        self.position += npc.velocity;

                        self.fallStart = (int)(self.position.Y / 16f);

                        self.velocity.Y = 0;
                        self.jump = 0;
                        self.gfxOffY = npc.gfxOffY;

                    }
                }
            }

            orig(self, fallThrough, ignorePlats);
        }
    }
}