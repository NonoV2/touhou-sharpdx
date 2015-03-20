﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SharpHelper;
using SharpDX;
using SharpDX.Windows;
using SharpDX.Direct3D11;

namespace touhou_test
{
    class GraphicHandlerSharpDX
    {

        public Game g;
        public RenderForm form;
        public SharpDevice device;
        public SharpBatch batch;
        public SharpFPS fpsCounter;
        public BlendState bs;
        public SharpDX.Toolkit.Graphics.BlendState bsToolkit;
        public SharpDX.Toolkit.Graphics.SamplerState ssToolkit;
        public SharpDX.Toolkit.Graphics.RasterizerState rsToolkit;
        public SharpDX.Toolkit.Graphics.RasterizerState rsToolkitWireframe;

        public ShaderResourceView resourceViewMenu;
        public ShaderResourceView resourceViewBackground;
        public ShaderResourceView resourceViewMokou;
        public ShaderResourceView resourceViewKaguya;
        public ShaderResourceView resourceViewHitboxGreen;
        public ShaderResourceView resourceViewHitboxRed;
        public ShaderResourceView resourceViewFocusHitbox;
        public ShaderResourceView resourceViewBullet00;

        public int windowW = 800;
        public int windowH = 600;

        public GraphicHandlerSharpDX(Game g) {
            this.g = g;
        }

        public void initRenderForm() {

            if (!SharpDevice.IsDirectX11Supported())
            {
                System.Windows.Forms.MessageBox.Show("DirectX11 Not Supported");
                return;
            }

            form = new RenderForm("Touhou test with SharpDX");
            form.Width = windowW;
            form.Height = windowH;
            form.Width = form.Width + (form.Width - form.ClientSize.Width);
            form.Height = form.Height + (form.Height - form.ClientSize.Height);
            //form.Focus();

            fpsCounter = new SharpFPS();
            device = new SharpDevice(form);

            // Handle Transparency using custom BlendState and RenderTarget
            BlendStateDescription bsd = BlendStateDescription.Default();

            bsd.AlphaToCoverageEnable = false;
            bsd.IndependentBlendEnable = false;

            bsd.RenderTarget[0].IsBlendEnabled = true;
            bsd.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;

            bsd.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
            bsd.RenderTarget[0].BlendOperation = BlendOperation.Add;
            bsd.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;

            bsd.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
            bsd.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
            bsd.RenderTarget[0].DestinationAlphaBlend = BlendOption.One;

            //create font from file (generated with tkfont.exe)
            batch = new SharpBatch(device, "font/textfont.dds");
            //batch = new SharpBatch(device, "font/arial64");

            bsToolkit = SharpDX.Toolkit.Graphics.BlendState.New(batch.Batch.GraphicsDevice, bsd);
            //batch.Batch.GraphicsDevice.SetBlendState(bsToolkit);
            bs = new BlendState(device.Device, bsd);
            //device.DeviceContext.OutputMerger.SetBlendState(bs);
            SamplerStateDescription ssd = new SamplerStateDescription();
            ssd.AddressU = TextureAddressMode.Wrap;
            ssd.AddressV = TextureAddressMode.Wrap;
            ssd.AddressW = TextureAddressMode.Wrap;
            ssd.Filter = Filter.MinLinearMagMipPoint;
            ssToolkit = SharpDX.Toolkit.Graphics.SamplerState.New(batch.Batch.GraphicsDevice, ssd);

            RasterizerStateDescription rsd = RasterizerStateDescription.Default();
            rsToolkit = SharpDX.Toolkit.Graphics.RasterizerState.New(batch.Batch.GraphicsDevice, rsd);
            RasterizerStateDescription rsdWireframe = RasterizerStateDescription.Default();
            rsdWireframe.FillMode = FillMode.Wireframe;
            rsToolkitWireframe = SharpDX.Toolkit.Graphics.RasterizerState.New(batch.Batch.GraphicsDevice, rsdWireframe);

            //experimental section, may fail in some epic proportion
            //SharpDX.Direct2D1.DeviceContext d2dContext = new SharpDX.Direct2D1.DeviceContext();


        }

        public void initAllTexturesFromFiles()
        {

            try
            {
                resourceViewMenu = ShaderResourceView.FromFile(device.Device, "img/wallpaper.jpg");
                resourceViewBackground = ShaderResourceView.FromFile(device.Device, "img/bg_00.png");
                resourceViewMokou = ShaderResourceView.FromFile(device.Device, "img/mokou_00.png");
                resourceViewKaguya = ShaderResourceView.FromFile(device.Device, "img/kaguya_00.png");
                resourceViewHitboxGreen = ShaderResourceView.FromFile(device.Device, "img/hitbox_green.png");
                resourceViewHitboxRed = ShaderResourceView.FromFile(device.Device, "img/hitbox_red.png");
                resourceViewFocusHitbox = ShaderResourceView.FromFile(device.Device, "img/player_hitbox.png");
                resourceViewBullet00 = ShaderResourceView.FromFile(device.Device, "img/bullet_white_8x8.png");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Cannot load texture images, aborting process: " + ex.ToString() + " " + ex.StackTrace);
                System.Console.ReadLine();
                Environment.Exit(-1);
            }

        }

        public void drawGameObject(GameObject go)
        {
            if (go == null) return;
            if (go.alwaysVisible || go.isVisibleByCamera() && !go.alwaysHidden) go.Draw();
            else go.updateFinalOriginAndCoord();
        }

        public void Dispose() {

            rsToolkit.Dispose();
            rsToolkitWireframe.Dispose();
            ssToolkit.Dispose();
            bsToolkit.Dispose();
            bs.Dispose();
            device.Dispose();

            resourceViewMenu.Dispose();
            resourceViewBackground.Dispose();
            resourceViewMokou.Dispose();
            resourceViewKaguya.Dispose();
            resourceViewHitboxGreen.Dispose();
            resourceViewHitboxRed.Dispose();
            resourceViewFocusHitbox.Dispose();
            resourceViewBullet00.Dispose();

        }

    //----------------------------------------------------------------------------------------------------------
    }
}