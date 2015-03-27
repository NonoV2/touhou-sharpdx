using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D11;

namespace touhou_test
{
    class EnemyObject : BulletObject
    {

        public enum ENEMYTYPE : int {NORMAL = 0, BOSS = 1, MINION = 2}
        public ENEMYTYPE type = ENEMYTYPE.NORMAL;
        public int health = 0;
        public int state = 0;
        public bool isAlive = false;

        public EnemyObject(ShaderResourceView resourceView, GameLogic gl) : base(resourceView, gl)
        {
            
        }


    }
}
