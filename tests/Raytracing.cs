using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Szark.Graphics;
using Szark.Math;

namespace Example
{
    abstract class Renderable
    {
        public Vec3 Position;
        public Material Material;

        public abstract bool Intersecting(Vec3 point);
        public abstract Vec3 GetNormal(Vec3 point);
    }

    class Sphere : Renderable
    {
        public float Radius;

        public override Vec3 GetNormal(Vec3 point) =>
            (point - Position).Normalized();

        public override bool Intersecting(Vec3 point) =>
            Vec3.Distance(Position, point) < Radius;
    }

    struct Material
    {
        public Color Color;
    }

    struct Ray
    {
        public Vec3 Origin;
        public Vec3 Direction;
    }

    struct RayHit
    {
        public Vec3 Point;
        public Renderable Renderable;
        public float Distance;
    }

    public class Raytracing : Szark.Game
    {
        public Raytracing() : base("Raytracing", 800, 800, 2, false) { }

        const float STEP = 0.05f;
        const float MAX_DIST = 10.0f;
        const int SUB_DIVISIONS = 2;

        private float framerate = 0f;
        private float time = 0f;

        private Vec3 lightDir = new Vec3(0, -1, -1).Normalized();

        private List<Renderable> scene = new()
        {
            new Sphere()
            {
                Position = new Vec3(0, 0, 3),
                Radius = 1f,
                Material = new Material()
                {
                    Color = Color.Blue,
                },
            },
            new Sphere()
            {
                Position = new Vec3(2, 1, 2),
                Radius = 0.5f,
                Material = new Material()
                {
                    Color = Color.Red,
                },
            },
            new Sphere()
            {
                Position = new Vec3(-2, 1, 5),
                Radius = 0.3f,
                Material = new Material()
                {
                    Color = Color.Green,
                },
            }
        };

        private Ray camera = new Ray()
        {
            Origin = new Vec3(0, 0, -1),
            Direction = new Vec3(0, 0, 1),
        };

        private Random random = new Random();

        protected override void OnCreated()
        {
            VSync = false;
        }

        protected override void OnRender(Canvas gfx, float deltaTime)
        {
            if ((time += deltaTime) >= 0.25f)
            {
                framerate = (float)Math.Floor(1f / deltaTime);
                time = 0;
            }

            int regionWidth = (int)ScreenWidth / SUB_DIVISIONS;
            int regionHeight = (int)ScreenHeight / SUB_DIVISIONS;

            for (int i = 0; i < SUB_DIVISIONS; i++)
            {
                for (int j = 0; j < SUB_DIVISIONS; j++)
                {
                    RenderRegion(
                        gfx,
                        regionWidth * i,
                        regionHeight * j,
                        regionWidth,
                        regionHeight
                    );
                }
            }

            gfx.DrawString(0, 0, $"{framerate}", Color.Green);
        }

        private void RenderRegion(Canvas canvas, int x0, int y0, int width, int height)
        {
            Vec3 perp = camera.Direction.GetPerpendicular().Normalized();

            for (int col = x0; col < x0 + width; col++)
            {
                float xP = (((float)col / ScreenWidth) * 2f) - 1f;

                for (int row = y0; row < y0 + height; row++)
                {
                    if (random.Next(10) == 0)
                    {
                        float yP = (((float)row / ScreenHeight) * 2f) - 1f;

                        Vec3 up = perp * -yP;
                        Vec3 left = (camera.Direction % perp).Normalized() * xP;

                        var rayHit = RayTrace(new Ray()
                        {
                            Origin = camera.Origin,
                            Direction = (camera.Direction + left + up).Normalized()
                        });

                        if (rayHit is RayHit hit)
                        {
                            float fog = 1 - (hit.Distance / MAX_DIST);
                            Color diffuse = Diffuse(hit) * fog;
                            canvas.Draw(col, row, diffuse);
                        }
                        else
                        {
                            canvas.Draw(col, row, Color.Black);
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Color Diffuse(RayHit hit)
        {
            Vec3 normal = hit.Renderable.GetNormal(hit.Point);
            float diffuse = Mathf.Clamp(Vec3.Dot(normal, lightDir), 0, 1);
            return hit.Renderable.Material.Color * diffuse;
        }

        private RayHit? RayTrace(Ray ray)
        {
            float dist = STEP;

            while (dist < MAX_DIST)
            {
                Vec3 point = ray.Origin + (ray.Direction * dist);

                foreach (var renderable in scene)
                {
                    if (renderable.Intersecting(point))
                    {
                        return new RayHit()
                        {
                            Distance = dist,
                            Point = point,
                            Renderable = renderable,
                        };
                    }
                }

                dist += STEP;
            }

            return null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}