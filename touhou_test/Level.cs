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

        public List<bool> timingEventDone;
        public int frameLogicCount = 0;
        public int bulletCount = 0;
        public float delay = 0;
        public float acceleration = 0;

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

            //enemy
            listGameObject.Add(new GameObject(ghSharpDX.resourceViewKaguya, gl));
            listGameObject[0].size = 0.5f;
            listGameObject[0].hitboxFactor = 0.4f;
            listGameObject[0].offsetY = -8;
            listGameObject[0].originX = 0f;
            listGameObject[0].originY = -500f;

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


            // hitbox base data creation + original object final coords update
            /*
            foreach (GameObject go in listPlayerObject)
            {
                GameObject newGo = new GameObject(ghSharpDX.resourceViewHitboxGreen, gl);
                listHitbox.Add(newGo);
            }
            foreach (GameObject go in listGameObject)
            {
                GameObject newGo = new GameObject(ghSharpDX.resourceViewHitboxGreen, gl);
                listHitbox.Add(newGo);
            }
            foreach (BulletObject bo in listBulletObject)
            {
                GameObject newGo = new GameObject(ghSharpDX.resourceViewHitboxGreen, gl);
                newGo.alwaysHidden = true;
                listHitbox.Add(newGo);
            }
            foreach (BulletObject bo in listPlayerBulletObject)
            {
                GameObject newGo = new GameObject(ghSharpDX.resourceViewHitboxGreen, gl);
                //newGo.alwaysHidden = true;
                listHitbox.Add(newGo);
            }
            */
        }

        public void logicStage01() {

            //Regular Updates
            int fps = gl.fps;

            //Timing Logic - Level design
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
                    listBulletObject[bulletCount + i].size = 2f;
                    listBulletObject[bulletCount + i].speed = 60f;
                }

                //if ((bulletCount / bulletBatch) % 10 == 9)
                //{
                    delay = 1.5f;
                //}
                //else
                //{
                //    delay = 0f;
                //}
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
                    //delay = 0.5f;
                    //timingEventDone[1] = true;
                }
                //timingEventDone[1] = true;
                frameLogicCount = 0;

            }
            frameLogicCount++;
        
        }

    }
}
