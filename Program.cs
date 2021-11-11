using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using GLFW;
using static OpenGL.Gl;
using Monitor = GLFW.Monitor;

namespace SharpEngine
{
    public class Triangle
    {
        Vertex[] vertices;
        public float CurrentScale {get; private set;}


        public Triangle(Vertex[] vertices)
        {
            this.vertices = vertices;
            CurrentScale = 1f;
        }

        public void Scale(float multiplier)
        {
            // 1. Scale the Triangle without Moving it

            // 1.1 Move the Triangle to the Center, so we can scale it without Side Effects
            // 1.1.1 Find the Center of the Triangle
            // 1.1.1.1 Find the Minimum and Maximum
            var min = vertices[0].position;
            for (var i = 1; i < vertices.Length; i++)
            {
                min = Vector.Min(min, vertices[i].position);
            }

            var max = vertices[0].position;
            for (var i = 1; i < vertices.Length; i++)
            {
                max = Vector.Max(max, vertices[i].position);
            }

            // 1.1.1.2 Average out the Minimum and Maximum to get the Center
            var center = (GetMinBounds() + GetMaxBounds()) / 2;
            Move(center*-1);
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i].position -= center;
            }

            Move(center);

            // 1.2 Scale the Triangle
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i].position *= multiplier;
            }

            // 1.3 Move the Triangle Back to where it was before
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i].position += center;
            }

            CurrentScale *= multiplier;
        }

        public void Move(Vector direction)
        {
            for (var i = 1; i < vertices.Length; i++)
            {
                vertices[i].position += direction;
            }
        }

        public Vector GetMinBounds()
        {
            var min = vertices[0].position;
            for (var i = 1; i < vertices.Length; i++)
            {
                min = Vector.Min(min, vertices[i].position);
            }

            return min;
        }

        public Vector GetMaxBounds()
        {
            var max = vertices[0].position;
            for (var i = 1; i < vertices.Length; i++)
            {
                max = Vector.Max(max, vertices[i].position);
            }

            return max;
        }
        
        public unsafe void Render() {
            fixed (Vertex* vertex = &vertices[0]) {
                glBufferData(GL_ARRAY_BUFFER, sizeof(Vertex) * vertices.Length, vertex, GL_DYNAMIC_DRAW);
            }
            glDrawArrays(GL_TRIANGLES, 0, vertices.Length);
        }
    }
    class Program
    {
        static Triangle triangle = new Triangle(new Vertex[] {
            new Vertex(new Vector(0f, 0f), Color.Red),
            new Vertex(new Vector(1f, 0f), Color.Green),
            new Vertex(new Vector(0f, 1f), Color.Blue),
            }
        );
        
        /*static Vector[] vertices = new Vector[] {
            new Vector(-0.1f, -0.1f),
            new Vector(0.1f, -0.1f),
            new Vector(0f, 0.1f),
            new Vector(0.1f, 0.1f),
            new Vector(0.3f, 0.1f),
            new Vector(0.2f, 0.3f),
            new Vector(-0.3f, -0.3f),
            new Vector(-0.1f, -0.3f),
            new Vector(-0.2f, -0.1f)
        };*/
        
        const int vertexSize = 3;
        static bool test;
        
        static void Main(string[] args)
        {
            var window = CreateWindow();
            var direction = new Vector(0.0003f, 0.0003f);
            var multiplier = 0.999f;
            var scale = 1f;

            LoadTriangleIntoBuffer();
            CreateShaderProgram();

            // engine rendering loop
            while (!Glfw.WindowShouldClose(window))
            {
                Glfw.PollEvents(); // react to window changes (position etc.)
                ClearScreen();
                Render(window);
                
                triangle.Scale(multiplier);
                
                
                // 2. Keep track of the Scale, so we can reverse it
                if (triangle.CurrentScale <= 0.5f) {
                    multiplier = 1.001f;
                }
                if (triangle.CurrentScale >= 1f) {
                    multiplier = 0.999f;
                }

                // 3. Move the Triangle by its Direction
                triangle.Move(direction);
                
                // 4. Check the X-Bounds of the Screen
                if (triangle.GetMaxBounds().x >= 1 && direction.x > 0 || triangle.GetMinBounds().x <= -1 && direction.x < 0) {
                    direction.x *= -1;
                }
                
                // 5. Check the Y-Bounds of the Screen
                if (triangle.GetMaxBounds().y >= 1 && direction.y > 0 || triangle.GetMinBounds().y <= -1 && direction.y < 0) {
                    direction.y *= -1;
                }
            }
        }

        /*static void Rotate()
        {
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i].position.x = (float)(vertices[i].position.x * Math.Cos(0.01f) + vertices[i].position.y * Math.Sin(0.01f));
                vertices[i].position.y = (float)(vertices[i].position.y * Math.Cos(0.01f) - vertices[i].position.x * Math.Sin(0.01f));  
            }
        }*/
        
        

        private static void ClearScreen() {
            glClearColor(0.2f, 0f, 0.2f, 1);
            glClear(GL_COLOR_BUFFER_BIT);
        }

        static void CreateShaderProgram() {
            // create vertex shader
            var vertexShader = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(vertexShader, File.ReadAllText("Shaders/position-color.vert"));
            glCompileShader(vertexShader);

            // create fragment shader
            var fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(fragmentShader, File.ReadAllText("Shaders/vertex-color.frag"));
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
            glVertexAttribPointer(0, 3, GL_FLOAT, false, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), nameof(Vertex.position)));
            glVertexAttribPointer(1, 4, GL_FLOAT, false, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), nameof(Vertex.color)));
            glEnableVertexAttribArray(0);
            glEnableVertexAttribArray(1);

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