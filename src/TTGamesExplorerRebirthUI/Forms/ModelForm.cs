using DarkUI.Forms;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.WinForms;
using SixLabors.ImageSharp.PixelFormats;
using System.Text;
using TTGamesExplorerRebirthLib.Formats;
using TTGamesExplorerRebirthLib.Formats.NuCore;
using Timer = System.Windows.Forms.Timer;

namespace TTGamesExplorerRebirthUI.Forms
{
    public partial class ModelForm : DarkForm
    {
        private readonly Vector3[] _vertexData;
        private readonly int[]     _indexData;
        private readonly Color4[]  _colorData;

        private int _cubeShader;
        private int _VAO;
        private int _EBO;
        private int _positionBuffer;
        private int _colorBuffer;

        private Timer _timer = null!;
        private float _angle = 0.0f;
        Matrix4 _projection;

        private readonly GLControl _glControl;
        private readonly Model     _gscFile;

        public ModelForm(string filePath, byte[] fileBuffer)
        {
            InitializeComponent();

            toolStripStatusLabel1.Text = Path.GetFileName(filePath);

            _glControl = new()
            {
                API           = OpenTK.Windowing.Common.ContextAPI.OpenGL,
                APIVersion    = new Version(3, 3, 0, 0),
                Flags         = OpenTK.Windowing.Common.ContextFlags.Debug,
                IsEventDriven = true,
                Location      = new Point(1, 1),
                Name          = "glControl",
                Profile       = OpenTK.Windowing.Common.ContextProfile.Core,
                Size          = new Size(200, 200),
                Dock          = DockStyle.Fill,
            };

            Controls.Add(_glControl);

            _gscFile = new(fileBuffer);

            _vertexData = new Vector3[_gscFile.Scene.NuMeshScene.NuRenderMesh.NuRenderMeshVbArray[0].NuVertexBuffer.Vertices.Count];
            _colorData = new Color4[_gscFile.Scene.NuMeshScene.NuRenderMesh.NuRenderMeshVbArray[0].NuVertexBuffer.Vertices.Count];

            for (int i = 0; i < _gscFile.Scene.NuMeshScene.NuRenderMesh.NuRenderMeshVbArray.Length; i++)
            {
                for (int j = 0; j < _gscFile.Scene.NuMeshScene.NuRenderMesh.NuRenderMeshVbArray[i].NuVertexBuffer.Vertices.Count; j++)
                {
                    for (int k = 0; k < _gscFile.Scene.NuMeshScene.NuRenderMesh.NuRenderMeshVbArray[i].NuVertexBuffer.Vertices[j].Count; k++)
                    {
                        if (_gscFile.Scene.NuMeshScene.NuRenderMesh.NuRenderMeshVbArray[i].NuVertexBuffer.Vertices[j][k].Definition == NuVertexDescAttributeDefinition.Position)
                        {
                            System.Numerics.Vector4 vertex = (System.Numerics.Vector4)_gscFile.Scene.NuMeshScene.NuRenderMesh.NuRenderMeshVbArray[i].NuVertexBuffer.Vertices[j][k].Value;

                            _vertexData[j] = new Vector3()
                            {
                                X = vertex.X,
                                Y = vertex.Y,
                                Z = vertex.Z,
                            };
                        }

                        if (_gscFile.Scene.NuMeshScene.NuRenderMesh.NuRenderMeshVbArray[i].NuVertexBuffer.Vertices[j][k].Definition == NuVertexDescAttributeDefinition.Color)
                        {
                            Rgba32 color = (Rgba32)_gscFile.Scene.NuMeshScene.NuRenderMesh.NuRenderMeshVbArray[i].NuVertexBuffer.Vertices[j][k].Value;

                            _colorData[j] = new Color4()
                            {
                                R = color.R / 255f,
                                G = color.G / 255f,
                                B = color.B / 255f,
                                A = color.A / 255f
                            };
                        }
                    }
                }
            }

            for (int i = 0; i < _gscFile.Scene.NuMeshScene.NuRenderMesh.NuIndexBufferArray.Length; i++)
            {
                _indexData = new int[_gscFile.Scene.NuMeshScene.NuRenderMesh.NuIndexBufferArray[i].Indices.Length];

                for (int j = 0; j < _gscFile.Scene.NuMeshScene.NuRenderMesh.NuIndexBufferArray[i].Indices.Length; j++)
                {
                    _indexData[j] = _gscFile.Scene.NuMeshScene.NuRenderMesh.NuIndexBufferArray[i].Indices[j];
                }
            }

            _glControl.Load += GlControl_Load;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            Helper.EnableDarkModeTitle(Handle);
        }

        private void GlControl_Load(object sender, EventArgs e)
        {
            _glControl.Resize += GlControl_Resize;
            _glControl.Paint += GlControl_Paint;

            // Redraw the screen every 1/20 of a second.
            _timer = new Timer();
            _timer.Tick += (sender, e) =>
            {
                const float DELTA_TIME = 1 / 50f;
                _angle += 180f * DELTA_TIME;
                Render();
            };

            // 1000 ms per sec / 33 ms per frame = 30 FPS
            _timer.Interval = 33;   
            _timer.Start();

            // Ensure that the viewport and projection matrix are set correctly initially.
            GlControl_Resize(_glControl, EventArgs.Empty);

            _cubeShader = CompileProgram(Encoding.UTF8.GetString(Properties.Resources.ModelVertex, 0, Properties.Resources.ModelVertex.Length),
                                         Encoding.UTF8.GetString(Properties.Resources.ModelFragment, 0, Properties.Resources.ModelFragment.Length));

            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);

            _EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indexData.Length * sizeof(int), _indexData, BufferUsageHint.StaticDraw);

            _positionBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _positionBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertexData.Length * sizeof(float) * 3, _vertexData, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);

            _colorBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _colorBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, _colorData.Length * sizeof(float) * 4, _colorData, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, sizeof(float) * 4, 0);
        }

        private static int CompileProgram(string vertexShader, string fragmentShader)
        {
            int program = GL.CreateProgram();

            int vert = CompileShader(ShaderType.VertexShader, vertexShader);
            int frag = CompileShader(ShaderType.FragmentShader, fragmentShader);

            GL.AttachShader(program, vert);
            GL.AttachShader(program, frag);

            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);

            if (success == 0)
            {
                throw new Exception($"Could not link program: {GL.GetProgramInfoLog(program)}");
            }

            GL.DetachShader(program, vert);
            GL.DetachShader(program, frag);

            GL.DeleteShader(vert);
            GL.DeleteShader(frag);

            return program;

            static int CompileShader(ShaderType type, string source)
            {
                int shader = GL.CreateShader(type);

                GL.ShaderSource(shader, source);
                GL.CompileShader(shader);

                GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);

                if (status == 0)
                {
                    throw new Exception($"Failed to compile {type}: {GL.GetShaderInfoLog(shader)}");
                }

                return shader;
            }
        }

        private void GlControl_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }

        private void GlControl_Resize(object sender, EventArgs e)
        {
            _glControl.MakeCurrent();

            if (_glControl.ClientSize.Height == 0)
            {
                _glControl.ClientSize = new Size(_glControl.ClientSize.Width, 1);
            }

            GL.Viewport(0, 0, _glControl.ClientSize.Width, _glControl.ClientSize.Height);

            float aspectRatio = Math.Max(_glControl.ClientSize.Width, 1) / (float)Math.Max(_glControl.ClientSize.Height, 1);

            _projection = Matrix4.CreatePerspectiveFieldOfView(0.05f, aspectRatio, 0.01f, 100f);
        }

        private void Render()
        {
            _glControl.MakeCurrent();

            GL.ClearColor(Color4.DarkGray);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.DepthTest);

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Enable(EnableCap.PolygonOffsetLine);
            GL.PolygonOffset(0.0f, 0.0f);

            Matrix4 lookat = Matrix4.LookAt(0, 5, 5, 0, 0, 0, 0, 1, 0);
            Matrix4 model  = Matrix4.CreateFromAxisAngle(new Vector3(0.0f, 1.0f, 0.0f), MathHelper.DegreesToRadians(_angle));
            Matrix4 mvp    = model * lookat * _projection;

            GL.UseProgram(_cubeShader);
            GL.UniformMatrix4(GL.GetUniformLocation(_cubeShader, "MVP"), true, ref mvp);

            GL.DrawElements(BeginMode.Triangles, _indexData.Length, DrawElementsType.UnsignedInt, 0);

            _glControl.SwapBuffers();
        }

        private void ModelViewerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _timer.Stop();
        }
    }
}