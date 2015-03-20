using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace touhou_test
{
    class GraphicHandlerSlimDX
    {
        Game g;

        public DataStream vertices;
        public SlimDX.Direct3D11.Buffer vertexBuffer;
        public RenderForm form;
        public SwapChain swapChain;
        public DeviceContext context;
        public Viewport viewport;
        public RenderTargetView renderTarget;
        public SlimDX.Direct3D11.Device device;
        public Effect effect;
        public InputLayout layout;
        public ShaderSignature inputSignature;
        public VertexShader vertexShader;
        public PixelShader pixelShader;
        public SamplerState b;
        public BlendState bs;

        public RasterizerStateDescription rsd;
        public RasterizerState rs;

        //public Pen myPen;
        //public Pen myRedPen;
        //public Graphics formGraphics;

        public Texture2D txMenu;
        public Texture2D txMokou;
        public Texture2D txKaguya;
        public Texture2D txHitboxGreen;
        public Texture2D txHitboxRed;
        public ShaderResourceView resourceViewMenu;
        public ShaderResourceView resourceViewMokou;
        public ShaderResourceView resourceViewKaguya;
        public ShaderResourceView resourceViewHitboxGreen;
        public ShaderResourceView resourceViewHitboxRed;

        public int bufferCount = 3;
        public bool wireframe = false;

        public int windowW = 800;
        public int windowH = 600;

        public GraphicHandlerSlimDX(Game g) {
            this.g = g;
        }

        public void createWindowFor2D() {

            form = new RenderForm("Touhou test with SlimDX");
            form.Width = windowW;
            form.Height = windowH;
            form.Width = form.Width + (form.Width - form.ClientSize.Width);
            form.Height = form.Height + (form.Height - form.ClientSize.Height);

            //Handler for hitbox creation and display in 2D
            //myPen = new Pen(System.Drawing.Color.Green, 2);
            //myRedPen = new Pen(System.Drawing.Color.Red, 2);
            //formGraphics = form.CreateGraphics();

            //Handler for Direct3D11
            var description = new SwapChainDescription()
            {
                BufferCount = bufferCount,
                Usage = Usage.RenderTargetOutput,
                OutputHandle = form.Handle,
                IsWindowed = true,
                ModeDescription = new ModeDescription(form.ClientSize.Width, form.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                Flags = SwapChainFlags.None,
                SwapEffect = SwapEffect.Discard
            };

            SlimDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, description, out device, out swapChain);

            // create a view of our render target, which is the backbuffer of the swap chain we just created
            using (var resource = SlimDX.Direct3D11.Resource.FromSwapChain<Texture2D>(swapChain, 0))
                renderTarget = new RenderTargetView(device, resource);

            // setting a viewport is required if you want to actually see anything
            context = device.ImmediateContext;
            var viewport = new Viewport(0.0f, 0.0f, form.ClientSize.Width, form.ClientSize.Height);
            context.OutputMerger.SetTargets(renderTarget);
            context.Rasterizer.SetViewports(viewport);

            using (ShaderBytecode effectByteCode = ShaderBytecode.CompileFromFile("effect.fx", "Render", "fx_5_0", ShaderFlags.None, EffectFlags.None))
            effect = new Effect(device, effectByteCode);

            SamplerDescription a = new SamplerDescription();
            a.AddressU = TextureAddressMode.Wrap;
            a.AddressV = TextureAddressMode.Wrap;
            a.AddressW = TextureAddressMode.Wrap;
            a.Filter = Filter.Anisotropic;
            //a.Filter = Filter.MinMagMipPoint;

            b = SamplerState.FromDescription(device, a);
            context.PixelShader.SetSampler(b, 0);
            effect.GetVariableByName("TextureSampler").AsSampler().SetSamplerState(0, b);

            // load and compile the vertex shader
            using (var bytecode = ShaderBytecode.CompileFromFile("effect.fx", "vs_main", "vs_4_0", ShaderFlags.None, EffectFlags.None))
            {
                inputSignature = ShaderSignature.GetInputSignature(bytecode);
                vertexShader = new VertexShader(device, bytecode);
            }

            // load and compile the pixel shader
            using (var bytecode = ShaderBytecode.CompileFromFile("effect.fx", "ps_main", "ps_5_0", ShaderFlags.None, EffectFlags.None))
                pixelShader = new PixelShader(device, bytecode);

            // create the vertex layout and buffer
            var elements = new[] { new InputElement("POSITION", 0, Format.R32G32B32_Float, 0), new InputElement("textcoord", 0, Format.R32G32_Float, 12, 0) };
            layout = new InputLayout(device, inputSignature, elements);
            //var vertexBuffer = new SlimDX.Direct3D11.Buffer(device, vertices, 20 * 4, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

            // configure the Input Assembler portion of the pipeline with the vertex data
            context.InputAssembler.InputLayout = layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            //context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, 20, 0));

            // set the shaders
            context.VertexShader.Set(vertexShader);
            context.PixelShader.Set(pixelShader);

            // prevent DXGI handling of alt+enter, which doesn't work properly with Winforms
            using (var factory = swapChain.GetParent<Factory>())
                factory.SetWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAltEnter);

            // Handle Transparency using custom BlendState and RenderTarget
            var transParentOp = new BlendStateDescription
            {
                AlphaToCoverageEnable = false,
                IndependentBlendEnable = false
            };
            transParentOp.RenderTargets[0] = new RenderTargetBlendDescription();
            transParentOp.RenderTargets[0].BlendEnable = true;
            transParentOp.RenderTargets[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;

            transParentOp.RenderTargets[0].SourceBlend = BlendOption.SourceAlpha;
            transParentOp.RenderTargets[0].BlendOperation = BlendOperation.Add;
            transParentOp.RenderTargets[0].DestinationBlend = BlendOption.InverseSourceAlpha;

            transParentOp.RenderTargets[0].SourceBlendAlpha = BlendOption.One;
            transParentOp.RenderTargets[0].BlendOperationAlpha = BlendOperation.Add;
            transParentOp.RenderTargets[0].DestinationBlendAlpha = BlendOption.One;

            bs = BlendState.FromDescription(device, transParentOp);
            context.OutputMerger.BlendState = bs;

            //Dynamic VertexBuffer
            vertices = new DataStream(20 * 4, false, false);
            vertexBuffer = new SlimDX.Direct3D11.Buffer(device, vertices, 20 * 4, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, 20, 0));
            
        }

        public void initAllTexturesFromFiles()
        {

            try
            {
                txMenu = Texture2D.FromFile(device, "img/wallpaper.jpg");
                txMokou = Texture2D.FromFile(device, "img/mokou_00.png");
                txKaguya = Texture2D.FromFile(device, "img/kaguya_00.png");
                txHitboxGreen = Texture2D.FromFile(device, "img/hitbox_green.png");
                txHitboxRed = Texture2D.FromFile(device, "img/hitbox_red.png");

                resourceViewMenu = new ShaderResourceView(device, txMenu);
                resourceViewMokou = new ShaderResourceView(device, txMokou);
                resourceViewKaguya = new ShaderResourceView(device, txKaguya);
                resourceViewHitboxGreen = new ShaderResourceView(device, txHitboxGreen);
                resourceViewHitboxRed = new ShaderResourceView(device, txHitboxRed);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Cannot load texture images, aborting process: " + ex.ToString() + " " + ex.StackTrace);
                System.Console.ReadLine();
                Environment.Exit(-1);
            }

        }

        public void Dispose() {

            // 2D Cleaning
            //myPen.Dispose();
            //myRedPen.Dispose();
            //formGraphics.Dispose();

            // Direct3D11 Cleaning
            vertexBuffer.Dispose();
            vertices.Close();
            bs.Dispose();
            b.Dispose();
            effect.Dispose();
            layout.Dispose();
            inputSignature.Dispose();
            vertexShader.Dispose();
            pixelShader.Dispose();
            renderTarget.Dispose();
            swapChain.Dispose();
            device.Dispose();

            // Texture Cleaning
            resourceViewMenu.Dispose();
            resourceViewMokou.Dispose();
            resourceViewKaguya.Dispose();
            resourceViewHitboxGreen.Dispose();
            resourceViewHitboxRed.Dispose();

            txMenu.Dispose();
            txMokou.Dispose();
            txKaguya.Dispose();
            txHitboxGreen.Dispose();
            txHitboxRed.Dispose();
            

        }

        public void drawGameObject(GameObject go)
        {
            if (go == null) return;
            if (!go.isVisibleByCamera()) return;
            // load texture info
            loadTextureInfo(go);

            // update vertex data
            updateVertexBuffer(device, context, go);

            // draw to context
            context.Draw(4, 0);

            // clean memory buffer
            //vertexBuffer.Dispose();
            //vertices.Close();
            

        }

        private void updateVertexBuffer(SlimDX.Direct3D11.Device device, DeviceContext context, GameObject go)
        {
            DataBox mappedData = context.MapSubresource(vertexBuffer, 0, MapMode.WriteDiscard, SlimDX.Direct3D11.MapFlags.None);
            //vertices = go.setVertexDataStream();
            go.setVertexDataStream(mappedData);
            context.UnmapSubresource(vertexBuffer, 0);
            //vertexBuffer = new SlimDX.Direct3D11.Buffer(device, vertices, 20 * 4, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            //context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, 20, 0));
        }

        public void updateAspectRatioAll(List<GameObject> listGameObject)
        {
            if (listGameObject == null) return;
            foreach (GameObject go in listGameObject)
            {
                go.ratioY = go.txPointer.Description.Height / (float)windowH;
                go.ratioX = go.txPointer.Description.Width / (float)windowW;
                go.baseRatioY = 1f / (float)windowH;
                go.baseRatioX = 1f / (float)windowW;
                //go.coordY = 1.0f;
                //go.coordX = 1.0f;
                //go.coordY = go.coordY * go.ratioY;
                //go.coordX = go.coordX * go.ratioX;
                //go.originY = go.originY * go.ratioY;
                //go.originX = go.originX * go.ratioX;
            }

        }

        private void loadTextureInfo(GameObject go)
        {

            device.ImmediateContext.PixelShader.SetShaderResource(go.resourceView, 0);
            effect.GetVariableByName("xTexture").AsResource().SetResource(go.resourceView);
            context.PixelShader.SetShaderResource(go.resourceView, 0);

        }

        public void updateScreenSize() {

            windowH = form.ClientSize.Height;
            windowW = form.ClientSize.Width;
            updateAspectRatioAll(g.gl.listGameObject);

            renderTarget.Dispose();
            swapChain.ResizeBuffers(2, form.ClientSize.Width, form.ClientSize.Height, Format.R8G8B8A8_UNorm, SwapChainFlags.None);
            using (var resource = SlimDX.Direct3D11.Resource.FromSwapChain<Texture2D>(swapChain, 0)) renderTarget = new RenderTargetView(device, resource);
            context.OutputMerger.SetTargets(renderTarget);
            viewport = new Viewport(0.0f, 0.0f, form.ClientSize.Width, form.ClientSize.Height);
            context.Rasterizer.SetViewports(viewport);

            //formGraphics.Dispose();
            //form.Refresh();
            //formGraphics = form.CreateGraphics();
            //formGraphics.Flush();
            
        
        }

        public void swapFullScreen() {

            if (!swapChain.IsFullScreen)
            {
                var description = new SwapChainDescription()
                {
                    BufferCount = bufferCount,
                    Usage = Usage.RenderTargetOutput,
                    OutputHandle = form.Handle,
                    IsWindowed = true,
                    ModeDescription = new ModeDescription(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                    SampleDescription = new SampleDescription(1, 0),
                    Flags = SwapChainFlags.None,
                    SwapEffect = SwapEffect.Discard
                };
                swapChain.ResizeTarget(description.ModeDescription);
                swapChain.IsFullScreen = !swapChain.IsFullScreen;
                updateScreenSize();
            }
            else
            {
                var description = new SwapChainDescription()
                {
                    BufferCount = bufferCount,
                    Usage = Usage.RenderTargetOutput,
                    OutputHandle = form.Handle,
                    IsWindowed = true,
                    ModeDescription = new ModeDescription(form.ClientSize.Width, form.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                    SampleDescription = new SampleDescription(1, 0),
                    Flags = SwapChainFlags.None,
                    SwapEffect = SwapEffect.Discard
                };
                swapChain.ResizeTarget(description.ModeDescription);
                swapChain.IsFullScreen = !swapChain.IsFullScreen;
                windowW = 800;
                windowH = 600;
                form.Width = windowW;
                form.Height = windowH;
                form.Width = form.Width + (form.Width - form.ClientSize.Width);
                form.Height = form.Height + (form.Height - form.ClientSize.Height);
                windowW = form.ClientSize.Width;
                windowH = form.ClientSize.Height;
                updateScreenSize();
            }

        }


        public void setWireframeMode() {

            if (wireframe)
            {
                context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
                rsd = new RasterizerStateDescription()
                {
                    CullMode = CullMode.None,
                    DepthBias = 0,
                    DepthBiasClamp = 0.0f,
                    FillMode = FillMode.Solid,
                    IsAntialiasedLineEnabled = false,
                    IsDepthClipEnabled = false,
                    IsFrontCounterclockwise = false,
                    IsMultisampleEnabled = false,
                    IsScissorEnabled = false,
                    SlopeScaledDepthBias = 0.0f
                };

            }
            else
            {
                context.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineStrip;
                rsd = new RasterizerStateDescription()
                {
                    CullMode = CullMode.None,
                    DepthBias = 0,
                    DepthBiasClamp = 0.0f,
                    FillMode = FillMode.Wireframe,
                    IsAntialiasedLineEnabled = false,
                    IsDepthClipEnabled = false,
                    IsFrontCounterclockwise = false,
                    IsMultisampleEnabled = false,
                    IsScissorEnabled = false,
                    SlopeScaledDepthBias = 0.0f
                };
            }
            rs = RasterizerState.FromDescription(device, rsd);
            device.ImmediateContext.Rasterizer.State = rs;
            wireframe = !wireframe;
        }
    //---------------------------------------------END---OF---CLASS---------------------------------------
    }
}
