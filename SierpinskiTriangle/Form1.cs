using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SierpinskiTriangle
{
    public partial class Form1 : Form
    {
        private Bitmap fractalBitmap;
        private bool isDrawing = false;
        private Graphics graphics;
        private int currentDepth = 0;
        private int targetDepth = 0;

        public Form1()
        {
            InitializeComponent();
            ConfigureComponents();
        }

        private void ConfigureComponents()
        {
            pictureBox.Size = new Size(380, 380);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.BackColor = Color.White;
            btnGenerate.Text = "Сгенерировать";
            label1.Text = "Количество итераций (0-7):";
        }

        private async void btnGenerate_Click(object sender, EventArgs e)
        {
            if (isDrawing)
            {
                isDrawing = false;
                btnGenerate.Text = "Сгенерировать";
                return;
            }

            if (!int.TryParse(txtIterations.Text, out targetDepth) || targetDepth < 0 || targetDepth > 7)
            {
                MessageBox.Show("Введите число от 0 до 7!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            isDrawing = true;
            btnGenerate.Text = "Остановить";

            fractalBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
            graphics = Graphics.FromImage(fractalBitmap);
            graphics.Clear(Color.White);
            pictureBox.Image = fractalBitmap;

            PointF top = new PointF(pictureBox.Width / 2, 20);
            PointF left = new PointF(20, pictureBox.Height - 20);
            PointF right = new PointF(pictureBox.Width - 20, pictureBox.Height - 20);

            await AnimateFractal(top, left, right);

            isDrawing = false;
            btnGenerate.Text = "Сгенерировать";
        }

        private async Task AnimateFractal(PointF top, PointF left, PointF right)
        {
            for (currentDepth = 0; currentDepth <= targetDepth; currentDepth++)
            {
                if (!isDrawing) break;

                graphics.Clear(Color.White);
                DrawAnimatedSierpinski(graphics, currentDepth, top, left, right);
                pictureBox.Refresh();
                await Task.Delay(500);
            }
        }

        private void DrawAnimatedSierpinski(Graphics g, int maxDepth, PointF top, PointF left, PointF right)
        {
            if (maxDepth == 0)
            {
                PointF[] points = { top, left, right };
                g.FillPolygon(Brushes.Black, points);
                return;
            }

            PointF midLeft = new PointF((top.X + left.X) / 2, (top.Y + left.Y) / 2);
            PointF midRight = new PointF((top.X + right.X) / 2, (top.Y + right.Y) / 2);
            PointF midBottom = new PointF((left.X + right.X) / 2, (left.Y + right.Y) / 2);

           
            void DrawLevel(int depth, PointF t, PointF l, PointF r)
            {
                if (depth == 0)
                {
                    PointF[] pts = { t, l, r };
                    g.FillPolygon(Brushes.Black, pts);
                    return;
                }

                if (depth > maxDepth) return;

                PointF ml = new PointF((t.X + l.X) / 2, (t.Y + l.Y) / 2);
                PointF mr = new PointF((t.X + r.X) / 2, (t.Y + r.Y) / 2);
                PointF mb = new PointF((l.X + r.X) / 2, (l.Y + r.Y) / 2);

                DrawLevel(depth - 1, t, ml, mr);
                DrawLevel(depth - 1, ml, l, mb);
                DrawLevel(depth - 1, mr, mb, r);
            }

            // Запуск отрисовки для текущего уровня
            DrawLevel(maxDepth, top, left, right);
        }

        private void Form1_Load(object sender, EventArgs e) { }
        private void pictureBox_Click(object sender, EventArgs e) { }
        private void txtIterations_TextChanged(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
    }
}