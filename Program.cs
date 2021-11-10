﻿using System;
using System.ComponentModel.Design;
using System.IO;
using System.Threading;
using GLFW;
using static OpenGL.Gl;
using Monitor = GLFW.Monitor;

namespace SharpEngine
{
    class Program
    {
        static float[] vertices = new float[] {
            // vertex 1 x, y, z
            -0.5f, -0.5f, 0f,
            // vertex 2 x, y, z
            0.5f, -0.5f, 0f,
            // vertex 3 x, y, z
            0f, 0.5f, 0f
        };

        const int vertexX = 0;
        private const int vertexY = 1;
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
                Render();
                UpdateTriangleBuffer();
                
                RightAngle();
                MoveDown();
                Shrink();
                if (vertices[7] <= 0)
                {
                    Grow();
                }
            }
        }

        static void RightAngle()
        {
            vertices[0] = vertices[4];
            vertices[3] = vertices[7];
            vertices[6] = vertices[1];
        }

        static void MoveRight()
        {
            for (var i = vertexX; i < vertices.Length; i += vertexSize)
            {
                vertices[i] += 0.0001f;
            }
            /*vertices[0] += 0.0001f;
            vertices[3] += 0.0001f;
            vertices[6] += 0.0001f;*/
        }

        static void MoveDown()
        {
            for (var i = vertexY; i < vertices.Length; i += vertexSize)
            {
                vertices[i] -= 0.0001f;
            }
            /*vertices [1] += -0.0001f;
            vertices [4] += -0.0001f;
            vertices [7] += -0.0001f;*/
        }

        static void Shrink()
        {
            if (vertices[7]>=0)
            {
                vertices[0] += 0.001f;
                vertices[1] += 0.001f;
                vertices[3] -= 0.001f;
                vertices[4] += 0.001f;
                vertices[7] -= 0.001f;
            }
        }
        
        static void Grow()
        {
            vertices[0] -= 0.001f;
            vertices[1] -= 0.001f;
            vertices[3] += 0.001f;
            vertices[4] -= 0.001f;
            vertices[7] += 0.001f;
        }

        static void Render()
        {
            glDrawArrays(GL_TRIANGLES, 0, vertices.Length/vertexSize);
            glFlush();
        }

        static void CreateShaderProgram() {
            // create vertex shader
            var vertexShader = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(vertexShader, File.ReadAllText("shaders/screen_coordinates.vert"));
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
            fixed (float* vertex = &vertices[0]) {
                glBufferData(GL_ARRAY_BUFFER, sizeof(float) * vertices.Length, vertex, GL_STATIC_DRAW);
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