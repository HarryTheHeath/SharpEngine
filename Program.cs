using System;
using System.IO;
using System.Runtime.InteropServices;
using GLFW;
using OpenGL;
using static OpenGL.Gl;
using Monitor = GLFW.Monitor;

namespace SharpEngine
{
    class Program
    {
        private static Shape triangle = new Triangle(0.2f, 0.2f, new Vector(0, 0), 
            new Color[] {new Color(1, 0, 0, 1), new Color(0, 1, 0, 1), new Color(0, 0, 1, 1)});

        private static Shape triangle2 = new Triangle(0.2f, 0.2f, new Vector(-0.2f, -0.2f), 
            new Color[] {new Color(1, 0, 1, 1), new Color(1, 1, 0, 1), new Color(0, 1, 1, 1)});

        static void Main(string[] args)
        {
            var window = CreateWindow();
            CreateShaderProgram();
            var direction = new Vector(0.0003f, 0.0003f);
            var multiplier = 0.999f;
            
            // engine rendering loop
            while (!Glfw.WindowShouldClose(window))
            {
                Glfw.PollEvents(); // react to window changes (position etc.)
                ClearScreen();
                Render(window);
                
                triangle.Scale(multiplier);
                triangle2.Scale(multiplier);

                
                // 2. Keep track of the Scale, so we can reverse it
                if (triangle.CurrentScale <= 0.5f) {
                    multiplier = 1.001f;
                }
                if (triangle.CurrentScale >= 1f) {
                    multiplier = 0.999f;
                }

                // 3. Move the Triangle by its Direction
                triangle.Move(direction);
                triangle2.Move(direction);

                
                // 4. Check the X-Bounds of the Screen
                if (triangle.GetMaxBounds().x >= 1 && direction.x > 0 || triangle.GetMinBounds().x <= -1 && direction.x < 0) {
                    direction.x *= -1;
                }
                
                // 5. Check the Y-Bounds of the Screen
                if (triangle.GetMaxBounds().y >= 1 && direction.y > 0 || triangle.GetMinBounds().y <= -1 && direction.y < 0) {
                    direction.y *= -1;
                }
                
                triangle.Rotate();
                triangle2.Rotate();

            }
        }
        
        static void Render(Window window) {
            triangle.Render();
            triangle2.Render();
            Glfw.SwapBuffers(window);
        }

        static void ClearScreen() {
            glClearColor(.2f, .05f, .2f, 1);
            glClear(GL_COLOR_BUFFER_BIT);
        }

        static void CreateShaderProgram() {
            // create vertex shader
            var vertexShader = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(vertexShader, File.ReadAllText("shaders/position-color.vert"));
            glCompileShader(vertexShader);

            // create fragment shader
            var fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(fragmentShader, File.ReadAllText("shaders/vertex-color.frag"));
            glCompileShader(fragmentShader);

            // create shader program - rendering pipeline
            var program = glCreateProgram();
            glAttachShader(program, vertexShader);
            glAttachShader(program, fragmentShader);
            glLinkProgram(program);
            glUseProgram(program);
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
            Glfw.WindowHint(Hint.Doublebuffer, Constants.True);

            // create and launch a window
            var window = Glfw.CreateWindow(1024, 768, "SharpEngine", Monitor.None, Window.None);
            Glfw.MakeContextCurrent(window);
            Import(Glfw.GetProcAddress);
            return window;
        }
    }
}