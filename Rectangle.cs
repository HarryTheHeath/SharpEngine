namespace SharpEngine
{
    public class Rectangle : Shape
    {
        public Rectangle(float width, float height, Vector position, Color[] color) : base(new Vertex[6])
        {
            vertices[0] = new Vertex(new Vector(position.x - width / 2, position.y - height / 2), color[0]);
            vertices[1] = new Vertex(new Vector(position.x + width / 2, position.y - height / 2), color[1]);
            vertices[2] = new Vertex(new Vector(position.x - width / 2, position.y + height / 2), color[2]);
            vertices[3] = new Vertex(new Vector(position.x + width / 2, position.y + height/ 2), color[3]);
            vertices[4] = new Vertex(new Vector(position.x - width / 2, position.y + height/ 2), color[4]);
            vertices[5] = new Vertex(new Vector(position.x + width / 2, position.y - height/ 2), color[5]);
        }
    }
}