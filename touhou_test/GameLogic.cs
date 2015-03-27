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
        public GAMESTATE gameState = GAMESTATE.MENU;

        public int fps = 0;
        public int menuOption = 0;
        public int gameOverMenuOption = 0;
        public int pauseMenuOption = 0;
        public int collision = 0;
        public bool hitboxDisplayNew = false;
        public float cameraOriginX = 0;
        public float cameraOriginY = 0;
        public bool printFPS = false;
        public bool wireframe = false;
        public bool pause = false;
        public bool printHelpLab = false;
        public bool visibleTextureCountIsActive = false;

        public bool focus = false;
        public bool fire = false;
        public int frameCountFire = 0;
        public int playerBulletCount = 0;
        public float bulletDelay = 0f;

        public int lives = 0;
        public bool isInvincible = false;
        public int invincibleTimer = 0;
        public bool gameOver = false;
        public int score = 0;
        public float fadingLevel = 0f;
        public bool fadingUp = true;
        public bool darkVision = false;

        public List<GameObject> listBackgroundObject;

        public List<GameObject> listPlayerObject;
        public List<BulletObject> listPlayerBulletObject;
        public List<GameObject> listGameObject;
        public List<BulletObject> listBulletObject;
        public List<EnemyObject> listEnemyObject;

        public List<GameObject> collisionObject;
        public List<GameObjectDuo> collisionEnemy;

        public List<GameObject> lifeUI;

        public Level level;

        private void cleanGameState()
        {
            // Object cleaning
            listPlayerObject.Clear();
            listBulletObject.Clear();
            listPlayerBulletObject.Clear();
            listBackgroundObject.Clear();
            listGameObject.Clear();
            listEnemyObject.Clear();
            collisionObject.Clear();
            collisionEnemy.Clear();
            lifeUI.Clear();

            // Global variables cleaning
            fps = 0;
            collision = 0;
            hitboxDisplayNew = false;
            cameraOriginX = 0;
            cameraOriginY = 0;
            printFPS = false;
            wireframe = false;
            focus = false;
            pause = false;
            printHelpLab = false;
            visibleTextureCountIsActive = false;
            lives = 0;
            isInvincible = false;
            invincibleTimer = 0;
            gameOver = false;
            gameOverMenuOption = 0;
            score = 0;
            fadingLevel = 0f;
            fadingUp = true;
            darkVision = false;
            pauseMenuOption = 0;

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
            listEnemyObject = new List<EnemyObject>();
            collisionObject = new List<GameObject>();
            collisionEnemy = new List<GameObjectDuo>();
            lifeUI = new List<GameObject>();

            //init levels
            level = new Level(this);

            //launch first menu
            //if (g.graphicLib == (int)Game.GRAPHIC.SLIMDX) loadMainMenuGameState();
            //else
            loadMainMenuGameState();

        }


        public void drawAllGameObjects()
        {
            // Coloring inits and logic
            float rgb = 1f;
            if (gameOver || darkVision) rgb = 0.5f;
            SharpDX.Color color = new SharpDX.Color(new SharpDX.Vector4(rgb, rgb, rgb, 1f));       // last float in vector4 is alpha used for transarency
            SharpDX.Color colorT75 = new SharpDX.Color(new SharpDX.Vector4(rgb, rgb, rgb, 0.75f)); // 75% opacity for most bullets
            SharpDX.Color colorT33 = new SharpDX.Color(new SharpDX.Vector4(rgb, rgb, rgb, 0.33f)); // 33% opacity
            SharpDX.Color playerColor = color;
            if (isInvincible) playerColor = colorT33;

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
            foreach (EnemyObject go in listEnemyObject) ghSharpDX.drawGameObject(go, color, false);
            foreach (GameObject go in listPlayerObject) ghSharpDX.drawGameObject(go, playerColor, false);
            foreach (BulletObject bo in listBulletObject) ghSharpDX.drawBulletObject(bo, colorT75, false);
            

            if (hitboxDisplayNew)
            {
                foreach (GameObject go in listPlayerBulletObject) { ghSharpDX.drawHitboxObject(go); }
                foreach (GameObject go in listGameObject) { ghSharpDX.drawHitboxObject(go); }
                foreach (EnemyObject eo in listEnemyObject) { ghSharpDX.drawHitboxObject(eo); }
                foreach (GameObject go in listPlayerObject) { ghSharpDX.drawHitboxObject(go); }
                foreach (GameObject go in listBulletObject) { ghSharpDX.drawHitboxObject(go); }
                
            }

            if (gameState == GAMESTATE.PLAYING) drawGameUI();
            drawAllText();

            //flush text to view
            ghSharpDX.batch.End();
            
        }

        private void drawGameUI()
        {
            SharpDX.Color color = new SharpDX.Color(new SharpDX.Vector4(1f, 1f, 1f, 1f));
            foreach (GameObject go in lifeUI) ghSharpDX.drawGameObject(go, color, false);
        }

        private void drawAllText()
        {
            float wFactor = (float)ghSharpDX.form.ClientSize.Width / (float)ghSharpDX.windowW;
            float hFactor = (float)ghSharpDX.form.ClientSize.Height / (float)ghSharpDX.windowH;
            float fontMul = 1f;
            SharpDX.Vector2 fontSize = new SharpDX.Vector2(wFactor * fontMul, hFactor * fontMul);
            fps = ghSharpDX.fpsCounter.FPS;

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
                string msg = "FPS: " + fps + " Current Time " + DateTime.Now.ToString();
                ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, msg, new SharpDX.Vector2(16 * wFactor, 40 * hFactor), SharpDX.Color.PaleGreen,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
            }
            if (pause)
            {
                string msg = "--- Help --- \n" +
                             "Toggle Pause/Help - Esc \n" +
                             "Focus - Hold Shift \n" +
                             "Toggle FPS and Time - F \n" +
                             "Toggle Hitboxes - Num5 \n" +
                             "Return To Title Screen - Enter";
                ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, msg, new SharpDX.Vector2(16 * wFactor, 430 * hFactor), SharpDX.Color.White,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);

                string pauseString = "--- PAUSE ---";
                string resume = "Resume";
                string quit = "Quit";
                ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, pauseString, new SharpDX.Vector2(330 * wFactor, 250 * hFactor), SharpDX.Color.Gray,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
                ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, pauseString, new SharpDX.Vector2(327 * wFactor, 247 * hFactor), SharpDX.Color.White,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);

                ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, resume, new SharpDX.Vector2(300 * wFactor, 285 * hFactor), SharpDX.Color.Gray,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
                ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, quit, new SharpDX.Vector2(440 * wFactor, 285 * hFactor), SharpDX.Color.Gray,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
                if (pauseMenuOption == 0)
                {
                    ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, resume, new SharpDX.Vector2(297 * wFactor, 282 * hFactor), SharpDX.Color.White,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
                }
                if (pauseMenuOption == 1)
                {
                    ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, quit, new SharpDX.Vector2(437 * wFactor, 282 * hFactor), SharpDX.Color.White,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
                }

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
                ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, msg, new SharpDX.Vector2(16 * wFactor, 64 * hFactor), SharpDX.Color.DarkOrchid,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
            }
            if (gameOver)
            {
                string msg = "Game Over... Continue ?";
                string yes = "Yes";
                string no = "No";
                ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, msg, new SharpDX.Vector2(290 * wFactor, 250 * hFactor), SharpDX.Color.Gray,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
                ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, msg, new SharpDX.Vector2(287 * wFactor, 247 * hFactor), SharpDX.Color.White,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);

                ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, yes, new SharpDX.Vector2(320 * wFactor, 285 * hFactor), SharpDX.Color.Gray,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
                ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, no, new SharpDX.Vector2(460 * wFactor, 285 * hFactor), SharpDX.Color.Gray,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
                if (gameOverMenuOption == 0)
                {
                    ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, yes, new SharpDX.Vector2(317 * wFactor, 282 * hFactor), SharpDX.Color.White,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
                }
                if (gameOverMenuOption == 1)
                {
                    ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, no, new SharpDX.Vector2(457 * wFactor, 282 * hFactor), SharpDX.Color.White,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
                }
            }
            if (gameState == GAMESTATE.PLAYING)
            {
                //always displayed text
                string msg = "Score - "+score;
                ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, msg, new SharpDX.Vector2(19 * wFactor, 19 * hFactor), SharpDX.Color.Black,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);
                ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, msg, new SharpDX.Vector2(16 * wFactor, 16 * hFactor), SharpDX.Color.White,
                               0, new SharpDX.Vector2(0, 0), fontSize, SpriteEffects.None, 0f);

                //specific moments texts, like level intro, etc
                if (level.stageNameIsVisible)
                {
                    fadingLevel = fadingLevel + (0.25f / (float)fps);
                    if (fadingLevel >= 1f) fadingLevel = 1f;

                    string levelName = "Level One";
                    SharpDX.Color fadingBlack = new SharpDX.Color(new SharpDX.Vector4(0f, 0f, 0f, 1f-fadingLevel));
                    SharpDX.Color fadingWhite = new SharpDX.Color(new SharpDX.Vector4(1f, 1f, 1f, 1f-fadingLevel));

                    ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, levelName, new SharpDX.Vector2(310 * wFactor, 240 * hFactor), fadingBlack,
                               0, new SharpDX.Vector2(0, 0), fontSize*2f, SpriteEffects.None, 0f);
                    ghSharpDX.batch.Batch.DrawString(ghSharpDX.batch.Font, levelName, new SharpDX.Vector2(305 * wFactor, 235 * hFactor), fadingWhite,
                                   0, new SharpDX.Vector2(0, 0), fontSize*2f, SpriteEffects.None, 0f);
                }
            }
        }

        public void logicUpdate()
        {
            switch (gameState)
            {
                case GAMESTATE.MENU:
                    menuLogicUpdate();
                    break;
                case GAMESTATE.PLAYING:
                    playingLogicUpdate();
                    break;
                case GAMESTATE.DEMO:
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

            if (ihSharpDX.kEnter && !ihSharpDX.kEnterOnce || ihSharpDX.kY && !ihSharpDX.kYOnce)
            {
                ihSharpDX.kEnterOnce = true;
                ihSharpDX.kYOnce = true;
                cleanGameState();
                if (menuOption == 0)
                {
                    gameState = GAMESTATE.PLAYING;
                    loadPlayingGameState();
                }
                if (menuOption == 1)
                {
                    gameState = GAMESTATE.DEMO;
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
            //init variables and futur UI elements
            lives = 3;
            for (int i = 0; i < 10; i++)
            {
                GameObject go = new GameObject(ghSharpDX.resourceViewLife, this);
                go.size = 0.5f;
                go.originX = 160f+(32*i);
                go.originY = -270f;
                go.alwaysHidden = true;
                lifeUI.Add(go);
            }
            updateLifeCount();

            //init stage
            level.loadStage01();

        }

        private void playingLogicUpdate() // TODO: Level Design Implementation for multilevels
        {
            // GameOver check, prompt user for a choice afterwards
            if (gameOver)
            {
                gameOverLogic();
                return;
            }
            // Handling pause the hard way
            if (pause) if (pauseLogic()) return; // if quit is selected, no bother finishing the method
            if (ihSharpDX.kEscape && !ihSharpDX.kEscapeOnce)
            {
                ihSharpDX.kEscapeOnce = true;
                pause = !pause;
                pauseMenuOption = 0;
            }
            if (pause) return;

            //Update counter and stuffs
            frameCountFire++;
            invincibleTimer++;

            //Global Logic
            checkCollisionFull(listPlayerObject[1]);
            checkCollisionPlayerBullet();
            if (invincibleTimer > 0.1 * fps) darkVision = false;
            if (invincibleTimer > 3 * fps) isInvincible = false;
            

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
                        listPlayerBulletObject[playerBulletCount].damage = 20;
                    }
                    else
                    {
                        bulletDelay = 0f;
                        listPlayerBulletObject[playerBulletCount].speed = 360f;
                        listPlayerBulletObject[playerBulletCount].resourceViewSharpDX = ghSharpDX.resourceViewPlayerBullet00;
                        listPlayerBulletObject[playerBulletCount].damage = 10;
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
            foreach (EnemyObject eo in listEnemyObject)
            {
                if (collisionObject.Contains(eo)) eo.hitboxType = 1;
                else eo.hitboxType = 0;
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
                    foreach (EnemyObject eo in listEnemyObject)
                    {
                        if (eo.isVisible && eo.isActive)
                        {

                            if (eo.collideWith(bo))
                            {
                                collisionEnemy.Add(new GameObjectDuo(eo, bo));
                                eo.health = eo.health - bo.damage;
                                if (bo.damage <= 10) bo.isActive = false;
                                if (eo.health <= 0)
                                {
                                    eo.isActive = false;
                                    eo.isAlive = false;
                                    score = score + 100;
                                    if (eo.type == EnemyObject.ENEMYTYPE.BOSS)
                                    {
                                        score = score + 2400;
                                        isInvincible = true;
                                        invincibleTimer = 2 * fps;
                                    }
                                }
                                Console.WriteLine("State: "+eo.state+" Type: "+eo.type+" Health: "+eo.health);
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
                        playerLoseLife();
                    }
                }
            }
            foreach (EnemyObject eo in listEnemyObject)
            {
                if (eo.isVisible && eo.isActive)
                {
                    if (mainGo.collideWith(eo))
                    {
                        collisionObject.Add(eo);
                        playerLoseLife();
                    }
                }
            }

            foreach (BulletObject bo in listBulletObject)
            {
                if (bo.isVisible && bo.isActive)
                {
                    // real collision 
                    if (mainGo.collideCircleWith(bo, ((bo.txPointerW * bo.size * bo.hitboxFactor) / 2f) * 0.8f))
                    {
                        collisionObject.Add(bo);
                        playerLoseLife();
                    }
                    // grazing
                    if (mainGo.collideCircleWith(bo, ((bo.txPointerW * bo.size * bo.hitboxFactor) / 2f) * 1.1f))
                    {
                        //collisionObject.Add(listBulletObject[i]);
                    }
                }
            }

        }

        private void playerLoseLife()
        {
            if (isInvincible) return;
            if (lives <= 0) gameOver = true;
            lives--;
            invincibleTimer = 0;
            isInvincible = true;
            darkVision = true;
            updateLifeCount();
        }

        private void updateLifeCount()
        {
            int i = 0;
            foreach (GameObject go in lifeUI)
            {
                go.alwaysHidden = false;
                if (i >= lives) go.alwaysHidden = true;
                i++;
            }
        }

        private void gameOverLogic()
        {
            if (ihSharpDX.kEnter && !ihSharpDX.kEnterOnce || ihSharpDX.kY && !ihSharpDX.kYOnce)
            {
                ihSharpDX.kEnterOnce = true;
                ihSharpDX.kYOnce = true;
                if (gameOverMenuOption == 0)
                {
                    gameOver = false;
                    lives = 3;
                    updateLifeCount();
                }
                if (gameOverMenuOption == 1)
                {
                    cleanGameState();
                    gameState = (int)GAMESTATE.MENU;
                    loadMainMenuGameState();
                }
            }
            if (ihSharpDX.kRight && !ihSharpDX.kRightOnce)
            {
                ihSharpDX.kRightOnce = true;
                gameOverMenuOption++;
                if (gameOverMenuOption > 1) gameOverMenuOption = 0;
            }
            if (ihSharpDX.kLeft && !ihSharpDX.kLeftOnce)
            {
                ihSharpDX.kLeftOnce = true;
                gameOverMenuOption--;
                if (gameOverMenuOption < 0) gameOverMenuOption = 1;
            }
        }

        private bool pauseLogic()
        {
            if (ihSharpDX.kEnter && !ihSharpDX.kEnterOnce || ihSharpDX.kY && !ihSharpDX.kYOnce)
            {
                ihSharpDX.kEnterOnce = true;
                ihSharpDX.kYOnce = true;
                ihSharpDX.kY = false; //bugfix for shooting bullet when resuming. not clean, but works
                if (pauseMenuOption == 0)
                {
                    pause = !pause;
                }
                if (pauseMenuOption == 1)
                {
                    cleanGameState();
                    gameState = (int)GAMESTATE.MENU;
                    loadMainMenuGameState();
                    return true;
                }
            }
            if (ihSharpDX.kRight && !ihSharpDX.kRightOnce)
            {
                ihSharpDX.kRightOnce = true;
                pauseMenuOption++;
                if (pauseMenuOption > 1) pauseMenuOption = 0;
            }
            if (ihSharpDX.kLeft && !ihSharpDX.kLeftOnce)
            {
                ihSharpDX.kLeftOnce = true;
                pauseMenuOption--;
                if (pauseMenuOption < 0) pauseMenuOption = 1;
            }
            return false;
        }

    //-----------------------------------END---OF---CLASS---------------------------------------------------------------
    }
}
