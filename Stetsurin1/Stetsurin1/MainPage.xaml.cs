using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Collections.Generic;

namespace Stetsurin1
{
    public partial class MainPage : ContentPage
    {
        private List<IShapeDrawable> drawings = new List<IShapeDrawable>();
        private List<IShapeDrawable> undoStack = new List<IShapeDrawable>();
        private List<IShapeDrawable> redoStack = new List<IShapeDrawable>();
        private IShapeDrawable currentDrawable = null;
        private string currentTool = null;
        private bool isErasing = false;
        private Point touchStart;

        public MainPage()
        {
            InitializeComponent();
            PaintCanvas.Drawable = new CanvasDrawable(drawings);
            BindingContext = this;

            PaintCanvas.StartInteraction += PaintCanvas_StartInteraction;
            PaintCanvas.DragInteraction += PaintCanvas_DragInteraction;
            PaintCanvas.EndInteraction += PaintCanvas_EndInteraction;

            RectangleButton.Clicked += RectangleButton_Clicked;
            CircleButton.Clicked += CircleButton_Clicked;
            TriangleButton.Clicked += TriangleButton_Clicked;
            EraseButton.Clicked += EraseButton_Clicked;
            UndoButton.Clicked += UndoButton_Clicked;
            RedoButton.Clicked += RedoButton_Clicked;
            SaveButton.Clicked += SaveButton_Clicked;
            LoadButton.Clicked += LoadButton_Clicked;
        }

        private void PaintCanvas_StartInteraction(object sender, TouchEventArgs args)
        {
            if (args.Touches.Length == 0) return;
            touchStart = args.Touches[0];

            if (isErasing)
            {
                EraseObject(touchStart);
            }
            else if (currentDrawable != null)
            {
                currentDrawable.StartDrawing(touchStart);
                PaintCanvas.Invalidate();
            }
        }

        private void PaintCanvas_DragInteraction(object sender, TouchEventArgs args)
        {
            if (args.Touches.Length == 0) return;
            if (currentDrawable != null && !isErasing)
            {
                Point currentPoint = args.Touches[0];
                currentDrawable.UpdateDrawing(currentPoint);
                PaintCanvas.Invalidate();
            }
        }

        private void PaintCanvas_EndInteraction(object sender, TouchEventArgs args)
        {
            if (currentDrawable != null && !isErasing)
            {
                drawings.Add(currentDrawable);
                undoStack.Add(currentDrawable);
                redoStack.Clear();
                
                if (currentTool == "Rectangle")
                {
                    currentDrawable = new RectangleDrawable();
                }
                else if (currentTool == "Circle")
                {
                    currentDrawable = new CircleDrawable();
                }
                else if (currentTool == "Triangle")
                {
                    currentDrawable = new TriangleDrawable();
                }
                PaintCanvas.Invalidate();
            }
        }

        private void RectangleButton_Clicked(object sender, EventArgs e)
        {
            isErasing = false;
            currentTool = "Rectangle";
            currentDrawable = new RectangleDrawable();
        }

        private void CircleButton_Clicked(object sender, EventArgs e)
        {
            isErasing = false;
            currentTool = "Circle";
            currentDrawable = new CircleDrawable();
        }

        private void TriangleButton_Clicked(object sender, EventArgs e)
        {
            isErasing = false;
            currentTool = "Triangle";
            currentDrawable = new TriangleDrawable();
        }

        private void EraseButton_Clicked(object sender, EventArgs e)
        {
            isErasing = true;
            currentTool = null;
            currentDrawable = null;
        }

        private void UndoButton_Clicked(object sender, EventArgs e)
        {
            if (drawings.Count > 0)
            {
                var lastDrawing = drawings[drawings.Count - 1];
                drawings.RemoveAt(drawings.Count - 1);
                undoStack.Add(lastDrawing);
                redoStack.Clear();
                PaintCanvas.Invalidate();
            }
        }

        private void RedoButton_Clicked(object sender, EventArgs e)
        {
            if (undoStack.Count > 0)
            {
                var lastUndone = undoStack[undoStack.Count - 1];
                drawings.Add(lastUndone);
                undoStack.RemoveAt(undoStack.Count - 1);
                redoStack.Clear();
                PaintCanvas.Invalidate();
            }
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            // Потом
        }

        private void LoadButton_Clicked(object sender, EventArgs e)
        {
            // Потом
        }

        private void EraseObject(Point point) // Потом
        {
            for (int i = drawings.Count - 1; i >= 0; i--)
            {
                if (IsPointNearDrawable(drawings[i], point))
                {
                    undoStack.Add(drawings[i]);
                    drawings.RemoveAt(i);
                    PaintCanvas.Invalidate();
                    break;
                }
            }
        }

        private bool IsPointNearDrawable(IShapeDrawable drawable, Point point)
        {
            // Потом
            return false;
        }
    }

    public class CanvasDrawable : Microsoft.Maui.Graphics.IDrawable
    {
        private readonly List<IShapeDrawable> drawings;

        public CanvasDrawable(List<IShapeDrawable> drawings)
        {
            this.drawings = drawings;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            foreach (var drawable in drawings)
            {
                drawable.Draw(canvas, dirtyRect);
            }
        }
    }

    public interface IShapeDrawable
    {
        void Draw(ICanvas canvas, RectF dirtyRect);
        void StartDrawing(Point point);
        void UpdateDrawing(Point point);
    }

    public class RectangleDrawable : IShapeDrawable
    {
        private Point startPoint;
        private Point endPoint;
        private Color color = Colors.Black;
        private float strokeWidth = 2;

        public void StartDrawing(Point point) => startPoint = point;
        public void UpdateDrawing(Point point) => endPoint = point;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.StrokeColor = color;
            canvas.StrokeSize = strokeWidth;
            canvas.DrawRectangle((float)Math.Min(startPoint.X, endPoint.X),
                                 (float)Math.Min(startPoint.Y, endPoint.Y),
                                 (float)Math.Abs(endPoint.X - startPoint.X),
                                 (float)Math.Abs(endPoint.Y - startPoint.Y));
        }
    }

    public class CircleDrawable : IShapeDrawable
    {
        private Point startPoint;
        private Point endPoint;
        private Color color = Colors.Black;
        private float strokeWidth = 2;

        public void StartDrawing(Point point) => startPoint = point;
        public void UpdateDrawing(Point point) => endPoint = point;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.StrokeColor = color;
            canvas.StrokeSize = strokeWidth;
            float radius = (float)Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) +
                                            Math.Pow(endPoint.Y - startPoint.Y, 2)) / 2;
            canvas.DrawCircle((float)(startPoint.X + (endPoint.X - startPoint.X) / 2),
                             (float)(startPoint.Y + (endPoint.Y - startPoint.Y) / 2),
                             radius);
        }
    }

    public class TriangleDrawable : IShapeDrawable
    {
        private Point startPoint;
        private Point endPoint;
        private Color color = Colors.Black;
        private float strokeWidth = 2;

        public void StartDrawing(Point point) => startPoint = point;
        public void UpdateDrawing(Point point) => endPoint = point;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.StrokeColor = color;
            canvas.StrokeSize = strokeWidth;

            double dx = endPoint.X - startPoint.X;
            double dy = endPoint.Y - startPoint.Y;
            Point p1, p2, p3;

            if (dy > 0 && dx > 0)
            {
                p1 = new Point(startPoint.X, endPoint.Y);
                p2 = new Point(startPoint.X + (dx / 2), startPoint.Y);
                p3 = new Point(endPoint.X, endPoint.Y);
            }
            else if (dy > 0 && dx < 0)
            {
                p1 = new Point(endPoint.X, endPoint.Y);
                p2 = new Point(endPoint.X + (-dx / 2), startPoint.Y);
                p3 = new Point(startPoint.X, endPoint.Y);
            }
            else if (dy < 0 && dx > 0)
            {
                p1 = new Point(startPoint.X, endPoint.Y);
                p2 = new Point(startPoint.X + (dx / 2), startPoint.Y);
                p3 = new Point(endPoint.X, endPoint.Y);
            }
            else if (dy < 0 && dx < 0)
            {
                p1 = new Point(endPoint.X, endPoint.Y);
                p2 = new Point(endPoint.X + (-dx / 2), startPoint.Y);
                p3 = new Point(startPoint.X, endPoint.Y);
            }
            else
            {
                return;
            }
            canvas.DrawLine((float)p1.X, (float)p1.Y, (float)p2.X, (float)p2.Y);
            canvas.DrawLine((float)p2.X, (float)p2.Y, (float)p3.X, (float)p3.Y);
            canvas.DrawLine((float)p3.X, (float)p3.Y, (float)p1.X, (float)p1.Y);
        }
    }
}