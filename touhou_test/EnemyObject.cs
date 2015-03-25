using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D11;

namespace touhou_test
{
    class EnemyObject : GameObject
    {

        int type = 0;
        int health = 0;

        public EnemyObject(ShaderResourceView resourceView, GameLogic gl) : base(resourceView, gl)
        {
            
        }



    }
}
