using SharpDX.Direct3D11;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace touhou_test
{
    class GameLogic
    {
        Game g;
        public GraphicHandlerSharpDX ghSharpDX;
        InputHandlerSharpDX ihSharpDX;

        public enum GAMESTATE : int { MENU = 0, PLAYING = 1, DEMO = 2 };
        public int gameState = (int)GAMESTATE.MENU;

        public int menuOption = 0;
        public int collision = 0;
        public bool hitboxDisplayNew = false;
        public float cameraOriginX = 0;
        public float cameraOriginY = 0;
        public bool printFPS = false;
        public bool wireframe = false;
        public bool printHelp = false;
        public bool printHelpLab = false;
        public bool visibleTextureCountIsActive = false;

        public int frameLogicCount = 0;
        public List<bool> timingEventDone;
        public int bulletCount = 0;
        public bool focus = false;
        public float delay = 0;
        public float acceleration = 0;

        public List<GameObject> listBackgroundObject;
        public List<GameObject> listPlayerObject;
        public List<GameObject> listGameObject;
        public List<BulletObject> listBulletObject;
        public List<GameObject> listHitbox;
        public List<GameObject> collisionObject;

        private void cleanGameState()
        {
            // Object cleaning
            listPlayerObject.Clear();
            listBulletObject.Clear();
            timingEventDone.Clear();
            listBackgroundObject.Clear();
            listGameObject.Clear();
            listHitbox.Clear();
            collisionObject.Clear();

            // Global variables cleaning
            collision = 0;
            hitboxDisplayNew = false;
            cameraOriginX = 0;
            cameraOriginY = 0;
            printFPS = false;
            wireframe = false;
            frameLogicCount = 0;
            bulletCount = 0;
            focus = false;
            delay = 0;
            printHelp = false;
            printHelpLab = false;
            acceleration = 0;
            visibleTextureCountIsActive = false;

            // Unknown due to saving states
            //menuOption = 0;
        }

        public GameLogic(Game g) {
            this.g = g;
            this.ghSharpDX = g.ghSharpDX;
            this.ihSharpDX = g.ihSharpDX;
        }

        public void init() {

            //init structure lists
            timingEventDone = new List<bool>();
            listBackgroundObject = new List<GameObject>();
            listPlayerObject = new List<GameObject>();
            listGameObject = new List<GameObject>();
            listBulletObject = new List<BulletObject>();
            listHitbox = new List<GameObject>();
            collisionObject = new List<GameObject>();

            //launch first menu
            //if (g.graphicLib == (int)Game.GRAPHIC.SLIMDX) loadMainMenuGameState();
            //else
            loadMainMenuGameState();

        }


        public void drawAllGameObjects()
        {
            //begin drawing
            if (wireframe) ghSharpDX.batch.Batch.Begin(SpriteSortMode.Immediate, ghSharpDX.bsToolkit, ghSharpDX.ssToolkit, null, ghSharpDX.rsToolkitWireframe);
            else ghSharpDX.batch.Batch.Begin(SpriteSortMode.Immediate, ghSharpDX.bsToolkit, ghSharpDX.ssToolkit, null, ghSharpDX.rsToolkit);
            //draw
            //if (listGameObject == null) return;
            foreach (GameObject go in listBackgroundObject)
            {
                ghSharpDX.drawGameObject(go);
            }
            foreach (GameObject go in listGameObject)
            {
                ghSharpDX.drawGameObject(go);
            }
            foreach (GameObject go in listPlayerObject) ghSharpDX.drawGameObject(go);

            foreach (BulletObject bo in listBulletObject) ghSharpDX.drawGameObject(bo);
            if (hitboxDisplayNew)
            {
                foreach (GameObject go in listHitbox)
                {
                    ghSharpDX.drawGameObject(go);
                }
            }

            drawAllText();

            //flush text to view
            ghSharpDX.batch.End();
            
        }

        private void drawAllText()
        {
            float wFactor = (float)ghSharpDX.form.ClientSize.Width / (float)ghSharpDX.windowW;
            float hFactor = (float)ghSharpDX.form.ClientSize.Height / (float)ghSharpDX.windowH;
            float fontMul = 1f;
            SharpDX.Vector2 fontSize = new SharpDX.Vector2(wFactor * fontMul, hFactor * fontMul);

            if (gameState == (int)GAMESTATE.MENU)
            {
                
                string danmaku = "Danmaku Demo";
                string lab = "Technical Test Demo";
                string quit = "Quit";
                
                //ghSharpDX.batch.DrawString(danmaku, 100, 400, SharpDX.Color.Black);
                ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, danmaku, new SharpDX.Vector2(100 * wFactor, 400 * hFactor), SharpDX.Color.Black,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
                //ghSharpDX.batch.DrawString(lab, 100, 450, SharpDX.Color.Black);
                ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, lab, new SharpDX.Vector2(100 * wFactor, 450 * hFactor), SharpDX.Color.Black,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
                ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, quit, new SharpDX.Vector2(100 * wFactor, 500 * hFactor), SharpDX.Color.Black,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
                if (menuOption == 0)
                {
                    ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, danmaku, new SharpDX.Vector2(97 * wFactor, 397 * hFactor), SharpDX.Color.White,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
                }
                if (menuOption == 1)
                {
                    ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, lab, new SharpDX.Vector2(97 * wFactor, 447 * hFactor), SharpDX.Color.White,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
                }
                if (menuOption == 2)
                {
                    ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, quit, new SharpDX.Vector2(97 * wFactor, 497 * hFactor), SharpDX.Color.White,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
                }
                
            }
            if (printFPS)
            {
                string msg = "FPS: " + ghSharpDX.fpsCounter.FPS + " Current Time " + DateTime.Now.ToString();
                ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, msg, new SharpDX.Vector2(16 * wFactor, 16 * hFactor), SharpDX.Color.PaleGreen,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
            }
            //ghSharpDX.batch.DrawString("FPS: " + ghSharpDX.fpsCounter.FPS + " Current Time " + DateTime.Now.ToString(),
            //                                         16, 16, SharpDX.Color.PaleGreen);
            
            if (printHelp)
            {
                string msg = "--- Pause & Help --- \n" +
                             "Toggle Pause/Help - Esc \n" +
                             "Focus - Hold Shift \n" +
                             "Toggle FPS and Time - F \n" +
                             "Toggle Hitboxes - Num5 \n" +
                             "Return To Title Screen - Enter";
                //ghSharpDX.batch.DrawString(msg, 16, 430, SharpDX.Color.White);
                ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, msg, new SharpDX.Vector2(16 * wFactor, 430 * hFactor), SharpDX.Color.White,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
                //ghSharpDX.batch.DrawString(msg, 13, 427, SharpDX.Color.Black);
            }
            if (printHelpLab)
            {
                string msg = "--- Pause & Help --- \n" +
                             "Toggle Pause/Help - Esc \n" +
                             "Movements - Up/Down/Left/Right \n" +
                             "Change Size - +/- \n" +
                             "Switch Texture - Multiply \n" +
                             "Toggle Texture Count - C \n" +
                             "Toggle Wireframe - W \n" +
                             "Toggle FPS and Time - F \n" +
                             "Toggle Hitboxes - Num5 \n" +
                             "Return To Title Screen - Enter";
                ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, msg, new SharpDX.Vector2(16 * wFactor, 345 * hFactor), SharpDX.Color.White,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
                //ghSharpDX.batch.DrawString(msg, 16, 345, SharpDX.Color.White);
                //ghSharpDX.batch.DrawString(msg, 13, 342, SharpDX.Color.Black);
            }
            if (visibleTextureCountIsActive)
            {
                int i = 0;
                foreach (GameObject go in listPlayerObject) if (go.isVisible) i++;
                foreach (GameObject go in listGameObject) if (go.isVisible) i++;
                if (hitboxDisplayNew) foreach (GameObject go in listHitbox) if (go.isVisible) i++;
                //Console.WriteLine("Number of visible Objects: " + i);
                string msg = "Number of Rendered Textures: " + i;
                //ghSharpDX.batch.DrawString(msg, 16, 40, SharpDX.Color.DarkOrchid);
                ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, msg, new SharpDX.Vector2(16 * wFactor, 40 * hFactor), SharpDX.Color.DarkOrchid,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
            }
        }

        public void logicUpdate()
        {

            switch (gameState)
            {
                case (int)GAMESTATE.MENU:
                    menuLogicUpdate();
                    break;
                case (int)GAMESTATE.PLAYING:
                    playingLogicUpdate();
                    break;
                case (int)GAMESTATE.DEMO:
                    demoLogicUpdate();
                    break;
                default:
                    break;
            }

        }


        private void loadMainMenuGameState()
        {

            //listGameObject = new List<GameObject>();
            listBackgroundObject.Add(new GameObject(ghSharpDX.resourceViewMenu, this));
            listBackgroundObject[0].originX = 0f;
            listBackgroundObject[0].originY = 30f;
            listBackgroundObject[0].size = 0.5f; //(float)ghSharpDX.form.ClientSize.Height / listGameObject[0].txPointerH;

        }


        private void menuLogicUpdate()
        {

            /*
            if (ihSharpDX.kD && !ihSharpDX.kDOnce)
            {
                ihSharpDX.kDOnce = true;
                cleanGameState();
                gameState = (int)GAMESTATE.DEMO;
                loadDemoGameState2();
            }
            */
            if (ihSharpDX.kUp && !ihSharpDX.kUpOnce)
            {
                ihSharpDX.kUpOnce = true;
                menuOption--;
                if (menuOption < 0) menuOption = 2;
            }
            if (ihSharpDX.kDown && !ihSharpDX.kDownOnce)
            {
                ihSharpDX.kDownOnce = true;
                menuOption++;
                if (menuOption > 2) menuOption = 0;
            }

            if (ihSharpDX.kEnter && !ihSharpDX.kEnterOnce)
            {
                ihSharpDX.kEnterOnce = true;
                cleanGameState();
                if (menuOption == 0)
                {
                    gameState = (int)GAMESTATE.PLAYING;
                    loadPlayingGameState();
                }
                if (menuOption == 1)
                {
                    gameState = (int)GAMESTATE.DEMO;
                    loadDemoGameState();
                }
                if (menuOption == 2)
                {
                    Environment.Exit(0);
                }

            }

        }


        private void loadDemoGameState() 
        {
            // populate or update GameObject list
            Random rng = new Random();
            int objectNumber = 1000;
            listPlayerObject.Add(new GameObject(ghSharpDX.resourceViewMokou, this));
            listPlayerObject[0].originX = 0f;
            listPlayerObject[0].originY = 0f;

            // listGameObject[0].alwaysVisible = true;
            for (int i = 0; i < objectNumber; i++)
            {
                listGameObject.Add(new GameObject(ghSharpDX.resourceViewKaguya, this));
            }

            foreach (GameObject go in listGameObject)
            {
                go.originX = -4000 + rng.Next(100)*80;
                go.originY = -4000 + rng.Next(100)*80;
            }

            // hitbox base data creation + original object final coords update
            foreach (GameObject go in listPlayerObject)
            {
                go.updateFinalOriginAndCoord();
                GameObject newGo = new GameObject(ghSharpDX.resourceViewHitboxGreen, this);
                listHitbox.Add(newGo);
            }
            foreach (GameObject go in listGameObject)
            {
                go.updateFinalOriginAndCoord();
                GameObject newGo = new GameObject(ghSharpDX.resourceViewHitboxGreen, this);
                listHitbox.Add(newGo);
            }
        }

        private void demoLogicUpdate()
        {
            //global state checks
            int fps = 60; // ghSharpDX.fpsCounter.FPS
            checkCollision(listPlayerObject[0]);

            //input and action checks
            if (ihSharpDX.kEnter && !ihSharpDX.kEnterOnce)
            {
                ihSharpDX.kEnterOnce = true;
                if (printHelpLab)
                {
                    cleanGameState();
                    gameState = (int)GAMESTATE.MENU;
                    loadMainMenuGameState();
                    return;
                }
            }
            if (ihSharpDX.kEscape && !ihSharpDX.kEscapeOnce)
            {
                ihSharpDX.kEscapeOnce = true;
                printHelpLab = !printHelpLab;
            }
            if (printHelpLab) return; // Handling pause the hard way

            if (ihSharpDX.kUp)
            {
                //acc = acc + (20f / (float)avgFPS);
                listPlayerObject[0].originY = listPlayerObject[0].originY - (500f / fps);
                cameraOriginY = cameraOriginY - (500f / fps);
            }

            if (ihSharpDX.kDown)
            {
                //acc = acc + (20f / (float)avgFPS);
                listPlayerObject[0].originY = listPlayerObject[0].originY + (500f / fps);
                cameraOriginY = cameraOriginY + (500f / fps);
            }

            if (ihSharpDX.kRight)
            {
                //acc = acc + (20f / (float)avgFPS);
                listPlayerObject[0].originX = listPlayerObject[0].originX + (500f / fps);
                cameraOriginX = cameraOriginX + (500f / fps);
            }

            if (ihSharpDX.kLeft)
            {
                //acc = acc + (20f / (float)avgFPS);
                listPlayerObject[0].originX = listPlayerObject[0].originX - (500f / fps);
                cameraOriginX = cameraOriginX - (500f / fps);
            }

            if (ihSharpDX.kPlus && !ihSharpDX.kPlusOnce)
            {
                listPlayerObject[0].size = listPlayerObject[0].size + 0.1f;
                //listGameObject[1].size = listGameObject[1].size + 0.1f;
                ihSharpDX.kPlusOnce = true;
                //updateAspectRatioAll();
            }
            if (ihSharpDX.kMinus && !ihSharpDX.kMinusOnce)
            {
                listPlayerObject[0].size = listPlayerObject[0].size - 0.1f;
                //listGameObject[1].size = listGameObject[1].size - 0.1f;
                ihSharpDX.kMinusOnce = true;
                //updateAspectRatioAll();
            }
            if (ihSharpDX.kMultiply && !ihSharpDX.kMultiplyOnce)
            {
                if (listPlayerObject[0].resourceViewSharpDX.Equals(ghSharpDX.resourceViewMokou))
                {
                    listPlayerObject[0].resourceViewSharpDX = ghSharpDX.resourceViewKaguya;
                    //listGameObject[0].txPointer = gh.txKaguya;
                }
                else
                {
                    listPlayerObject[0].resourceViewSharpDX = ghSharpDX.resourceViewMokou;
                    //listGameObject[0].txPointer = gh.txMokou;
                }
                //loadTextureInfo(listGameObject[0]);
                //gh.updateAspectRatioAll(listGameObject);
                ihSharpDX.kMultiplyOnce = true;
            }
            if (ihSharpDX.kW && !ihSharpDX.kWOnce)
            {
                ihSharpDX.kWOnce = true;
                wireframe = !wireframe;

            }
            if (ihSharpDX.kC && !ihSharpDX.kCOnce)
            {
                ihSharpDX.kCOnce = true;
                visibleTextureCountIsActive = !visibleTextureCountIsActive;
            }
            if (ihSharpDX.kF && !ihSharpDX.kFOnce)
            {
                ihSharpDX.kFOnce = true;
                printFPS = !printFPS;
                //Console.WriteLine("Start/Stop FPS counter");
            }
            if (ihSharpDX.kNumpad5 && !ihSharpDX.kNumpad5Once) // Handle Hitbox Display
            {
                ihSharpDX.kNumpad5Once = true;
                hitboxDisplayNew = !hitboxDisplayNew;
            }

            //additional checks
            if (hitboxDisplayNew) //XXX: Fix last index of hitbox gameobject which is not treated...
            {
                // Player Case
                if (collisionObject.Count > 0)
                {
                    listHitbox[0].resourceViewSharpDX = ghSharpDX.resourceViewHitboxRed;
                }
                else
                {
                    listHitbox[0].resourceViewSharpDX = ghSharpDX.resourceViewHitboxGreen;
                }
                listHitbox[0].deepCopyFrom(listPlayerObject[0]);
                listHitbox[0].size = (listPlayerObject[0].txPointerH / listHitbox[0].txPointerH) * listPlayerObject[0].size * listPlayerObject[0].hitboxFactor;

                // All Objects Cases
                for (int i = 0; i < listGameObject.Count; i++)
                {
                    if (collisionObject.Contains(listGameObject[i]))
                    {
                        listHitbox[i + listPlayerObject.Count].resourceViewSharpDX = ghSharpDX.resourceViewHitboxRed;
                    }
                    else
                    {
                        listHitbox[i + listPlayerObject.Count].resourceViewSharpDX = ghSharpDX.resourceViewHitboxGreen;
                    }
                    listHitbox[i + listPlayerObject.Count].deepCopyFrom(listGameObject[i]);
                    listHitbox[i + listPlayerObject.Count].size = (listGameObject[i].txPointerH / listHitbox[i + listPlayerObject.Count].txPointerH) * listGameObject[i].size * listGameObject[i].hitboxFactor;
                    //Console.WriteLine("Size = " + listHitbox[i].size);

                }
            }

        }


        private void loadPlayingGameState()
        {
            //Initialize Timings Events
            for (int i = 0; i < 100; i++) timingEventDone.Add(false);

            //LoadBackground
            listBackgroundObject.Add(new GameObject(ghSharpDX.resourceViewBackground, this));
            listBackgroundObject[0].originY = 0f;
            //listBackgroundObject[0].alwaysVisible = true;

            listBackgroundObject.Add(new GameObject(ghSharpDX.resourceViewBackground, this));
            listBackgroundObject[1].originY = -1800f;
            //listBackgroundObject[1].alwaysVisible = true;
            

            //Player = 0, 1 = player's hitbox
            listPlayerObject.Add(new GameObject(ghSharpDX.resourceViewMokou, this));
            listPlayerObject[0].size = 0.5f;
            listPlayerObject[0].originX = 0f;
            listPlayerObject[0].originY = 220f;

            //player hitbox
            listPlayerObject.Add(new GameObject(ghSharpDX.resourceViewFocusHitbox, this));
            listPlayerObject[1].hitboxFactor = 0.5f;

            //enemy
            listGameObject.Add(new GameObject(ghSharpDX.resourceViewKaguya, this));
            listGameObject[0].size = 0.5f;
            listGameObject[0].hitboxFactor = 0.4f;
            listGameObject[0].offsetY = -8;
            listGameObject[0].originX = 0f;
            listGameObject[0].originY = -500f;

            //bullets
            for (int i = 0; i < 2000; i++)
            {
                listBulletObject.Add(new BulletObject(ghSharpDX.resourceViewBullet00, this));
                //listBulletObject[i].originX = 0f;
                //listBulletObject[i].originY = 0f;
                //listBulletObject[i].size = 5f;
                //listBulletObject[i].hitboxFactor = 0.5f;
            }
            

            // hitbox base data creation + original object final coords update
            foreach (GameObject go in listPlayerObject)
            {
                GameObject newGo = new GameObject(ghSharpDX.resourceViewHitboxGreen, this);
                listHitbox.Add(newGo);
            }
            foreach (GameObject go in listGameObject)
            {
                GameObject newGo = new GameObject(ghSharpDX.resourceViewHitboxGreen, this);
                listHitbox.Add(newGo);
            }
            foreach (BulletObject bo in listBulletObject)
            {
                GameObject newGo = new GameObject(ghSharpDX.resourceViewHitboxGreen, this);
                newGo.alwaysHidden = true;
                listHitbox.Add(newGo);
            }
        }

        private void playingLogicUpdate() // TODO: Level Design Implementation for multilevels
        {
            //check logic states
            checkCollisionFull(listPlayerObject[1]);
            float playerSpeed = 240f;
            focus = false;
            listPlayerObject[1].alwaysHidden = true;
            listPlayerObject[0].speedY = 0f;
            listPlayerObject[0].speedX = 0f;
            int fps = 60; // ghSharpDX.fpsCounter.FPS;

            //input and action checks
            if (ihSharpDX.kEnter && !ihSharpDX.kEnterOnce)
            {
                ihSharpDX.kEnterOnce = true;
                if (printHelp)
                {
                    cleanGameState();
                    gameState = (int)GAMESTATE.MENU;
                    loadMainMenuGameState();
                    return;
                }
            }
            if (ihSharpDX.kEscape && !ihSharpDX.kEscapeOnce)
            {
                ihSharpDX.kEscapeOnce = true;
                printHelp = !printHelp;
            }
            if (printHelp) return; // Handling pause the hard way

            if (ihSharpDX.kShift) // Focus
            {
                focus = true;
                playerSpeed = 120f;
                listPlayerObject[1].alwaysHidden = false;
            }
            if (ihSharpDX.kUp)
            {

                if (listPlayerObject[0].originY > -300) listPlayerObject[0].originY = listPlayerObject[0].originY - (playerSpeed / fps);
                
            }

            if (ihSharpDX.kDown)
            {

                if (listPlayerObject[0].originY < 300) listPlayerObject[0].originY = listPlayerObject[0].originY + (playerSpeed / fps);
                
            }

            if (ihSharpDX.kRight)
            {

                if (listPlayerObject[0].originX < 400) listPlayerObject[0].originX = listPlayerObject[0].originX + (playerSpeed / fps);
                
            }

            if (ihSharpDX.kLeft)
            {

                if (listPlayerObject[0].originX > -400) listPlayerObject[0].originX = listPlayerObject[0].originX - (playerSpeed / fps);
                
            }

            if (ihSharpDX.kF && !ihSharpDX.kFOnce)
            {
                ihSharpDX.kFOnce = true;
                printFPS = !printFPS;
            }
            if (ihSharpDX.kNumpad5 && !ihSharpDX.kNumpad5Once) // Handle Hitbox Display
            {
                ihSharpDX.kNumpad5Once = true;
                hitboxDisplayNew = !hitboxDisplayNew;
            }
            

            //permanent updates
            listPlayerObject[1].originX = listPlayerObject[0].originX + 2; //update focus hitbox into player sprite
            listPlayerObject[1].originY = listPlayerObject[0].originY + 8;

            //scrolling background
            listBackgroundObject[0].originY = listBackgroundObject[0].originY + (60f / fps);
            listBackgroundObject[1].originY = listBackgroundObject[1].originY + (60f / fps);
            if (listBackgroundObject[0].originY > 1800) listBackgroundObject[0].originY = -1800f;
            if (listBackgroundObject[1].originY > 1800) listBackgroundObject[1].originY = -1800f;

            //Bullet Logic, must be applied before Timing logic
            if (timingEventDone[0])
            {
                foreach (BulletObject bo in listBulletObject)
                {
                    if (bo.isActive) bo.follow();
                }
            }

            //Timing Logic
            if (!timingEventDone[0] && frameLogicCount > 1 * fps)
            {
                //listGameObject[0].originX = 0f;
                listGameObject[0].originY = listGameObject[0].originY + ((100f - acceleration) / fps);
                if (listGameObject[0].originY > -200)
                {
                    listGameObject[0].originY = -200;
                    timingEventDone[0] = true;
                    frameLogicCount = 0;
                    acceleration = 0f;
                }
                if (listGameObject[0].originY > -250)
                {
                    acceleration = acceleration + (100f / fps);
                    if (acceleration > 80f) { acceleration = 90f; }
                }
                
            }
            if (timingEventDone[0] && !timingEventDone[1] && frameLogicCount > 1.75f * fps)
            {
                timingEventDone[1] = true;
                frameLogicCount = 0;
            }
            if (timingEventDone[1] && !timingEventDone[2] && frameLogicCount > (0.5f + delay) * fps)
            {
                int bulletBatch = 20;
                for (int i = 0; i < bulletBatch; i++)
                {
                    listBulletObject[bulletCount + i].originX = listGameObject[0].originX - (20 * bulletBatch) + (i * 4 * bulletBatch);
                    listBulletObject[bulletCount + i].originY = listGameObject[0].originY - 30;
                    listBulletObject[bulletCount + i].trackPlayerData(listPlayerObject[1].originX - (20 * bulletBatch) + (i * 4 * bulletBatch), listPlayerObject[1].originY);
                    listBulletObject[bulletCount + i].isActive = true;
                    listBulletObject[bulletCount + i].size = 4f;
                    listBulletObject[bulletCount + i].speed = 70f;
                    listHitbox[listPlayerObject.Count + listGameObject.Count + bulletCount + i].alwaysHidden = false;
                }
                

                if ((bulletCount / bulletBatch) % 10 == 9)
                {
                    delay = 2.0f;
                }
                else
                {
                    delay = 0f;
                }
                if ((bulletCount / bulletBatch) % 20 > 9)
                {
                    for (int i = 0; i < bulletBatch; i++)
                    {
                        listBulletObject[bulletCount + i].speed = 200f;
                        listBulletObject[bulletCount + i].size = 1f;
                    }
                }

                if (bulletCount < (listBulletObject.Count - bulletBatch))
                {
                    bulletCount = bulletCount + bulletBatch;
                }
                else
                {
                    bulletCount = 0;
                    //delay = 0.5f;
                    //timingEventDone[1] = true;
                }
                //timingEventDone[1] = true;
                frameLogicCount = 0;
                
            }
            frameLogicCount++;

            //hotbox debug display
            if (hitboxDisplayNew)
            {
                //int i = 0;
                int k = 0;
                foreach (GameObject go in listPlayerObject)
                {
                    // color player hitbox
                    if (collisionObject.Count > 0) listHitbox[k].resourceViewSharpDX = ghSharpDX.resourceViewHitboxRed;
                    else listHitbox[k].resourceViewSharpDX = ghSharpDX.resourceViewHitboxGreen;

                    listHitbox[k].deepCopyFrom(go);
                    listHitbox[k].size = (go.txPointerH / listHitbox[k].txPointerH) * go.size * go.hitboxFactor;
                    k++;
                }
                
                foreach (GameObject go in listGameObject)
                {

                    if (collisionObject.Contains(go))
                    {
                        listHitbox[k].resourceViewSharpDX = ghSharpDX.resourceViewHitboxRed;
                    }
                    else
                    {
                        listHitbox[k].resourceViewSharpDX = ghSharpDX.resourceViewHitboxGreen;
                    }
                    listHitbox[k].deepCopyFrom(go);
                    listHitbox[k].size = (go.txPointerH / listHitbox[k].txPointerH) * go.size * go.hitboxFactor;
                    k++;

                }
                foreach (BulletObject bo in listBulletObject)
                {

                    if (collisionObject.Contains(bo))
                    {
                        listHitbox[k].resourceViewSharpDX = ghSharpDX.resourceViewHitboxRed;
                    }
                    else
                    {
                        listHitbox[k].resourceViewSharpDX = ghSharpDX.resourceViewHitboxGreen;
                    }
                    listHitbox[k].deepCopyFrom(bo);
                    listHitbox[k].size = (bo.txPointerH / listHitbox[k].txPointerH) * bo.size * bo.hitboxFactor;
                    k++;

                }


            }


        }


        private void checkCollision(GameObject go)
        {
            collisionObject.Clear();
            foreach (GameObject collisionGo in listGameObject)
            {
                if (collisionGo.isVisible)
                {
                    if (go.collideWith(collisionGo))
                    {
                        collisionObject.Add(collisionGo);
                    }
                }
            }
        }

        private void checkCollisionFull(GameObject go)
        {
            collisionObject.Clear();
            for (int i = 0; i < listGameObject.Count; i++)
            {
                if (listGameObject[i].isVisible)
                {
                    if (go.collideWith(listGameObject[i]))
                    {
                        collisionObject.Add(listGameObject[i]);
                    }
                }
            }
            for (int i = 0; i < listBulletObject.Count; i++)
            {
                if (listBulletObject[i].isVisible && listBulletObject[i].isActive)
                {
                    // real collision 
                    if (listBulletObject[i].size > 1f && go.collideCircleWith(listBulletObject[i],
                       ((listBulletObject[i].txPointerW * listBulletObject[i].size * listBulletObject[i].hitboxFactor) / 2f) - 6 ))
                    {
                        collisionObject.Add(listBulletObject[i]);
                    }
                    if (listBulletObject[i].size <= 1f && go.collideCircleWith(listBulletObject[i],
                       ((listBulletObject[i].txPointerW * listBulletObject[i].size * listBulletObject[i].hitboxFactor) / 2f) + 1))
                    {
                        collisionObject.Add(listBulletObject[i]);
                    }
                    // grazing
                    if (go.collideCircleWith(listBulletObject[i],
                       ((listBulletObject[i].txPointerW * listBulletObject[i].size * listBulletObject[i].hitboxFactor) / 2f) + 6))
                    {
                        //collisionObject.Add(listBulletObject[i]);
                    }
                }
            }

        }



    //-----------------------------------END---OF---CLASS---------------------------------------------------------------
    }
}
