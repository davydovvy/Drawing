using System;
using System.Drawing;

namespace Drawing
{
    class Program
    {
        static void Main(string[] args)
        {
            Image image = new Bitmap(1024, 1024);
            Graphics graph = Graphics.FromImage(image);
            graph.Clear(Color.Azure);
            Pen pen = new Pen(Brushes.Black);

            DrawHilbert(graph, pen, x: 0f, y: 0f, size: 1024f, direction: 2, depth: 2);

            image.Save("graph.png", System.Drawing.Imaging.ImageFormat.Png);

            Console.WriteLine("Hello World!");
        }

        static void DrawHilbert(Graphics graph, Pen pen, float x, float y, float size, int direction, int depth)
        {
            if (direction > 4) direction -= 4;
            var points = new PointF[]
            {
                new PointF(x+size*1/4, y+size*3/4),
                new PointF(x+size*1/4, y+size*1/4),
                new PointF(x+size*3/4, y+size*1/4),
                new PointF(x+size*3/4, y+size*3/4)
            };

            points = RotatePoints(points, direction);

            if (depth == 0)
                graph.DrawLines(pen, points);
            else
            {
                DrawHilbert(graph, pen, x, y + size / 2, size / 2, direction + 1, depth - 1);
                DrawHilbert(graph, pen, x, y, size / 2, direction, depth - 1);
                DrawHilbert(graph, pen, x + size / 2, y, size / 2, direction, depth - 1);
                DrawHilbert(graph, pen, x + size / 2, y + size / 2, size / 2, direction + 3, depth - 1);
            }
        }

        static PointF[] RotatePoints(PointF[] points, int direction)
        {
            if (direction == 1) return points;

            var result = new PointF[4];

            // Shift array number of times equal to direction-1
            for (int i = 0; i < 4; i++)
                if (i + direction - 1 < 4)
                    result[i] = points[i + direction - 1];
                else
                    result[i] = points[i + direction - 5];

            return result;
        }
    }
}
