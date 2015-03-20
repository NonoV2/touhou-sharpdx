﻿using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace touhou_test
{
    class BulletObject : GameObject
    {

        public bool isActive = false;
        public float speed = 0f;
        float targetX = 0f;
        float targetY = 0f;
        float vectorX = 0f;
        float vectorY = 0f;
        float norm = 0f;
        int fps = 60;

        public BulletObject(ShaderResourceView resourceView, GameLogic gl) : base(resourceView, gl)
        {
            
        }

        public void trackAndFollow(float pX, float pY)
        {
            trackPlayerData(pX, pY);
            follow();
        }

        public void follow()
        {
            fps = 60; // base.gl.ghSharpDX.fpsCounter.FPS;

            //trackPlayerData(pX, pY);
            //if (Math.Abs(base.originX - targetX) < 3f && Math.Abs(base.originY - targetY) < 3f) return;
            base.originX = base.originX - vectorX / fps * speed;
            base.originY = base.originY - vectorY / fps * speed;

            //Stabilization
            
        }

        public void trackPlayerData(float pX, float pY) {
            targetX = pX;
            targetY = pY;
            norm = calculateNorm();
            vectorX = (base.originX - targetX) / (float)Math.Sqrt(norm);
            vectorY = (base.originY - targetY) / (float)Math.Sqrt(norm);
        }

        private float calculateNorm()
        {
            return (base.originX - targetX) * (base.originX - targetX) +
                   (base.originY - targetY) * (base.originY - targetY);
        }

        public override void Draw()
        {
            if (isActive) base.Draw();
        }

    }
}