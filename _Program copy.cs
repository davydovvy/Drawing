using System;
using System.Drawing;
using System.Linq;

namespace Drawing
{
    class Program1
    {
        static void Main1(string[] args)
        {
            Image image = new Bitmap(1024, 1024);
            Graphics graph = Graphics.FromImage(image);
            graph.Clear(Color.Azure);
            Pen pen = new Pen(Brushes.Black);

            Hilbert(graph, pen, x: 0f, y: 0f, size: 1024f, direction: 1, depth: 5, quadrant: 0, connectionPoint: new PointF(0, 0), absQuadrant:0);

            image.Save("graph.png", System.Drawing.Imaging.ImageFormat.Png);

            Console.WriteLine("Hello World!");
        }

        static PointF Hilbert(Graphics graph, Pen pen, float x, float y, float size, int direction, int depth, int quadrant, PointF connectionPoint, int absQuadrant)
        {
            if (direction > 4) direction -= 4;

            if (depth == 2) graph.DrawRectangle(pen, x, y, size, size);
            //int absoluteQuadrant = direction;
           // if (absoluteQuadrant<0) absoluteQuadrant += 4;
            graph.DrawString(absQuadrant.ToString(), new Font(new FontFamily("Consolas"), 20, FontStyle.Regular),
                Brushes.Blue, new PointF(x+size/2-20, y+size/2-15));
            
            var points = new PointF[]
            {
                new PointF(x,        y+size/2),
                new PointF(x,        y),
                new PointF(x+size/2, y),
                new PointF(x+size/2, y+size/2)
            };

            points = RotatePoints(points, direction);

            PointF[] innerPoints;
            PointF lastPoint = new PointF(0, 0);

            if (depth == 1)
            {
                innerPoints = DrawElement(graph, pen, points[0].X, points[0].Y, size / 2, direction + 1, quadrant: 1);
                // if (direction == 2 || direction == 4)
                //     lastPoint = new PointF(innerPoints[3].X, innerPoints[3].Y);
                innerPoints = DrawElement(graph, pen, points[1].X, points[1].Y, size / 2, direction, quadrant: 2);
                innerPoints = DrawElement(graph, pen, points[2].X, points[2].Y, size / 2, direction, quadrant: 3);
                innerPoints = DrawElement(graph, pen, points[3].X, points[3].Y, size / 2, direction + 3, quadrant: 4);
                //if (direction == 1 || direction == 3)
                    lastPoint = new PointF(innerPoints[3].X, innerPoints[3].Y);
                //else
                //      graph.DrawLine(pen, connectionPoint, innerPoints[0]);
            }
            else
            {
                var endPoints = new PointF[4];
                endPoints[0] = Hilbert(graph, pen, points[0].X, points[0].Y, size / 2, direction + 1, depth - 1, quadrant: 1, connectionPoint: lastPoint, absQuadrant: 1);
                endPoints[1] = Hilbert(graph, pen, points[1].X, points[1].Y, size / 2, direction, depth - 1, quadrant: 2, connectionPoint: lastPoint, absQuadrant: 2);
                endPoints[2] = Hilbert(graph, pen, points[2].X, points[2].Y, size / 2, direction, depth - 1, quadrant: 3, connectionPoint: lastPoint, absQuadrant: 3);
                endPoints[3] = Hilbert(graph, pen, points[3].X, points[3].Y, size / 2, direction + 3, depth - 1, quadrant: 4, connectionPoint: lastPoint, absQuadrant: 4);
                if (direction == 1 || direction == 3)
                {
                    lastPoint = endPoints[3];
                    //graph.DrawRectangle(pen, endPoints[3].X, endPoints[3].Y, 6, 6);
                }
                else if (direction == 2 || direction == 4)
                    lastPoint = endPoints[0];

                graph.DrawRectangle(pen, lastPoint.X, lastPoint.Y, 6, 6);
            }

            return lastPoint;
        }

        static PointF[] DrawElement(Graphics graph, Pen pen, float x, float y, float size, int direction, int quadrant)
        {
            if (direction > 4) direction -= 4;

            var points = new PointF[]
            {
                new PointF(x,        y+size/2),
                new PointF(x,        y),
                new PointF(x+size/2, y),
                new PointF(x+size/2, y+size/2)
            };

            points = RotatePoints(points, direction);
            //points = points.Select(point => new PointF(point.X + size / 4, point.Y + size / 4)).ToArray();

            graph.DrawLines(pen, points);

            // // Draw connector to the next quadrant
            // switch (quadrant)
            // {
            //     case 1:
            //         graph.DrawLine(pen, points[0].X, points[0].Y, 2 * points[0].X - points[3].X, 2 * points[0].Y - points[3].Y);
            //         break;

            //     case 2:
            //         graph.DrawLine(pen, points[3].X, points[3].Y, 2 * points[3].X - points[0].X, 2 * points[3].Y - points[0].Y);
            //         break;

            //     case 3:
            //         graph.DrawLine(pen, points[3].X, points[3].Y, 2 * points[3].X - points[2].X, 2 * points[3].Y - points[2].Y);
            //         break;
            // }

            return points;
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
