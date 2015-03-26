using System;
using System.Windows.Forms;

namespace touhou_test
{
    class InputHandlerSharpDX
    {
        Game g;
        GraphicHandlerSharpDX gh;
        //Publicly accessible key states

        public bool kDown = false;
        public bool kDownOnce = false;
        public bool kUp = false;
        public bool kUpOnce = false;
        public bool kRight = false;
        public bool kRightOnce = false;
        public bool kLeft = false;
        public bool kLeftOnce = false;
        public bool kPlus = false;
        public bool kPlusOnce = false;
        public bool kMinus = false;
        public bool kMinusOnce = false;
        public bool kMultiply = false;
        public bool kMultiplyOnce = false;
        public bool kNumpad5 = false;
        public bool kNumpad5Once = false;
        public bool kEscape = false;
        public bool kEscapeOnce = false;
        public bool kEnter = false;
        public bool kEnterOnce = false;
        public bool kD = false;
        public bool kDOnce = false;
        public bool kW = false;
        public bool kWOnce = false;
        public bool kC = false;
        public bool kCOnce = false;
        public bool kF = false;
        public bool kFOnce = false;
        public bool kShift = false;
        public bool kH = false;
        public bool kHOnce = false;
        public bool kY = false;
        public bool kYOnce = false;
        public bool kX = false;
        public bool kXOnce = false;

        public InputHandlerSharpDX(Game g)
        {
            this.g = g;
            this.gh = g.ghSharpDX;
        }

        public void initAllEventListener()
        {

            gh.form.KeyDown += form_KeyDown;

            gh.form.KeyUp += form_KeyUp;

        }

        private void form_KeyDown(object sender, KeyEventArgs e)
        {
            // handle alt+enter ourselves
            if (e.Alt && e.KeyCode == Keys.Enter)
            {
                gh.device.SwapChain.SetFullscreenState(!gh.device.SwapChain.IsFullScreen, null);
            }
            if (e.KeyCode == Keys.Up)
            {
                kUp = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                kRight = true;
            }
            if (e.KeyCode == Keys.Left)
            {
                kLeft = true;
            }
            if (e.KeyCode == Keys.Down)
            {
                kDown = true;
            }
            if (e.KeyCode == Keys.Add)
            {
                kPlus = true;
            }
            if (e.KeyCode == Keys.Subtract)
            {
                kMinus = true;
            }
            if (e.KeyCode == Keys.Multiply)
            {
                kMultiply = true;
            }
            if (e.KeyCode == Keys.NumPad5)
            {
                kNumpad5 = true;
            }
            if (e.KeyCode == Keys.Escape)
            {
                kEscape = true;
            }
            if (e.KeyCode == Keys.Enter)
            {
                kEnter = true;
            }
            if (e.KeyCode == Keys.D)
            {
                kD = true;
            }
            if (e.KeyCode == Keys.W)
            {
                kW = true;
            }
            if (e.KeyCode == Keys.C)
            {
                kC = true;
            }
            if (e.KeyCode == Keys.F)
            {
                kF = true;
            }
            if (e.KeyCode == Keys.Y)
            {
                kY = true;
            }
            if (e.KeyCode == Keys.X)
            {
                kX = true;
            }
            if (e.KeyCode == Keys.ShiftKey)
            {
                kShift = true;
            }
            if (e.KeyCode == Keys.H)
            {
                kH = true;
            }
        }

        private void form_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                kUp = false;
                kUpOnce = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                kRight = false;
                kRightOnce = false;
            }
            if (e.KeyCode == Keys.Left)
            {
                kLeft = false;
                kLeftOnce = false;
            }
            if (e.KeyCode == Keys.Down)
            {
                kDown = false;
                kDownOnce = false;
            }
            if (e.KeyCode == Keys.Add)
            {
                kPlus = false;
                kPlusOnce = false;
            }
            if (e.KeyCode == Keys.Subtract)
            {
                kMinus = false;
                kMinusOnce = false;
            }
            if (e.KeyCode == Keys.Multiply)
            {
                kMultiply = false;
                kMultiplyOnce = false;
            }
            if (e.KeyCode == Keys.NumPad5)
            {
                kNumpad5 = false;
                kNumpad5Once = false;
            }
            if (e.KeyCode == Keys.Escape)
            {
                kEscape = false;
                kEscapeOnce = false;
            }
            if (e.KeyCode == Keys.Enter)
            {
                kEnter = false;
                kEnterOnce = false;
            }
            if (e.KeyCode == Keys.D)
            {
                kD = false;
                kDOnce = false;
            }
            if (e.KeyCode == Keys.W)
            {
                kW = false;
                kWOnce = false;
            }
            if (e.KeyCode == Keys.C)
            {
                kC = false;
                kCOnce = false;
            }
            if (e.KeyCode == Keys.F)
            {
                kF = false;
                kFOnce = false;
            }
            if (e.KeyCode == Keys.Y)
            {
                kY = false;
                kYOnce = false;
            }
            if (e.KeyCode == Keys.X)
            {
                kX = false;
                kXOnce = false;
            }
            if (e.KeyCode == Keys.ShiftKey)
            {
                kShift = false;
            }
            if (e.KeyCode == Keys.H)
            {
                kH = false;
                kHOnce = false;
            }
        }


        //----------------------------------------------END---OF---CLASS----------------------------------------------
    }
}
