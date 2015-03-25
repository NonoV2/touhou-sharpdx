using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Timers;
//SharpDX
using SharpDX;
using SharpDX.Windows;
using System.Threading;


namespace touhou_test
{
    class Game
    {

        //Class Fields:
        public long avgFPS = 0;
        public long previousAvgFPS = 0;
        public long currentFrameDelta = 0;
        public long totalNumberFrames = 0;
        public long lastFrames = 0;
        public long currentFrames = 0;
        public bool stableFPS = false;

        public enum GRAPHIC : int { SLIMDX = 0, SHARPDX = 1};
        public int graphicLib = (int)GRAPHIC.SHARPDX;

        public GameLogic gl;
        public GraphicHandlerSharpDX ghSharpDX;
        public InputHandlerSharpDX ihSharpDX;
        //public PhysicSimulation ps;

        /*
        public enum GAMESTATE : int { MENU = 0, PLAYING = 1, DEMO = 2 };
        public int gameState = (int)GAMESTATE.DEMO;
        public int collision = 0;
        public bool hitboxDisplay = false;

        public GameObject[] listGameObject;
        */
        public Game() {}

        //Main Entry Point
        public void Run() {

            RunSharpDX();
        
        }

        private void RunSharpDX() {

            ghSharpDX = new GraphicHandlerSharpDX(this);
            ghSharpDX.initRenderForm();
            ghSharpDX.initAllTexturesFromFiles();

            ihSharpDX = new InputHandlerSharpDX(this);
            ihSharpDX.initAllEventListener();

            gl = new GameLogic(this);
            gl.init();

            //Thread mt = Thread.CurrentThread;
            //ps = new PhysicSimulation(gl, mt);
            //Thread psThread = new Thread(new ThreadStart(ps.start));
            //psThread.Start();
            //while (!psThread.IsAlive) ;

            Thread.Sleep(100); //let some time for inits to finish
            ghSharpDX.fpsCounter.Reset();

            RenderLoop.Run(ghSharpDX.form, () =>
            {
                //resize if form was resized
                if (ghSharpDX.device.MustResize)
                {
                    //ghSharpDX.form.Width  = 800;
                    //ghSharpDX.form.Height = 600;
                    //ghSharpDX.form.Width = ghSharpDX.form.Width + (ghSharpDX.form.Width - ghSharpDX.form.ClientSize.Width);
                    //ghSharpDX.form.Height = ghSharpDX.form.Height + (ghSharpDX.form.Height - ghSharpDX.form.ClientSize.Height);
                    ghSharpDX.device.Resize();
                    ghSharpDX.batch.Resize();
                }
                //clear color
                ghSharpDX.device.Clear(SharpDX.Color.Transparent);
                
                //update logic
                gl.logicUpdate();
                
                //draw everything
                gl.drawAllGameObjects();

                //present - swapbuffers
                ghSharpDX.device.SwapChain.Present(1,SharpDX.DXGI.PresentFlags.None); // force v-sync with 1 seems like a good idea? not ideal, but less flickering.

                //update fps timer
                ghSharpDX.fpsCounter.Update();

            });
            ghSharpDX.batch.Dispose();
            ghSharpDX.Dispose();

        }

       

    //-------------------------------END---OF---CLASS----------------------------
    }
}
