using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Lab7___Scribble
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double m_StartX = 0;
        double m_StartY = 0;
        bool m_DrawingStarted = false;
        Shape m_shape;
        Color m_BorderColor = Colors.Black;
        Color m_FillColor = Colors.Black;
        bool m_freeDraw = false;
        Polyline m_line;
        Point m_startingPoint;
        int m_Thickness = 5;
        Shape m_Selected = null;
        bool m_moving = false;
        Shape m_curMoving;
        string m_shapeType = "Rectangle";
        int m_lowestZIndex = -1;
        int m_highestZIndex = 1;
        bool m_resize = false;
        public MainWindow()
        {
            InitializeComponent();

            Rectangle rectangle = new Rectangle();
            rectangle.Height = 20;
            rectangle.Width = 40;
            rectangle.Fill = new SolidColorBrush(Colors.Black);
            ShapeRectangle.Content = rectangle;

            Ellipse ellipse = new Ellipse();
            ellipse.Height = 20;
            ellipse.Width = 40;
            ellipse.Fill = new SolidColorBrush(Colors.Black);
            ShapeEllipse.Content = ellipse;

            Line line = new Line();
            line.X1 = 0;
            line.Y1 = 0;
            line.X2 = 20;
            line.Y2 = 20;
            line.Stroke = new SolidColorBrush(Colors.Black);
            ShapeLine.Content = line;

            ShapeRectangle.BorderThickness = new Thickness(4);
            ShapeRectangle.BorderBrush = new SolidColorBrush(Colors.Brown);

            Thickness5.BorderThickness = new Thickness(4);
            Thickness5.BorderBrush = new SolidColorBrush(Colors.Brown);
        }

        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (!m_DrawingStarted && !m_freeDraw)
                {
                    m_StartX = e.GetPosition(MyCanvas).X;
                    m_StartY = e.GetPosition(MyCanvas).Y;
                    m_DrawingStarted = true;
                }
                else if (m_freeDraw)
                {
                    Point cur = e.GetPosition(MyCanvas);
                    if (m_startingPoint != cur)
                    {
                        m_line.Points.Add(cur);
                    }
                }
                else if (m_moving)
                {
                    if (m_curMoving.GetType() == typeof(Line))
                    {
                        ((Line)m_curMoving).X2 = ((Line)m_curMoving).X2 - ((Line)m_curMoving).X1 + e.GetPosition(MyCanvas).X;
                        ((Line)m_curMoving).Y2 = ((Line)m_curMoving).Y2 - ((Line)m_curMoving).Y1 + e.GetPosition(MyCanvas).Y;


                        ((Line)m_curMoving).X1 = e.GetPosition(MyCanvas).X;
                        ((Line)m_curMoving).Y1 = e.GetPosition(MyCanvas).Y;

                    }
                    else
                    {
                        Canvas.SetLeft(m_curMoving, e.GetPosition(MyCanvas).X - (m_curMoving.Width / 2));
                        Canvas.SetTop(m_curMoving, e.GetPosition(MyCanvas).Y - (m_curMoving.Height / 2));
                    }
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            MyCanvas.Children.Clear();
        }

        private void MyCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Shape obj = e.OriginalSource as Shape;
            if (obj != null)
            {
                obj.Opacity = .50;
                m_Selected = obj;

                obj.ContextMenu = new ContextMenu();

                MenuItem delete = new MenuItem();
                delete.Header = "Delete";
                delete.Click += new RoutedEventHandler(ContextMenuClick);
                MenuItem resize = new MenuItem();
                resize.Header = "Resize";
                resize.Click += new RoutedEventHandler(ContextMenuClick);
                MenuItem unselect = new MenuItem();
                unselect.Header = "Unselect";
                unselect.Click += new RoutedEventHandler(ContextMenuClick);
                MenuItem DoneResizing = new MenuItem();
                DoneResizing.Header = "Done Resizing";
                DoneResizing.Click += new RoutedEventHandler(ContextMenuClick);

                MenuItem ZOrder = new MenuItem();
                ZOrder.Header = "ZOrder";
                MenuItem SendBack = new MenuItem();
                SendBack.Header = "Send to Back";
                SendBack.Click += new RoutedEventHandler(ContextMenuClick);
                MenuItem SendFront = new MenuItem();
                SendFront.Header = "Send to Front";
                SendFront.Click += new RoutedEventHandler(ContextMenuClick);

                ZOrder.Items.Add(SendBack);
                ZOrder.Items.Add(SendFront);

                obj.ContextMenu.Items.Add(delete);
                obj.ContextMenu.Items.Add(resize);
                obj.ContextMenu.Items.Add(DoneResizing);
                obj.ContextMenu.Items.Add(unselect);
                obj.ContextMenu.Items.Add(ZOrder);
            }
        }

        private void ContextMenuClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            switch (item.Header.ToString())
            {
                case "Delete":
                    MyCanvas.Children.Remove(m_Selected);
                    break;
                case "Resize":
                    m_resize = true;
                    break;
                case "Unselect":
                    m_Selected.Opacity = 1;
                    m_Selected = null;
                    m_resize = false;
                    break;
                case "Done Resizing":
                    m_resize = false;
                    break;
                case "Send to Back":
                    Canvas.SetZIndex(m_Selected, m_lowestZIndex--);
                    break;
                case "Send to Front":
                    Canvas.SetZIndex(m_Selected, m_highestZIndex++);
                    break;
            }
        }

        private void MyCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (m_DrawingStarted == false)
                return;

            m_DrawingStarted = false;
            double endX = e.GetPosition(MyCanvas).X;
            double endY = e.GetPosition(MyCanvas).Y;
            if (m_moving)
            {
                m_moving = false;
            }
            else if ((m_shapeType == "Rectangle" || m_shapeType == "Ellipse") && !m_resize)
            {
                if (m_shapeType == "Rectangle")
                {
                    m_shape = new Rectangle();
                }
                else if (m_shapeType == "Ellipse")
                {
                    m_shape = new Ellipse();
                }
                m_shape.Stroke = new SolidColorBrush(m_BorderColor);
                m_shape.StrokeThickness = m_Thickness;
                m_shape.Fill = new SolidColorBrush(m_FillColor);

                if (endX > m_StartX)
                {
                    m_shape.Width = endX - m_StartX;
                    Canvas.SetLeft(m_shape, m_StartX);
                }
                else
                {
                    m_shape.Width = m_StartX - endX;
                    Canvas.SetLeft(m_shape, endX);
                }
                if (endY > m_StartY)
                {
                    m_shape.Height = endY - m_StartY;
                    Canvas.SetTop(m_shape, m_StartY);
                }
                else
                {
                    m_shape.Height = m_StartY - endY;
                    Canvas.SetTop(m_shape, endY);
                }

                MyCanvas.Children.Add(m_shape);
            }
            else if (m_shapeType == "Line" && !m_resize)
            {
                Line line = new Line();
                line.X1 = m_StartX;
                line.Y1 = m_StartY;
                line.X2 = endX;
                line.Y2 = endY;
                line.StrokeThickness = m_Thickness;
                MyCanvas.Children.Add(line);
                line.Stroke = new SolidColorBrush(m_BorderColor);
                line.Fill = new SolidColorBrush(m_FillColor);
            }
        }

        private void MyCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (m_freeDraw)
            {
                m_startingPoint = e.GetPosition(MyCanvas);
                m_line = new Polyline();
                m_line.Stroke = new SolidColorBrush(m_BorderColor);
                m_line.StrokeThickness = m_Thickness;

                MyCanvas.Children.Add(m_line);
            }
            else if (!m_freeDraw && m_Selected != null && m_Selected == e.OriginalSource as Shape && !m_resize)
            {
                m_moving = true;
                m_curMoving = e.OriginalSource as Shape;
            }
            else if (m_resize)
            {
                
                if (e.GetPosition(MyCanvas).X > Canvas.GetLeft(m_Selected) && e.GetPosition(MyCanvas).Y > Canvas.GetTop(m_Selected) && (m_Selected.GetType() == typeof(Rectangle) || m_Selected.GetType() == typeof(Ellipse)))
                {
                    m_Selected.Width = Math.Abs(e.GetPosition(MyCanvas).X - Canvas.GetLeft(m_Selected));
                    m_Selected.Height += e.GetPosition(MyCanvas).Y - (Canvas.GetTop(m_Selected) + m_Selected.Height);
                }
                else if(m_Selected.GetType() == typeof(Line) && (e.GetPosition(MyCanvas).X > ((Line)m_Selected).X1) || e.GetPosition(MyCanvas).Y > ((Line)m_Selected).Y1)
                {
                    ((Line)m_Selected).X2 = e.GetPosition(MyCanvas).X;
                    ((Line)m_Selected).Y2 = e.GetPosition(MyCanvas).Y;
                }
            }
        }


        private void Shape_Click(object sender, RoutedEventArgs e)
        {
            ShapeRectangle.BorderThickness = new Thickness(0);
            ShapeEllipse.BorderThickness = new Thickness(0);
            ShapeLine.BorderThickness = new Thickness(0);
            ShapeDraw.BorderThickness = new Thickness(0);

            Button button = sender as Button;

            button.BorderThickness = new Thickness(2);
            button.BorderBrush = new SolidColorBrush(Colors.Brown);

            switch (button.Name)
            {
                case "ShapeRectangle":
                    m_shapeType = "Rectangle";
                    m_freeDraw = false;
                    break;
                case "ShapeEllipse":
                    m_shapeType = "Ellipse";
                    m_freeDraw = false;
                    break;
                case "ShapeLine":
                    m_shapeType = "Line";
                    m_freeDraw = false;
                    break;
                case "ShapeDraw":
                    m_freeDraw = true;
                    m_shapeType = "Draw";
                    break;
            }
        }

        private void Thickness_Click(object sender, RoutedEventArgs e)
        {
            Thickness2.BorderThickness = new Thickness(0);
            Thickness5.BorderThickness = new Thickness(0);
            Thickness10.BorderThickness = new Thickness(0);
            Thickness20.BorderThickness = new Thickness(0);

            Button button = sender as Button;

            button.BorderThickness = new Thickness(4);
            button.BorderBrush = new SolidColorBrush(Colors.Brown);

            switch (button.Name)
            {
                case "Thickness2":
                    m_Thickness = 2;
                    break;
                case "Thickness5":
                    m_Thickness = 5;
                    break;
                case "Thickness10":
                    m_Thickness = 10;
                    break;
                case "Thickness20":
                    m_Thickness = 20;
                    break;
            }
            if (m_Selected != null)
            {
                m_Selected.StrokeThickness = m_Thickness;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            MyCanvas.Children.Clear();
        }

        private void BorderChooser_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            m_BorderColor = BorderChooser.SelectedColor.Value;
            if(m_Selected != null)
            {
                m_Selected.Stroke = new SolidColorBrush(m_BorderColor);
            }
        }

        private void FillChooser_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            m_FillColor = FillChooser.SelectedColor.Value;
            if (m_Selected != null)
            {
                m_Selected.Fill = new SolidColorBrush(m_FillColor);
            }
        }
    }
}
