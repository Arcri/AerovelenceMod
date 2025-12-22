using AerovelenceMod.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using Terraria;

namespace AerovelenceMod.Common.Particles
{
    public enum ParticleType : int
    {
        FireParticle = 1,
    }

    //Use this instead of dust when we want to apply a shader to a particle
    public class ShaderParticle : Entity
    {
        public int particleID;
        public int particleType;

        public Vector2 Center;
        public Vector2 Velocity;
        public float Rotation;
        public float Scale;
        public float Alpha;

        //How many ticks this particle has been active
        public uint Timer;

        //Should this particle kill at a certain time
        public bool shouldKillEarly;
        public int timeToKillEarly;


        public RenderLayer renderLayer;

        public Effect myShader;

        /// <summary>
        /// 
        /// </summary>
        public virtual void Update() { }

        public virtual void Draw(SpriteBatch spriteBatch) { }
        public virtual void DrawWithShader(SpriteBatch spriteBatch, Effect effect) { }

        public void Kill() => ShaderParticleHandler.RemoveParticle(this);
    }
}
