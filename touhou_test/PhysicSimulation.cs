using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace touhou_test
{
    class PhysicSimulation // TODO fix issue and implement again...
    {

        public GameLogic gl;
        public Thread mainThread;

        public bool kill = false;
        public long physicTicks = 0;
        public long oldTick = 0;
        public long newTick = 0;
        public List<long> averageTick;
        public long tick = 0;
        public long realTick = 0;

        public PhysicSimulation(GameLogic gl, Thread mainThread)
        {
            this.gl = gl;
            this.mainThread = mainThread;
            averageTick = new List<long>();
        }

        private void setupReferenceTimer()
        {
            System.Timers.Timer fpsTimer = new System.Timers.Timer();
            //System.Windows.Forms.Timer fpsTimer = new System.Windows.Forms.Timer();
            fpsTimer.Elapsed += calculateTicks; //new ElapsedEventHandler(calculateFPS);
            //fpsTimer.Tick += calculateFPS;
            fpsTimer.Interval = 8d; // 1000 ms is one second
            fpsTimer.Start();
        }

        void calculateTicks(object sender, EventArgs e)
        {
            oldTick = newTick;
            newTick = physicTicks;

            foreach (GameObject go in gl.listBackgroundObject) { }
            foreach (GameObject go in gl.listGameObject)
            {
                go.originX = go.originX + (go.speedX / 64);
                go.originY = go.originY + (go.speedY / 64);
            }
            foreach (BulletObject go in gl.listBulletObject) { }
            //foreach (GameObject go in gl.listHitbox) { }

        }

        public void start()
        {
            setupReferenceTimer();
            while (!kill)
            {
                if (!mainThread.IsAlive) kill = true;
                physicTicks++;
            }
        }

    }
}
