using System.Collections.Generic;

namespace SmokeTest
{
    public abstract class BoxBase : BaseEntity
    {
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
        public IList<BoxBase> Boxes { get; set; }

        public BoxBase()
        {
            A = 5;
            B = 7;
            C = 9;
            Boxes = new List<BoxBase>();
        }
    }

    public class BoxInterne : BoxBase
    {
        public int V => A * B * C;
    }
}