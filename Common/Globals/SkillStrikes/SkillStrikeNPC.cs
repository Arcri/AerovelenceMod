using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using tModPorter;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Common.Globals.SkillStrikes
{
	public class SkillStrikeOldNPC : GlobalNPC
	{
        public override bool InstancePerEntity => true;

        public bool strikeCTRemove = true;
        public bool PendingSkillStrike { get; set; } = false;
        public bool IsCrit { get; set; } = false;
        public int DamageAmount { get; set; } = 0;
        public Vector2 HitPosition { get; set; } = Vector2.Zero;
        public float ImpactScale { get; set; } = 1f;
        public SkillStrikeImpactType ImpactType { get; set; } = SkillStrikeImpactType.Basic;

        public bool DirectStrike { get; set; } = false;

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (PendingSkillStrike)
            {
                modifiers.HideCombatText();
            }
        }


        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (PendingSkillStrike)
            {
                int damage = damageDone > 0 ? damageDone : DamageAmount;
                CreateSkillStrikeText(npc, damage, IsCrit, HitPosition);
                PendingSkillStrike = false;
            }
            /*
            int recent = -1;
            for (int i = 99; i >= 0; i--)
            {
                CombatText ctToCheck = Main.combatText[i];
                if (ctToCheck.lifeTime == 60 || ctToCheck.lifeTime == 120)
                {
                    if (ctToCheck.alpha == 1f)
                    {
                        if ((ctToCheck.color == CombatText.DamagedHostile || ctToCheck.color == CombatText.DamagedHostileCrit))
                        {
                            recent = i;
                            i = 0;
                        }
                    }
                }
            }
            CombatText anchor = Main.combatText[recent];
            anchor.color = Color.White * 0f;
            int a = Projectile.NewProjectile(null, anchor.position, anchor.velocity, ProjectileType<SkillStrikeProj>(), 0, 0, Main.myPlayer, recent, recent);
            if (Main.projectile[a].ModProjectile is SkillStrikeProj SS)
            {
                SS.damageNumber = anchor.text;
                SS.skillCrit = false;
                SS.superCrit = false;
            }
            */
            base.OnHitByItem(npc, player, item, hit, damageDone);
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (PendingSkillStrike && !projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike)
            {
                int damage = damageDone > 0 ? damageDone : DamageAmount;
                CreateSkillStrikeText(npc, damage, IsCrit, HitPosition);
                PendingSkillStrike = false;
            }
            //Main.NewText("OnHitByProjectile");

            #region old
            /*
            int recent = -1;
            for (int i = 99; i >= 0; i--)
            {
                CombatText ctToCheck = Main.combatText[i];
                if (ctToCheck.lifeTime == 60 || ctToCheck.lifeTime == 120)
                {
                    if (ctToCheck.alpha == 1f)
                    {
                        if ( (ctToCheck.color == CombatText.DamagedHostile || ctToCheck.color == CombatText.DamagedHostileCrit) )
                        {
                            recent = i;
                            break;
                        }
                    }
                }
            }

            if (projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike)
            {
                if (recent == -1)
                {
                    Main.NewText("Skill Crit -1 (You shouldn't see this)");
                    return;
;               }
                if (hit.Crit)
                    Main.combatText[recent].color = Color.Purple * 0f;
                else
                    Main.combatText[recent].color = Color.Gold * 0f;

                CombatText anchor = Main.combatText[recent];
                int a = Projectile.NewProjectile(null, anchor.position, anchor.velocity, ProjectileType<SkillStrikeProj>(), 0, 0, Main.myPlayer, recent, recent);
                if (Main.projectile[a].ModProjectile is SkillStrikeProj SS)
                {
                    SS.damageNumber = anchor.text;
                    SS.skillCrit = true;
                    SS.superCrit = hit.Crit;
                }
                //Main.NewText("Spawned proj with CT index: " + recent);
            } else
            {

            }
            */
            #endregion
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile proj, ref NPC.HitModifiers modifiers)
        {
            if (PendingSkillStrike && !proj.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike)
            {
                modifiers.HideCombatText();
            }
        }

        public override void OnKill(NPC npc)
        {
            if (PendingSkillStrike)
            {
                CreateSkillStrikeText(npc, DamageAmount, IsCrit, HitPosition);
                PendingSkillStrike = false;
            }
        }
        public override void PostAI(NPC npc)
        {
            if (DirectStrike && PendingSkillStrike)
            {
                CreateSkillStrikeText(npc, DamageAmount, IsCrit, HitPosition);
                PendingSkillStrike = false;
                DirectStrike = false;
            }
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (PendingSkillStrike)
            {
                modifiers.HideCombatText();
            }
            base.ModifyIncomingHit(npc, ref modifiers);
        }

        private void CreateSkillStrikeText(NPC npc, int damage, bool isCrit, Vector2 hitPosition)
        {
            if (damage <= 0) return;
            Vector2 randomSpawnPos = Main.rand.NextVector2FromRectangle(
                new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, (int)(npc.height * 0.75f)));

            Dust text = Dust.NewDustPerfect(randomSpawnPos, ModContent.DustType<SkillStrikeText>(),
                new Vector2(0f, -12f), Scale: 1f);

            SkillStrikeTextBehavior sstb = new SkillStrikeTextBehavior();
            sstb.isCrit = isCrit;
            sstb.damageNumber = "" + damage;

            text.customData = sstb;
        }
    }
}