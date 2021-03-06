using System;
using System.Numerics;
using System.Runtime.InteropServices;
using OpenGL;
using static OpenGL.Gl;
using Monitor = GLFW.Monitor;


namespace SharpEngine
{
    public class Shape
    {

        protected Vertex[] vertices;
        public float CurrentScale { get; private set; }

        public Shape(Vertex[] vertices)
        {
            this.vertices = vertices;
            CurrentScale = 1f;
            LoadTriangleIntoBuffer();
        }

        protected Shape()
        {
            throw new NotImplementedException();
        }

        public Vector GetMinBounds()
        {
            var min = this.vertices[0].position;
            for (var i = 1; i < this.vertices.Length; i++)
            {
                min = Vector.Min(min, this.vertices[i].position);
            }
            return min;
        }

        public Vector GetMaxBounds()
        {
            var max = this.vertices[0].position;
            for (var i = 1; i < this.vertices.Length; i++)
            {
                max = Vector.Max(max, this.vertices[i].position);
            }
            return max;
        }

        public Vector GetCenter()
        {
            return (GetMinBounds() + GetMaxBounds()) / 2;
        }

        public void Scale(float multiplier)
        {
            // We first move the triangle to the center, to avoid
            // the triangle moving around while scaling.
            // Then, we move it back again.
            var center = GetCenter();
            Move(center * -1);
            for (var i = 0; i < this.vertices.Length; i++)
            {
                this.vertices[i].position *= multiplier;
            }
            Move(center);

            this.CurrentScale *= multiplier;
        }

        public void Move(Vector direction)
        {
            for (var i = 0; i < this.vertices.Length; i++)
            {
                this.vertices[i].position += direction;
            }
        }

        public void Rotate()
        {
            float rotation = 0.003f;
            var center = GetCenter();
            Move(center * -1);

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector position = vertices[i].position;
                var currentAngle = Math.Atan2(position.y, position.x);
                var currentMagnitude = MathF.Sqrt(MathF.Pow(position.x, 2) + MathF.Pow(position.y, 2));
                var newXPosition = MathF.Cos((float) currentAngle + rotation) * currentMagnitude;
                var newYPosition = MathF.Sin((float) currentAngle + rotation) * currentMagnitude;
                vertices[i].position = new Vector(newXPosition, newYPosition);
            }
            Move(center);
        }


        public unsafe void Render()
        {
            fixed (Vertex* vertex = &vertices[0])
            {
                Gl.glBufferData(Gl.GL_ARRAY_BUFFER, sizeof(Vertex) * this.vertices.Length, vertex, Gl.GL_DYNAMIC_DRAW);
            }

            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, this.vertices.Length);
        }
        
        private static unsafe void LoadTriangleIntoBuffer() {
            var vertexArray = glGenVertexArray();
            var vertexBuffer = glGenBuffer();
            glBindVertexArray(vertexArray);
            glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
            glVertexAttribPointer(0, 3, GL_FLOAT, false, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), nameof(Vertex.position)));
            glVertexAttribPointer(1, 4, GL_FLOAT, false, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), nameof(Vertex.color)));
            glEnableVertexAttribArray(0);
            glEnableVertexAttribArray(1);
        }
    }
}