using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.World.Generation;

namespace AerovelenceMod.Common.Utilities
{
    public static class ExtensionMethods
    {
        /// <summary>
        ///     Attempts to insert a <see cref="GenPass" /> task into a <see cref="List{T}" /> of <see cref="GenPass" />es.
        /// </summary>
        /// <param name="tasks">The list of GenPass tasks being added to.</param>
        /// <param name="index">The index of the GenPass task we're using for our <paramref name="additive" />.</param>
        /// <param name="item">The item actually being inserted.</param>
        /// <param name="additive">What we're adding to the index for our insertion, this can be a negative.</param>
        /// <returns></returns>
        public static bool TryInsert<T>(this List<T> tasks, int index, T item, int additive = 1) where T : class
        {
            if (index == -1)
                return false;

            tasks.Insert(index + additive, item);
            return true;
        }

        /// <summary>
        ///     Automatically sets an item's size.
        /// </summary>
        public static Vector2 Autosize(this Item item)
        {
            Texture2D texture = Main.itemTexture[item.type];

            if (texture == null)
                return Vector2.Zero;

            return item.Size = Main.itemAnimationsRegistered.Contains(item.type)
                ? new Vector2(texture.Width, texture.Height / Main.itemAnimations[item.type].FrameCount)
                : texture.Size();
        }
    }
}