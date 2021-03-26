using AerovelenceMod;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using System.Collections.Generic;
using System.Linq;
using System;
using static Terraria.ModLoader.ModContent;
using System.Reflection;

namespace AerovelenceMod.Core.Prim
{
    class LegPrimTrail : PrimTrail
    {
        public LegPrimTrail(Projectile projectile) : base(projectile)
        {
            _projectile = projectile;
        }
        public override void SetDefaults()
        {
             _points.Add(_projectile.Center);
            _cap = 80;
             _alphaValue = 0.7f;
            _width = 8;
            _color = Color.White;
        }
        public override void PrimStructure(SpriteBatch spriteBatch)
        {
            /*if (_noOfPoints <= 1) return; //for easier, but less customizable, drawing
            float colorSin = (float)Math.Sin(_counter / 3f);
            Color c1 = Color.Lerp(Color.White, Color.Cyan, colorSin);
            float widthVar = (float)Math.Sqrt(_points.Count) * _width;
            DrawBasicTrail(c1, widthVar);*/

            if (_noOfPoints <= 6) return;
            if (_destroyed) return;
            float widthVar;
            for (int i = 0; i < _points.Count; i++)
            {
                if (i == 0)
                {
                    widthVar =  _width;
                    Color c1 = Color.White;
                    Vector2 normalAhead = CurveNormal(_points, i + 1);
                    Vector2 secondUp = _points[i + 1] - normalAhead * widthVar;
                    Vector2 secondDown = _points[i + 1] + normalAhead * widthVar;
                    AddVertex(_points[i], c1 * _alphaValue, new Vector2(0, 0.5f));
                    AddVertex(secondUp, c1 * _alphaValue, new Vector2(1 / (float)_points.Count, 0));
                    AddVertex(secondDown, c1 * _alphaValue, new Vector2(1 / (float)_points.Count, 1));
                }
                else
                {
                    if (i != _points.Count - 1)
                    {
                        widthVar = _width;
                        Color c = Color.Cyan;
                        Color CBT = Color.Cyan;
                        Vector2 normal = CurveNormal(_points, i);
                        Vector2 normalAhead = CurveNormal(_points, i + 1);
                        float j = (((float)_points.Count) + ((float)(Math.Sin(_counter / 10f)) * 1) - i * 0.1f) / ((float)_points.Count);
                        widthVar *= j;
                        Vector2 firstUp = _points[i] - normal * widthVar;
                        Vector2 firstDown = _points[i] + normal * widthVar;
                        Vector2 secondUp = _points[i + 1] - normalAhead * widthVar;
                        Vector2 secondDown = _points[i + 1] + normalAhead * widthVar;

                        AddVertex(firstDown, c * _alphaValue, new Vector2((i / ((float)_points.Count)), 1));
                        AddVertex(firstUp, c * _alphaValue, new Vector2((i / ((float)_points.Count)), 0));
                        AddVertex(secondDown, CBT * _alphaValue, new Vector2((i + 1) / ((float)_points.Count), 1));

                        AddVertex(secondUp, CBT * _alphaValue, new Vector2((i + 1) / ((float)_points.Count), 0));
                        AddVertex(secondDown, CBT * _alphaValue, new Vector2((i + 1) / ((float)_points.Count), 1));
                        AddVertex(firstUp, c * _alphaValue, new Vector2((i / ((float)_points.Count)), 0));
                    }
                    else
                    {

                    }
                }
            }
        }
       public override void SetShaders()
        {
            PrepareShader(AerovelenceMod.LegElectricity, "MainPS", _noOfPoints, Color.Red);
        }
        public override void OnUpdate()
        {
            _counter++;
            _noOfPoints = _points.Count() * 6;
            if (_cap < _noOfPoints / 6)
            {
                _points.RemoveAt(0);
            }
            if ((!_projectile.active && _projectile != null) || _destroyed)
            {
                OnDestroy();
            }
            else
            {
                _points.Add(_projectile.Center);
            }
        }
        public override void OnDestroy()
        {
            _destroyed = true;
            _width *= 0.95f;
            if (_width < 0.05f)
            {
                Dispose();
            }
        }
    }
}