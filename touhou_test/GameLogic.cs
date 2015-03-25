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

        public int fps = 0;
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

        public bool focus = false;
        public bool fire = false;
        public int frameCountFire = 0;
        public int playerBulletCount = 0;
        public float bulletDelay = 0f;

        //public List<bool> timingEventDone;
        //public int frameLogicCount = 0;
        //public int bulletCount = 0;
        //public float delay = 0;
        //public float acceleration = 0;

        public List<GameObject> listBackgroundObject;

        public List<GameObject> listPlayerObject;
        public List<BulletObject> listPlayerBulletObject;
        public List<GameObject> listGameObject;
        public List<BulletObject> listBulletObject;

        public List<GameObject> collisionObject;
        public List<GameObjectDuo> collisionEnemy;

        public Level level;

        private void cleanGameState()
        {
            // Object cleaning
            listPlayerObject.Clear();
            listBulletObject.Clear();
            listPlayerBulletObject.Clear();
            listBackgroundObject.Clear();
            listGameObject.Clear();
            collisionObject.Clear();
            collisionEnemy.Clear();

            // Global variables cleaning
            fps = 0;
            collision = 0;
            hitboxDisplayNew = false;
            cameraOriginX = 0;
            cameraOriginY = 0;
            printFPS = false;
            wireframe = false;
            focus = false;
            printHelp = false;
            printHelpLab = false;
            visibleTextureCountIsActive = false;

            //frameLogicCount = 0;
            //bulletCount = 0;
            //delay = 0;
            //acceleration = 0;

            // Reinitialization of stuff
            level = new Level(this);

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
            listBackgroundObject = new List<GameObject>();
            listPlayerObject = new List<GameObject>();
            listPlayerBulletObject = new List<BulletObject>();
            listGameObject = new List<GameObject>();
            listBulletObject = new List<BulletObject>();
            collisionObject = new List<GameObject>();
            collisionEnemy = new List<GameObjectDuo>();

            //init levels
            level = new Level(this);

            //launch first menu
            //if (g.graphicLib == (int)Game.GRAPHIC.SLIMDX) loadMainMenuGameState();
            //else
            loadMainMenuGameState();

        }


        public void drawAllGameObjects()
        {
            SharpDX.Color color = new SharpDX.Color(new SharpDX.Vector4(1f, 1f, 1f, 1f));     // last float in vector4 is alpha used for transarency
            SharpDX.Color colorT75 = new SharpDX.Color(new SharpDX.Vector4(1f, 1f, 1f, 0.75f)); // 75% opacity

            //begin drawing
            if (wireframe) ghSharpDX.batch.Batch.Begin(SpriteSortMode.Immediate, ghSharpDX.bsToolkit, ghSharpDX.ssToolkit, null, ghSharpDX.rsToolkitWireframe);
            else ghSharpDX.batch.Batch.Begin(SpriteSortMode.Immediate, ghSharpDX.bsToolkit, ghSharpDX.ssToolkit, null, ghSharpDX.rsToolkit);
            //draw
            
            int bgCount = 0;
            foreach (GameObject go in listBackgroundObject)
            {
                if (bgCount == 0) ghSharpDX.drawGameObject(go, color, false);
                else ghSharpDX.drawGameObject(go, color, true);
                bgCount++;
            }
            foreach (BulletObject bo in listPlayerBulletObject) ghSharpDX.drawBulletObject(bo, colorT75, false);
            foreach (GameObject go in listGameObject) ghSharpDX.drawGameObject(go, color, false);
            foreach (GameObject go in listPlayerObject) ghSharpDX.drawGameObject(go, color, false);
            foreach (BulletObject bo in listBulletObject) ghSharpDX.drawBulletObject(bo, colorT75, false);
            

            if (hitboxDisplayNew)
            {
                foreach (GameObject go in listPlayerBulletObject) { ghSharpDX.drawHitboxObject(go); }
                foreach (GameObject go in listGameObject) { ghSharpDX.drawHitboxObject(go); }
                foreach (GameObject go in listPlayerObject) { ghSharpDX.drawHitboxObject(go); }
                foreach (GameObject go in listBulletObject) { ghSharpDX.drawHitboxObject(go); }
                
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
                ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, danmaku, new SharpDX.Vector2(100 * wFactor, 400 * hFactor), SharpDX.Color.Black,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
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
            if (printHelp)
            {
                string msg = "--- Pause & Help --- \n" +
                             "Toggle Pause/Help - Esc \n" +
                             "Focus - Hold Shift \n" +
                             "Toggle FPS and Time - F \n" +
                             "Toggle Hitboxes - Num5 \n" +
                             "Return To Title Screen - Enter";                
                ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, msg, new SharpDX.Vector2(16 * wFactor, 430 * hFactor), SharpDX.Color.White,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
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
            }
            if (visibleTextureCountIsActive)
            {
                int i = 0;
                foreach (GameObject go in listPlayerObject) if (go.isVisible) i++;
                foreach (GameObject go in listGameObject) if (go.isVisible) i++;
                string msg = "Number of Rendered Textures: " + i;
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
            /*
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
            */ 
        }

        private void demoLogicUpdate()
        {
            // Handling pause the hard way
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
            if (printHelpLab) return; 

            //global state checks
            int fps = 60; // ghSharpDX.fpsCounter.FPS
            checkCollision(listPlayerObject[0]);

            //input and action checks
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
            if (hitboxDisplayNew) updateHitboxData();


        }


        private void loadPlayingGameState()
        {

            level.loadStage01();

        }

        private void playingLogicUpdate() // TODO: Level Design Implementation for multilevels
        {
            // Handling pause the hard way
            if (ihSharpDX.kEscape && !ihSharpDX.kEscapeOnce)
            {
                ihSharpDX.kEscapeOnce = true;
                printHelp = !printHelp;
            }
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
            if (printHelp) return; 

            //Global Logic
            checkCollisionFull(listPlayerObject[1]);
            checkCollisionPlayerBullet();

            //Initial Variables
            float playerSpeed = 240f;
            focus = false;
            listPlayerObject[1].alwaysHidden = true;
            fps = 60; // ghSharpDX.fpsCounter.FPS; // For stability purposes, might change if no v-sync

            //input and action checks
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

            if (ihSharpDX.kShift) // Focus
            {
                focus = true;
                playerSpeed = playerSpeed / 2f;
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
            if (ihSharpDX.kY)
            {
                if (frameCountFire > ((0.1+bulletDelay) * fps))
                {
                    frameCountFire = 0;
                    listPlayerBulletObject[playerBulletCount].isActive = true;
                    listPlayerBulletObject[playerBulletCount].originX = listPlayerObject[0].originX;
                    listPlayerBulletObject[playerBulletCount].originY = listPlayerObject[0].originY;
                    listPlayerBulletObject[playerBulletCount].trackPlayerData(listPlayerObject[0].originX, listPlayerObject[0].originY - 10); //going up
                    if (focus)
                    {
                        bulletDelay = 1.2f;
                        listPlayerBulletObject[playerBulletCount].speed = 120f;
                        listPlayerBulletObject[playerBulletCount].resourceViewSharpDX = ghSharpDX.resourceViewPlayerBullet01;
                    }
                    else
                    {
                        bulletDelay = 0f;
                        listPlayerBulletObject[playerBulletCount].speed = 360f;
                        listPlayerBulletObject[playerBulletCount].resourceViewSharpDX = ghSharpDX.resourceViewPlayerBullet00;
                    }
                    listPlayerBulletObject[playerBulletCount].updateTextureSizeDescription();

                    if (playerBulletCount >= listPlayerBulletObject.Count - 1) playerBulletCount = 0;
                    else playerBulletCount++;
                }
            }
            
            //permanent updates
            listPlayerObject[1].originX = listPlayerObject[0].originX + 2; //update focus hitbox into player sprite
            listPlayerObject[1].originY = listPlayerObject[0].originY + 8;

            //scrolling background
            listBackgroundObject[0].originY = listBackgroundObject[0].originY + (60f / fps);
            listBackgroundObject[1].originY = listBackgroundObject[1].originY + (60f / fps);
            if (listBackgroundObject[0].originY > 1799) listBackgroundObject[0].originY = -1799f;
            if (listBackgroundObject[1].originY > 1799) listBackgroundObject[1].originY = -1799f;

            //Bullet Logic, must be applied before Timing logic/level logic
            foreach (BulletObject bo in listBulletObject)
            {
                bo.isHitboxActive = bo.isActive;
                if (bo.isActive) bo.follow();
            }
            foreach (BulletObject bo in listPlayerBulletObject)
            {
                bo.isHitboxActive = bo.isActive;
                if (bo.isActive) bo.follow();
            }

            //Level Logic
            level.logicStage01();

            //hitbox debug display
            if (hitboxDisplayNew) updateHitboxData();

            //Finally, update counter and stuffs
            frameCountFire++;
        }

        private void updateHitboxData()
        {
            foreach (GameObject go in listPlayerObject)
            {
                // color player hitbox
                if (collisionObject.Count > 0) go.hitboxType = 1;
                else go.hitboxType = 0;
            }
            foreach (GameObject go in listGameObject)
            {
                if (collisionObject.Contains(go)) go.hitboxType = 1;
                else go.hitboxType = 0;                
            }
            foreach (BulletObject bo in listBulletObject)
            {
                if (collisionObject.Contains(bo)) bo.hitboxType = 1;
                else bo.hitboxType = 0;
            }
            foreach (BulletObject bo in listPlayerBulletObject)
            {
                bo.hitboxType = 0;
                foreach (GameObjectDuo god in collisionEnemy)
                {
                    if (god.bo.Equals(bo))
                    {
                        bo.hitboxType = 2;
                        god.go.hitboxType = 2;
                    }
                }
            }
        }

        /*
        private void updateHitboxData()
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
            foreach (BulletObject bo in listPlayerBulletObject)
            {
                listHitbox[k].deepCopyFrom(bo);
                listHitbox[k].size = (bo.txPointerH / listHitbox[k].txPointerH) * bo.size * bo.hitboxFactor;
                k++;
            }
        }
        */

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

        private void checkCollisionPlayerBullet()
        {
            collisionEnemy.Clear();

            // Our player bullet detection with enemies
            foreach (BulletObject bo in listPlayerBulletObject)
            {
                if (bo.isVisible && bo.isActive)
                {
                    foreach (GameObject go in listGameObject)
                    {
                        if (go.isVisible)
                        {

                            if (go.collideWith(bo))
                            {
                                collisionEnemy.Add(new GameObjectDuo(go, bo));
                            }

                        }

                    }

                }
            }

        }

        private void checkCollisionFull(GameObject mainGo)
        {
            collisionObject.Clear();
            
            foreach (GameObject go in listGameObject)
            {
                if (go.isVisible)
                {
                    if (mainGo.collideWith(go))
                    {
                        collisionObject.Add(go);
                    }
                }
            }
            foreach (BulletObject bo in listBulletObject)
            {
                if (bo.isVisible && bo.isActive)
                {
                    // real collision 
                    if (bo.size > 1f && mainGo.collideCircleWith(bo,
                       ((bo.txPointerW * bo.size * bo.hitboxFactor) / 2f) - 12))
                    {
                        collisionObject.Add(bo);
                    }
                    if (bo.size <= 1f && mainGo.collideCircleWith(bo,
                       ((bo.txPointerW * bo.size * bo.hitboxFactor) / 2f) - 6))
                    {
                        collisionObject.Add(bo);
                    }
                    // grazing
                    if (mainGo.collideCircleWith(bo,
                       ((bo.txPointerW * bo.size * bo.hitboxFactor) / 2f) + 8))
                    {
                        //collisionObject.Add(listBulletObject[i]);
                    }
                }
            }

        }



    //-----------------------------------END---OF---CLASS---------------------------------------------------------------
    }
}
