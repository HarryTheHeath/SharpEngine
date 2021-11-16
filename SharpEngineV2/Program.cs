﻿using System;
using System.Collections.Generic;
using GLFW;

namespace SharpEngine
{
    class Program {
        static float Lerp(float from, float to, float t) {
            return from + (to - from) * t;
        }

        static float GetRandomFloat(Random random, float min = 0, float max = 1) {
            return Lerp(min, max, (float)random.Next() / int.MaxValue);
        }
        
        static void FillSceneWithTriangles(Scene scene, Material material) {
            var random = new Random();
            for (var i = 0; i < 10; i++) {
                var triangle = new Triangle(new Vertex[] {
                    new Vertex(new Vector(-.1f, 0f), Color.Red),
                    new Vertex(new Vector(.1f, 0f), Color.Green),
                    new Vertex(new Vector(0f, .133f), Color.Blue)
                }, material);
                triangle.Transform.Rotate(GetRandomFloat(random));
                triangle.Transform.Move(new Vector(GetRandomFloat(random, -1, 1), GetRandomFloat(random, -1, 1)));
                scene.Add(triangle);
            }
        }
        
        static void Main(string[] args) {
            
            var window = new Window();
            var material = new Material("shaders/world-position-color.vert", "shaders/vertex-color.frag");
            var scene = new Scene();
            window.Load(scene);

            //FillSceneWithTriangles(scene, material);
            var newTriangle = new Triangle(new Vertex[] {
                new Vertex(new Vector(-.1f, 0f), Color.Red),
                new Vertex(new Vector(.1f, 0f), Color.Green),
                new Vertex(new Vector(0f, .133f), Color.Blue)
            }, material);
            scene.Add(newTriangle);
            
            // engine rendering loop
            var direction = new Vector(0.009f, 0.009f);
            var multiplier = 0.999f;
            var rotation = 0.005f;
            const int fixedStepNumbersPerSecond = 30;
            while (window.IsOpen()) {
                // check if another step should execute now
                Console.WriteLine(Glfw.Time);
                
                window.Render();
                
                // Update Triangles
                for (var i = 0; i < scene.triangles.Count; i++) {
                    var triangle = scene.triangles[i];
                
                    // 2. Keep track of the Scale, so we can reverse it
                    if (triangle.Transform.CurrentScale.GetMagnitude() <= 0.5f) {
                        multiplier = 1.001f;
                    }
                    if (triangle.Transform.CurrentScale.GetMagnitude() >= 2f) {
                        multiplier = 0.999f;
                    }
                    
                    triangle.Transform.Scale(multiplier);
                    triangle.Transform.Rotate(rotation);
                
                    // 4. Check the X-Bounds of the Screen
                    if (triangle.GetMaxBounds().x >= 1 && direction.x > 0 || triangle.GetMinBounds().x <= -1 && direction.x < 0) {
                        direction.x *= -1;
                    }
                
                    // 5. Check the Y-Bounds of the Screen
                    if (triangle.GetMaxBounds().y >= 1 && direction.y > 0 || triangle.GetMinBounds().y <= -1 && direction.y < 0) {
                        direction.y *= -1;
                    }
                    
                    triangle.Transform.Move(direction);
                }
                window.Render();
            }
        }
    }
}