using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageClusteringLibrary.Data
{
    public readonly struct ImagePixel
    {
        public readonly Vector2<int> Position;
        public readonly RgbVector ColorRgb;

        public ImagePixel(int x, int y, Color color)
        {
            this.Position = new Vector2<int>(x, y);
            this.ColorRgb = new RgbVector(color.R, color.G, color.B);
        }
    }
}
