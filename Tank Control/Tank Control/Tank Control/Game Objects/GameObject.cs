using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Tank_Control.Game_Objects
{
    public abstract class GameObject 
    {
        protected Game game;
        protected Vector3 position;

        public GameObject(Game g)
        {
            this.game = g;
        }

        public GameObject(Game g, Vector3 p)
        {
            this.game = g;
            this.position = p;
        }

        public abstract void LoadContent(ContentManager contentMan);

        public abstract void Draw();

        public abstract void Update(double elapsedMillis);

        public Vector3 getPosition()
        {
            return position;
        }

    }
}
