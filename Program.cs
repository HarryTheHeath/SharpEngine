using System;
using System.ComponentModel.Design;
using System.IO;
using System.Threading;
using GLFW;
using static OpenGL.Gl;
using Monitor = GLFW.Monitor;

namespace SharpEngine
{
    struct Vector
    {
        public float x, y, z;

        public Vector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }
        
        
        public static Vector operator *(Vector v, float f)
        {
            return new Vector(v.x * f, v.y * f, v.z * f);
        }
        
        // +
        // -
        // /
    }
    class Program
    {
        static Vector[] vertices = new Vector[] {
            new Vector(-0.1f, -0.1f),
            new Vector(0.1f, -0.1f),
            new Vector(0f, 0.1f),
            new Vector(0.4f, 0.4f),
            new Vector(0.6f, 0.4f),
            new Vector(0.5f, 0.6f)
        };
        
        const int vertexSize = 3;
        
        static void Main(string[] args) {
            var window = CreateWindow();

            LoadTriangleIntoBuffer();

            CreateShaderProgram();

            // engine rendering loop
            while (!Glfw.WindowShouldClose(window)) {
                Glfw.PollEvents(); // react to window changes (position etc.)
                glClearColor(0.2f, 0.05f, 0.2f, 1);
                glClear(GL_COLOR_BUFFER_BIT);
                Render(window);
                UpdateTriangleBuffer();

                MoveRight();
            }
        }

        static void RightAngle()
        {
            for (var i = 0; i < vertices.Length; i++)
            {
                //vertices[i].x = vertices[Math.Abs(vertices[i].x -1)];
            }
            
        }
        
        static void MoveRight()
        {
            for (var i = 0; i < vertices.Length; i ++)
            {
                vertices[i].x += 0.0001f;
            }
        }

        static void MoveDown()
        {
            for (var i = 0; i < vertices.Length; i ++)
            {
                vertices[i].x -= 0.0001f;
            }
        }

        static void Shrink()
        {
            for (var i = 0; i < vertices.Length; i ++)
            {
                vertices[i].x *= 1.0001f;
            }
        }
        
        static void Grow()
        {
            for (var i = 0; i < vertices.Length; i ++)
            {
                vertices[i].x *= 1.0001f;
            }

            /*vertices[0] -= 0.001f;
            vertices[1] -= 0.001f;
            vertices[3] += 0.001f;
            vertices[4] -= 0.001f;
            vertices[7] += 0.001f;*/
        }

        static void Render(Window window)
        {
            glDrawArrays(GL_TRIANGLES, 0, vertices.Length);
            glFlush();
            Glfw.SwapBuffers(window);

        }

        static void CreateShaderProgram() {
            // create vertex shader
            var vertexShader = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(vertexShader, File.ReadAllText("shaders/screen-coordinates.vert"));
            glCompileShader(vertexShader);

            // create fragment shader
            var fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(fragmentShader, File.ReadAllText("shaders/green-triangle.frag"));
            glCompileShader(fragmentShader);

            // create shader program - rendering pipeline
            var program = glCreateProgram();
            glAttachShader(program, vertexShader);
            glAttachShader(program, fragmentShader);
            glLinkProgram(program);
            glUseProgram(program);
        }

        static unsafe void LoadTriangleIntoBuffer() {

            // load the vertices into a buffer
            var vertexArray = glGenVertexArray();
            var vertexBuffer = glGenBuffer();
            glBindVertexArray(vertexArray);
            glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
            UpdateTriangleBuffer();
            glVertexAttribPointer(0, vertexSize, GL_FLOAT, false, vertexSize * sizeof(float), NULL);

            glEnableVertexAttribArray(0);
        }
        
        static unsafe void UpdateTriangleBuffer() {
            fixed (Vector* vertex = &vertices[0]) {
                glBufferData(GL_ARRAY_BUFFER, sizeof(Vector) * vertices.Length, vertex, GL_STATIC_DRAW);
            }
        }

        static Window CreateWindow() {
            // initialize and configure
            Glfw.Init();
            Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.Decorated, true);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
            Glfw.WindowHint(Hint.OpenglForwardCompatible, Constants.True);
            Glfw.WindowHint(Hint.Doublebuffer, Constants.False);

            // create and launch a window
            var window = Glfw.CreateWindow(1024, 768, "SharpEngine", Monitor.None, Window.None);
            Glfw.MakeContextCurrent(window);
            Import(Glfw.GetProcAddress);
            return window;
        }
    }
}