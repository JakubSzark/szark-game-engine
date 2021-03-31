using Szark.Graphics;
using System.IO;
using System;

namespace Example
{
    public class RaytracingGPU : Szark.Game
    {
        public RaytracingGPU() : base("RaytracingGPU", 800, 800, 4, false) { }

        private Shader shader;
        private uint uCol;

        protected override void OnCreated()
        {
            // Setup Custom GPU Shader
            ErrorRecieved += s => throw new ApplicationException($"[Error]: {s}");
            string vertSrc = File.ReadAllText("resources/raytracing.vert");
            string fragSrc = File.ReadAllText("resources/raytracing.frag");
            shader = SetCustomShader(vertSrc, fragSrc);

            uCol = shader.GetLocation("uCol").GetValueOrDefault(0);
        }

        protected override void OnRender(Canvas canvas, float deltaTime)
        {
            shader.Send(uCol, new Color(0, 255, 0));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}