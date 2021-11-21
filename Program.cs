using System;
using System.Drawing;
using System.Linq;

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

            Hilbert(graph, pen, x: 0f, y: 0f, size: 1024f, depth: 8, direction: 1, connectorDirection: 0, isInverse: false);

            image.Save("graph.png", System.Drawing.Imaging.ImageFormat.Png);

            Console.WriteLine("Hello World!");
        }

        static PointF Hilbert(Graphics graph, Pen pen, float x, float y, float size, int depth, int direction, int connectorDirection, bool isInverse)
        {
            if (direction > 4) direction -= 4;

            //if (depth == 1) graph.DrawRectangle(pen, x, y, size, size);
            //graph.DrawString(connectorDirection.ToString(), new Font(new FontFamily("Consolas"), 20, FontStyle.Regular),
              //  Brushes.Blue, new PointF(x+size/2-20, y+size/2-15));
            // graph.DrawString(isInverse.ToString(), new Font(new FontFamily("Consolas"), 20, FontStyle.Regular),
            //     Brushes.Green, new PointF(x+size/2, y+size/2-15));
            
            var points = new PointF[]
            {
                new PointF(x,        y+size/2),
                new PointF(x,        y),
                new PointF(x+size/2, y),
                new PointF(x+size/2, y+size/2)
            };

            if (direction > 1) points = RotatePoints(points, direction);
            PointF lastPoint = new PointF(0,0);

            if (depth == 0)
            {
                lastPoint = DrawElement(graph, pen, points, size, isInverse);
                DrawConnector(graph, pen, lastPoint, connectorDirection, size);
            }
            else
            {
                var endPoints = new PointF[4];
                endPoints[0] = Hilbert(graph, pen, points[0].X, points[0].Y, size / 2, depth - 1, direction + 1, connectorDirection: (isInverse?0:direction), isInverse: !isInverse);
                endPoints[1] = Hilbert(graph, pen, points[1].X, points[1].Y, size / 2, depth - 1, direction, connectorDirection:2 + direction - 1 + (isInverse?1:0), isInverse: isInverse);
                endPoints[2] = Hilbert(graph, pen, points[2].X, points[2].Y, size / 2, depth - 1, direction, connectorDirection:3 + direction - 1 + (isInverse?1:0), isInverse: isInverse);
                endPoints[3] = Hilbert(graph, pen, points[3].X, points[3].Y, size / 2, depth - 1, direction + 3, connectorDirection: (isInverse?direction:0)  , isInverse: !isInverse);
                lastPoint = endPoints[isInverse?0:3];
                
                //graph.DrawRectangle(pen, lastPoint.X, lastPoint.Y, 6, 6);
                DrawConnector(graph, pen, lastPoint, connectorDirection, size/((float)Math.Pow(2, depth)));
            }
            
            return lastPoint;
        }

        static PointF DrawElement(Graphics graph, Pen pen, PointF[] points, float size, bool isInverse)
        {
            points = points.Select(point => new PointF(point.X + size / 4, point.Y + size / 4)).ToArray();

            // Change places for points 0,3 and 1,2 accordingly
            if (isInverse)
            {
                var temp = points[0];
                points[0] = points[3];
                points[3] = temp;
                temp = points[1];
                points[1] = points[2];
                points[2] = temp;
            }

            // Draw lines between three points
            graph.DrawLines(pen, points);
            return points[3];
        }

        static void DrawConnector(Graphics graph, Pen pen, PointF startPoint, int direction, float size)
        {
            direction = (direction-1)%4 + 1;
            switch(direction)
            {
                case 1: graph.DrawLine(pen, startPoint, PointF.Add(startPoint, new SizeF(0, -size/2)));
                break;
                case 2: graph.DrawLine(pen, startPoint, PointF.Add(startPoint, new SizeF(size/2, 0)));
                break;
                case 3: graph.DrawLine(pen, startPoint, PointF.Add(startPoint, new SizeF(0, size/2)));
                break;
                case 4: graph.DrawLine(pen, startPoint, PointF.Add(startPoint, new SizeF(-size/2, 0)));
                break;
            }
        }

        static PointF[] RotatePoints(PointF[] points, int direction)
        {
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
