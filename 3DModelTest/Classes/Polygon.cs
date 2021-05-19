namespace PlayerModelTest
{
    public class Polygon
    {
        public Vector Normal = new Vector();

        public ushort[] Verticies = new ushort[4];
        public ushort Vertex0 => Verticies[0];
        public ushort Vertex1 => Verticies[1];
        public ushort Vertex2 => Verticies[2];
        public ushort Vertex3 => Verticies[3];


        public ATTR Attribute;
    }
}
