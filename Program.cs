﻿using System;
using System.ComponentModel.Design;
using System.IO;
using System.Numerics;
using System.Threading;
using GLFW;
using static OpenGL.Gl;
using Monitor = GLFW.Monitor;

namespace SharpEngine
{
    class Program
    {
        public struct Vertex {
            public Vector position;

            public Vertex(Vector position) {
                this.position = position;
            }
        }
        
        static Vertex[] vertices = new Vertex[] {
            new Vertex(new Vector(0f, 0f)),
            new Vertex(new Vector(1f, 0f)),
            new Vertex(new Vector(0f, 1f))
        };
        
        
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
                
                // 1. Scale the Triangle without Moving it
                
                // 1.1 Move the Triangle to the Center, so we can scale it without Side Effects
                // 1.1.1 Find the Center of the Triangle
                // 1.1.1.1 Find the Minimum and Maximum
                var min = vertices[0].position;
                for (var i = 1; i < vertices.Length; i++) {
                    min = Vector.Min(min, vertices[i].position);
                }
                var max = vertices[0].position;
                for (var i = 1; i < vertices.Length; i++) {
                    max = Vector.Max(max, vertices[i].position);
                }
                // 1.1.1.2 Average out the Minimum and Maximum to get the Center
                var center = (min + max) / 2;
                // 1.1.2 Move the Triangle the Center
                for (var i = 0; i < vertices.Length; i++) {
                    vertices[i].position -= center;
                }
                // 1.2 Scale the Triangle
                for (var i = 0; i < vertices.Length; i++) {
                    vertices[i].position *= multiplier;
                }
                // 1.3 Move the Triangle Back to where it was before
                for (var i = 0; i < vertices.Length; i++) {
                    vertices[i].position += center;
                }
                
                // 2. Keep track of the Scale, so we can reverse it
                scale *= multiplier;
                if (scale <= 0.5f) {
                    multiplier = 1.001f;
                }
                if (scale >= 1f) {
                    multiplier = 0.999f;
                }

                // 3. Move the Triangle by its Direction
                for (var i = 0; i < vertices.Length; i++) {
                    vertices[i].position += direction;
                }
                // 4. Check the X-Bounds of the Screen
                for (var i = 0; i < vertices.Length; i++) {
                    if (vertices[i].position.x >= 1 && direction.x > 0 || vertices[i].position.x <= -1 && direction.x < 0) {
                        direction.x *= -1;
                        break;
                    }
                }
                // 5. Check the Y-Bounds of the Screen
                for (var i = 0; i < vertices.Length; i++) {
                    if (vertices[i].position.y >= 1 && direction.y > 0 || vertices[i].position.y <= -1 && direction.y < 0) {
                        direction.y *= -1;
                        break;
                    }
                }
                UpdateTriangleBuffer();
            }
        }
        static void BackAndForth()
        
        {
           
        }
       
        static void Rotate()
        {
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i].position.x = (float)(vertices[i].position.x * Math.Cos(0.01f) + vertices[i].position.y * Math.Sin(0.01f));
                vertices[i].position.y = (float)(vertices[i].position.y * Math.Cos(0.01f) - vertices[i].position.x * Math.Sin(0.01f));  
            }
        }

        private static void ClearScreen() {
            glClearColor(0.2f, 0f, 0.2f, 1);
            glClear(GL_COLOR_BUFFER_BIT);
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
            fixed (Vertex* vertex = &vertices[0]) {
                glBufferData(GL_ARRAY_BUFFER, sizeof(Vertex) * vertices.Length, vertex, GL_STATIC_DRAW);
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