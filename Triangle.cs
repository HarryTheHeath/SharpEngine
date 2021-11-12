namespace SharpEngine
{
    public class Triangle : Shape
    {
        public Triangle(float width, float height, Vector position, Color[] color) : base(new Vertex[3])
        {
            vertices[0] = new Vertex(new Vector(position.x - width / 2, position.y - height / 2), color[0]);
            vertices[1] = new Vertex(new Vector(position.x + width / 2, position.y - height / 2), color[1]);
            vertices[2] = new Vertex(new Vector(position.x, position.y + height / 2), color[2]);
        }
    }
}