using System;
//using System.Collections.Generic;
using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;

namespace touhou_test
{
    class GameObjectDuo
    {
        public GameObject go;
        public BulletObject bo;

        public GameObjectDuo(GameObject go, BulletObject bo)
        {
            this.go = go;
            this.bo = bo;
        }
    }

    class GameObject
    {
        protected GameLogic gl;

        public float size = 1.0f; // external multipliser to use for scaling things
        public int hitboxType = 0;

        public float coordX = 1.0f; // 0.5f;
        public float coordY = 1.0f; // 0.5f;
        public float originX = 0f;
        public float originY = 0f;
        public float finalOriginX = 0f;
        public float finalOriginY = 0f;
        public float finalCoordX = 1f;
        public float finalCoordY = 1f;
        public float hitboxFactor = 1f;
        public float offsetX = 0f;
        public float offsetY = 0f;

        public float speedX = 0f;
        public float speedY = 0f;

        public SharpDX.Direct3D11.ShaderResourceView resourceViewSharpDX;
        public float txPointerW = 1f;
        public float txPointerH = 1f;

        public bool alwaysVisible = false;
        public bool alwaysHidden = false;
        public bool isVisible = false;
        public bool isHitboxActive = true;


        public GameObject(SharpDX.Direct3D11.ShaderResourceView resourceView, GameLogic gl)
        {
            this.gl = gl;
            this.resourceViewSharpDX = resourceView;
            updateTextureSizeDescription();
        }

        public void updateTextureSizeDescription() {
            using (var resource = resourceViewSharpDX.Resource)
            {
                var texture2D = new SharpDX.Direct3D11.Texture2D(resource.NativePointer);
                this.txPointerW = texture2D.Description.Width;
                this.txPointerH = texture2D.Description.Height;
                this.coordX = txPointerW / 2f;
                this.coordY = txPointerH / 2f;
            }
        }

        public virtual void Draw(SharpDX.Color color)
        {
            float wFactor = (float)gl.ghSharpDX.form.ClientSize.Width / (float)gl.ghSharpDX.windowW;
            float hFactor = (float)gl.ghSharpDX.form.ClientSize.Height / (float)gl.ghSharpDX.windowH;
            
            //gl.ghSharpDX.batch.Batch.Draw(resourceViewSharpDX, new SharpDX.Vector2(finalOriginX, finalOriginY), SharpDX.Color.White);
            gl.ghSharpDX.batch.Batch.Draw(
                resourceViewSharpDX, // texture
                new SharpDX.Vector2(finalOriginX*wFactor + (gl.ghSharpDX.form.ClientSize.Width / 2f), finalOriginY*hFactor + (gl.ghSharpDX.form.ClientSize.Height / 2f)), // position in the screen
                //new SharpDX.Vector2(finalOriginX + 400f, finalOriginY + 300f), // position in the screen
                new SharpDX.Rectangle(0, 0, (int)txPointerW, (int)txPointerH), // subpart of the texture as RectangleF
                color, // tint color, use white for normal color
                0, // rotation in radians
                new SharpDX.Vector2(txPointerW / 2f, txPointerH / 2f), // origin of the texture object (should be centered to reuse SlimDX code)
                new SharpDX.Vector2(size * wFactor, size * hFactor), // scaling factor horizontal, vertical
                SharpDX.Toolkit.Graphics.SpriteEffects.None, // mirror effects
                0 // depth layer
           );


        }

        public virtual void DrawFlipVertically(SharpDX.Color color)
        {

            float wFactor = (float)gl.ghSharpDX.form.ClientSize.Width / (float)gl.ghSharpDX.windowW;
            float hFactor = (float)gl.ghSharpDX.form.ClientSize.Height / (float)gl.ghSharpDX.windowH;
            
            //gl.ghSharpDX.batch.Batch.Draw(resourceViewSharpDX, new SharpDX.Vector2(finalOriginX, finalOriginY), SharpDX.Color.White);
            gl.ghSharpDX.batch.Batch.Draw(
                resourceViewSharpDX, // texture
                new SharpDX.Vector2(finalOriginX * wFactor + (gl.ghSharpDX.form.ClientSize.Width / 2f), finalOriginY * hFactor + (gl.ghSharpDX.form.ClientSize.Height / 2f)), // position in the screen
                //new SharpDX.Vector2(finalOriginX + 400f, finalOriginY + 300f), // position in the screen
                new SharpDX.Rectangle(0, 0, (int)txPointerW, (int)txPointerH), // subpart of the texture as RectangleF
                color, // tint color, use white for normal color
                0, // rotation in radians
                new SharpDX.Vector2(txPointerW / 2f, txPointerH / 2f), // origin of the texture object (should be centered to reuse SlimDX code)
                new SharpDX.Vector2(size * wFactor, size * hFactor), // scaling factor horizontal, vertical
                SharpDX.Toolkit.Graphics.SpriteEffects.FlipVertically, // mirror effects
                0 // depth layer
           );

        }

        public void DrawHitbox()
        {
            if (isHitboxActive == false) return;
            ShaderResourceView tx = gl.ghSharpDX.resourceViewHitboxGreen;
            if (hitboxType == 1)
            {
                tx = gl.ghSharpDX.resourceViewHitboxRed;
            }
            if (hitboxType == 2)
            {
                tx = gl.ghSharpDX.resourceViewHitboxOrange;
            }
            float hitboxTextureSize = 64.0f;
            float sizeHitbox = (txPointerH / hitboxTextureSize) * size * hitboxFactor;
            float wFactor = (float)gl.ghSharpDX.form.ClientSize.Width / (float)gl.ghSharpDX.windowW;
            float hFactor = (float)gl.ghSharpDX.form.ClientSize.Height / (float)gl.ghSharpDX.windowH;
            float finalOriginXcopy = finalOriginX - offsetX;
            float finalOriginYcopy = finalOriginY - offsetY;

            gl.ghSharpDX.batch.Batch.Draw(
                tx, // texture
                new SharpDX.Vector2(finalOriginXcopy * wFactor + (gl.ghSharpDX.form.ClientSize.Width / 2f), finalOriginYcopy * hFactor + (gl.ghSharpDX.form.ClientSize.Height / 2f)), // position in the screen
                //new SharpDX.Vector2(finalOriginX + 400f, finalOriginY + 300f), // position in the screen
                null, // subpart of the texture as RectangleF
                new SharpDX.Color(new Vector4(1f,1f,1f,0.40f)), // tint color 40% Alpha
                0, // rotation in radians
                new SharpDX.Vector2(hitboxTextureSize / 2f, hitboxTextureSize / 2f), // origin of the texture object (should be centered to reuse SlimDX code)
                new SharpDX.Vector2(sizeHitbox * wFactor, sizeHitbox * hFactor), // scaling factor horizontal, vertical
                SharpDX.Toolkit.Graphics.SpriteEffects.FlipVertically, // mirror effects
                0 // depth layer
           );

        }

        public void updateFinalOriginAndCoord() {

            finalOriginX = (originX - gl.cameraOriginX); // -offsetX;
            finalOriginY = (originY - gl.cameraOriginY); // -offsetY;
            finalCoordX = coordX * size;
            finalCoordY = coordY * size;

        }

        public virtual bool isVisibleByCamera() {
            isVisible = false;
            if (originX - gl.cameraOriginX < ( gl.ghSharpDX.form.ClientSize.Width  + txPointerW * size) / 2f &&
                originX - gl.cameraOriginX > (-gl.ghSharpDX.form.ClientSize.Width - txPointerW * size) / 2f &&
                originY - gl.cameraOriginY < (gl.ghSharpDX.form.ClientSize.Height + txPointerH * size) / 2f &&
                originY - gl.cameraOriginY > (-gl.ghSharpDX.form.ClientSize.Height - txPointerH * size) / 2f) isVisible = true;
            return isVisible;

        }

        // Reference to Screen Coordinate if needed
        /*
        public void showHitbox(GraphicHandler gh, Pen p) {

            Graphics g = gh.formGraphics;
            int windowW = gh.windowW;
            int windowH = gh.windowH;
            // Vector3 to Screen Coordinates
            g.DrawRectangle(p, ((originX + windowW - txPointerW * size) / 2f), ((-originY + windowH - txPointerH * size) / 2f),
                            txPointerW * size, txPointerH * size);

        }
        */
 
        public bool collideWith(GameObject go) {

            // offset hitbox ajustments
            float finalOriginXcopy = finalOriginX - offsetX;
            float finalOriginYcopy = finalOriginY - offsetY;
            float goFinalOriginX = go.finalOriginX - go.offsetX;
            float goFinalOriginY = go.finalOriginY - go.offsetY;

            // bigger than go

            if (finalOriginXcopy + finalCoordX * hitboxFactor <= goFinalOriginX + go.finalCoordX * go.hitboxFactor &&
                finalOriginXcopy + finalCoordX * hitboxFactor >= goFinalOriginX - go.finalCoordX * go.hitboxFactor &&
                finalOriginYcopy + finalCoordY * hitboxFactor <= goFinalOriginY + go.finalCoordY * go.hitboxFactor &&
                finalOriginYcopy + finalCoordY * hitboxFactor >= goFinalOriginY - go.finalCoordY * go.hitboxFactor) return true;

            if (finalOriginXcopy - finalCoordX * hitboxFactor <= goFinalOriginX + go.finalCoordX * go.hitboxFactor &&
                finalOriginXcopy - finalCoordX * hitboxFactor >= goFinalOriginX - go.finalCoordX * go.hitboxFactor &&
                finalOriginYcopy + finalCoordY * hitboxFactor <= goFinalOriginY + go.finalCoordY * go.hitboxFactor &&
                finalOriginYcopy + finalCoordY * hitboxFactor >= goFinalOriginY - go.finalCoordY * go.hitboxFactor) return true;

            if (finalOriginXcopy - finalCoordX * hitboxFactor <= goFinalOriginX + go.finalCoordX * go.hitboxFactor &&
                finalOriginXcopy - finalCoordX * hitboxFactor >= goFinalOriginX - go.finalCoordX * go.hitboxFactor &&
                finalOriginYcopy - finalCoordY * hitboxFactor <= goFinalOriginY + go.finalCoordY * go.hitboxFactor &&
                finalOriginYcopy - finalCoordY * hitboxFactor >= goFinalOriginY - go.finalCoordY * go.hitboxFactor) return true;

            if (finalOriginXcopy + finalCoordX * hitboxFactor <= goFinalOriginX + go.finalCoordX * go.hitboxFactor &&
                finalOriginXcopy + finalCoordX * hitboxFactor >= goFinalOriginX - go.finalCoordX * go.hitboxFactor &&
                finalOriginYcopy - finalCoordY * hitboxFactor <= goFinalOriginY + go.finalCoordY * go.hitboxFactor &&
                finalOriginYcopy - finalCoordY * hitboxFactor >= goFinalOriginY - go.finalCoordY * go.hitboxFactor) return true;

            // smaller than go

            if (go.finalOriginX + go.finalCoordX * go.hitboxFactor < finalOriginX + finalCoordX * hitboxFactor &&
                go.finalOriginX + go.finalCoordX * go.hitboxFactor > finalOriginX - finalCoordX * hitboxFactor &&
                go.finalOriginY + go.finalCoordY * go.hitboxFactor < finalOriginY + finalCoordY * hitboxFactor &&
                go.finalOriginY + go.finalCoordY * go.hitboxFactor > finalOriginY - finalCoordY * hitboxFactor) return true;

            if (go.finalOriginX - go.finalCoordX * go.hitboxFactor < finalOriginX + finalCoordX * hitboxFactor &&
                go.finalOriginX - go.finalCoordX * go.hitboxFactor > finalOriginX - finalCoordX * hitboxFactor &&
                go.finalOriginY + go.finalCoordY * go.hitboxFactor < finalOriginY + finalCoordY * hitboxFactor &&
                go.finalOriginY + go.finalCoordY * go.hitboxFactor > finalOriginY - finalCoordY * hitboxFactor) return true;

            if (go.finalOriginX - go.finalCoordX * go.hitboxFactor < finalOriginX + finalCoordX * hitboxFactor &&
                go.finalOriginX - go.finalCoordX * go.hitboxFactor > finalOriginX - finalCoordX * hitboxFactor &&
                go.finalOriginY - go.finalCoordY * go.hitboxFactor < finalOriginY + finalCoordY * hitboxFactor &&
                go.finalOriginY - go.finalCoordY * go.hitboxFactor > finalOriginY - finalCoordY * hitboxFactor) return true;

            if (go.finalOriginX + go.finalCoordX * go.hitboxFactor < finalOriginX + finalCoordX * hitboxFactor &&
                go.finalOriginX + go.finalCoordX * go.hitboxFactor > finalOriginX - finalCoordX * hitboxFactor &&
                go.finalOriginY - go.finalCoordY * go.hitboxFactor < finalOriginY + finalCoordY * hitboxFactor &&
                go.finalOriginY - go.finalCoordY * go.hitboxFactor > finalOriginY - finalCoordY * hitboxFactor) return true;

            return false;

        }

        public bool collideCircleWith(GameObject go, float radius)
        {
            // offset hitbox ajustments
            finalOriginX = finalOriginX - offsetX;
            finalOriginY = finalOriginY - offsetY;
            go.finalOriginX = go.finalOriginX - go.offsetX;
            go.finalOriginY = go.finalOriginY - go.offsetY;

            if (Math.Abs(finalOriginX - go.finalOriginX) < radius && Math.Abs(finalOriginY - go.finalOriginY) < radius) return true;

            return false;
        }

        public void deepCopyFrom(GameObject go)
        {
            this.size = go.size;
            //this.ratioX = go.ratioX;
            //this.ratioY = go.ratioY;
            //this.baseRatioX = go.baseRatioX;
            //this.baseRatioY = go.baseRatioY;
            this.coordX = go.coordX;
            this.coordY = go.coordY;
            this.originX = go.originX;
            this.originY = go.originY;
            this.finalOriginX = go.finalOriginX;
            this.finalOriginY = go.finalOriginY;
            this.finalCoordX = go.finalCoordX;
            this.finalCoordY = go.finalCoordY;

            //this.offsetX = go.offsetX;
            //this.offsetY = go.offsetY;

            //this.txPointer = go.txPointer;
            //this.resourceView = go.resourceView;
            //this.txPointerW = go.txPointerW;
            //this.txPointerH = go.txPointerH;
        }



    }
}
