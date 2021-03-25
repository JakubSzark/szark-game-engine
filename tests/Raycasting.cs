using System;
using Szark.Graphics;
using Szark.Math;

namespace Example
{
    public class Raycasting : Szark.Game
    {
        static readonly string[] map = new string[] {
            "XXXXXXXXXX",
            "X--------X",
            "X--P-----X",
            "X--------X",
            "X--------X",
            "XXXXXXXXXX",
        };

        private const float MAX_DIST = 10f;
        private const float RAY_INCR = 0.1f;
        private const float FOV = 70f;

        private Vector playerPos;
        private float lookAngle = 45f;

        private float rotation = 0f;
        private float time = 0f;
        private float currentFPS = 0f;

        public Raycasting() : base("Raycasting", 1280, 720, 4, false) { }

        protected override void OnCreated()
        {
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[i].Length; j++)
                {
                    if (map[i][j] == 'P')
                        playerPos = new Vector(j, i);
                }
            }
        }

        protected override void OnRender(Canvas gfx, float deltaTime)
        {
            for (int col = 0; col < ScreenWidth; col++)
            {
                float colNorm = (col - (ScreenWidth * 0.5f)) / ScreenWidth;
                float angle = ((colNorm * FOV) + lookAngle) * Mathf.DEG2RAD;

                Vector lookDir = new Vector((float)Math.Cos(angle),
                    (float)Math.Sin(angle));

                float dist = RAY_INCR;

                while (dist < MAX_DIST)
                {
                    Vector ray = playerPos + (lookDir * dist);

                    ray.X = (float)Math.Round(ray.X);
                    ray.Y = (float)Math.Round(ray.Y);

                    if (map[(int)ray.Y][(int)ray.X] == 'X')
                        break;

                    dist += RAY_INCR;
                }

                float distNorm = dist / MAX_DIST;
                float centerDist = distNorm * ScreenHeight * 0.5f;
                byte distVal = (byte)((1 - distNorm) * 255.0f);

                for (int row = 0; row < ScreenHeight; row++)
                {
                    // Draw the World!
                    if (row >= centerDist && row <= ScreenHeight - centerDist)
                    {
                        gfx.Draw(col, row, new Color(distVal, distVal, distVal));
                    }
                    else if (row < centerDist)
                    {
                        gfx.Draw(col, row, new Color(135, 206, 235));
                    }
                    else
                    {
                        gfx.Draw(col, row, new Color(0, 154, 23));
                    }
                }
            }

            // Update the Current Framerate
            if ((time += deltaTime) >= 1.0f)
            {
                currentFPS = (float)Math.Floor(1f / deltaTime);
                time = 0;
            }

            // Rotate the Player in a Circle
            if ((rotation += (10f * deltaTime)) >= 360.0f)
                rotation = 0f;

            lookAngle = rotation;

            // Draw the FPS in the Upper Left Corner
            gfx.DrawString(0, 0, $"{currentFPS}", Color.Magenta);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}