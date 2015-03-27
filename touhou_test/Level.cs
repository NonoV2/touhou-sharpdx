using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace touhou_test
{
    class Level // Abstraction of all game levels. World initialization should be done here.
    {

        GameLogic gl;
        GraphicHandlerSharpDX ghSharpDX;

        public List<GameObject> listBackgroundObject;
        public List<GameObject> listPlayerObject;
        public List<GameObject> listGameObject;
        public List<BulletObject> listBulletObject;
        public List<GameObject> collisionObject;
        public List<BulletObject> listPlayerBulletObject;
        public List<EnemyObject> listEnemyObject;

        public List<bool> timingEventDone;
        public int frameLogicCount = 0;
        public int bulletCount = 0;
        public float delay = 0;
        public float acceleration = 0;
        public bool stageNameIsVisible = true;

        public Level(GameLogic gl)
        {
            this.gl = gl;
            this.ghSharpDX = gl.ghSharpDX;
            this.listBackgroundObject = gl.listBackgroundObject;
            this.listBulletObject = gl.listBulletObject;
            this.listGameObject = gl.listGameObject;
            this.listPlayerObject = gl.listPlayerObject;
            this.collisionObject = gl.collisionObject;
            this.listPlayerBulletObject = gl.listPlayerBulletObject;
            this.listEnemyObject = gl.listEnemyObject;
            
            //inits
            timingEventDone = new List<bool>();
            for (int i = 0; i < 100; i++) timingEventDone.Add(false);
        }

        public void loadStage01()
        {
            //LoadBackground
            listBackgroundObject.Add(new GameObject(ghSharpDX.resourceViewBackground, gl));
            listBackgroundObject[0].originY = 0f;
            //listBackgroundObject[0].alwaysVisible = true;

            listBackgroundObject.Add(new GameObject(ghSharpDX.resourceViewBackground, gl));
            listBackgroundObject[1].originY = -1799f;
            //listBackgroundObject[1].alwaysVisible = true;

            //Player = 0
            listPlayerObject.Add(new GameObject(ghSharpDX.resourceViewMokou, gl));
            listPlayerObject[0].size = 0.5f;
            listPlayerObject[0].originX = 0f;
            listPlayerObject[0].originY = 220f;

            //player hitbox = 1
            listPlayerObject.Add(new GameObject(ghSharpDX.resourceViewFocusHitbox, gl));
            listPlayerObject[1].hitboxFactor = 0.33f;

            //enemies base init
            for (int i = 0; i < 100; i++ ) listEnemyObject.Add(new EnemyObject(ghSharpDX.resourceViewKaguya, gl));


            //bullets
            for (int i = 0; i < 2000; i++)
            {
                BulletObject bo = new BulletObject(ghSharpDX.resourceViewBullet00, gl);
                bo.isHitboxActive = false;
                listBulletObject.Add(bo);
            }

            for (int i = 0; i < 100; i++)
            {
                BulletObject bo = new BulletObject(ghSharpDX.resourceViewPlayerBullet00, gl);
                bo.isHitboxActive = false;
                listPlayerBulletObject.Add(bo);
            }

        }

        public void logicStage01() {

            //Regular Updates
            frameLogicCount++;
            int fps = gl.fps;

            //Timing Logic - Level design

            int firstWave = 16;
            if (!timingEventDone[0] && frameLogicCount > 5 * fps) //init first enemies
            {
                stageNameIsVisible = false;

                for (int i = 0; i < firstWave; i++)
                {
                    listEnemyObject[i].size = 0.5f;
                    listEnemyObject[i].hitboxFactor = 0.4f;
                    listEnemyObject[i].offsetY = -8;
                    listEnemyObject[i].originX = -250f;
                    listEnemyObject[i].originY = -500f - (i*40f);
                    listEnemyObject[i].health = 1;
                    listEnemyObject[i].state = 0;
                    listEnemyObject[i].speed = 100f;
                    listEnemyObject[i].type = EnemyObject.ENEMYTYPE.NORMAL;
                    listEnemyObject[i].isActive = true;
                    listEnemyObject[i].isAlive = true;
                }
                timingEventDone[0] = true;
            }

            if (timingEventDone[0] && !timingEventDone[1])
            {
                for (int i = 0; i < firstWave; i++)
                {
                    listEnemyObject[i].isActive = listEnemyObject[i].isAlive;
                    if (listEnemyObject[i].state == 0)
                    {
                        listEnemyObject[i].trackAndFollow(-250f, listEnemyObject[i].originY+10f);
                    }
                    if (listEnemyObject[i].state == 1)
                    {
                        listEnemyObject[i].trackAndFollow(250f, -150f);
                    }
                    if (listEnemyObject[i].state == 2)
                    {
                        listEnemyObject[i].trackAndFollow(250f, listEnemyObject[i].originY+10f);
                    }

                    if (listEnemyObject[i].state == 0 && listEnemyObject[i].originY > 200f)
                    {
                        listEnemyObject[i].state = 1;
                    }
                    if (listEnemyObject[i].state == 1 && listEnemyObject[i].originX >= 250f)
                    {
                        listEnemyObject[i].state = 2;
                    }
                }
   
            }           

            
            if (!timingEventDone[19] && frameLogicCount > 21 * fps) // X seconds before boss spawns
            {
                //Reset Global Variables
                acceleration = 0;
                bulletCount = 0;
                frameLogicCount = 0;

                //Boss initialization
                listEnemyObject[50].size = 1f;
                listEnemyObject[50].hitboxFactor = 0.4f;
                listEnemyObject[50].offsetY = -16;
                listEnemyObject[50].originX = 0f;
                listEnemyObject[50].originY = -500f;
                listEnemyObject[50].health = 10000;
                listEnemyObject[50].type = EnemyObject.ENEMYTYPE.BOSS;
                listEnemyObject[50].isActive = true;
                listEnemyObject[50].isAlive = true;

                //Event completed
                timingEventDone[19] = true;
                
            }
            if (timingEventDone[19] && !timingEventDone[20])
            {
                listEnemyObject[50].isActive = listEnemyObject[50].isAlive;
                listEnemyObject[50].originY = listEnemyObject[50].originY + ((100f - acceleration) / fps);
                if (listEnemyObject[50].originY > -200)
                {
                    listEnemyObject[50].originY = -200;
                    timingEventDone[20] = true;
                    frameLogicCount = 0;
                    acceleration = 0f;
                }
                if (listEnemyObject[50].originY > -250)
                {
                    acceleration = acceleration + (100f / fps);
                    if (acceleration > 80f) { acceleration = 90f; }
                }

            }
            if (timingEventDone[20] && !timingEventDone[21] && frameLogicCount > 1.75f * fps)
            {
                timingEventDone[21] = true;
                frameLogicCount = 0;
            }
            if (timingEventDone[21] && !timingEventDone[22] && frameLogicCount > (0.5f + delay) * fps)
            {
                frameLogicCount = 0;
                int bulletBatch = 20;
                for (int i = 0; i < bulletBatch; i++)
                {
                    listBulletObject[bulletCount + i].originX = listEnemyObject[50].originX - (20 * bulletBatch) + (i * 4 * bulletBatch);
                    listBulletObject[bulletCount + i].originY = listEnemyObject[50].originY - 30;
                    listBulletObject[bulletCount + i].trackPlayerData(listPlayerObject[1].originX - (20 * bulletBatch) + (i * 4 * bulletBatch), listPlayerObject[1].originY);
                    listBulletObject[bulletCount + i].isActive = true;
                    listBulletObject[bulletCount + i].size = 2f;
                    listBulletObject[bulletCount + i].speed = 60f;
                }

                delay = 1.5f;

                if ((bulletCount / bulletBatch) % 20 > 9)
                {
                    delay = 0f;
                    for (int i = 0; i < bulletBatch; i++)
                    {
                        listBulletObject[bulletCount + i].speed = 180f;
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
                }
            }
            if (timingEventDone[21] && !timingEventDone[22] && listEnemyObject[50].health <= 0)
            {
                //forgotten, cleaning
                timingEventDone[1] = true;

                //new, must be done
                bulletCount = 0;
                clearAllBullets();
                timingEventDone[22] = true;
                frameLogicCount = 0;
            }
        


        }

        private void clearAllBullets() //TODO: Add explosion SFX or animation later on...
        {
            foreach (BulletObject bo in listBulletObject) bo.isActive = false;
        }


    }
}
