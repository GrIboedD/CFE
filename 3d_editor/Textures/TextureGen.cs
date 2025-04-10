using System.Drawing.Imaging;
using System.Drawing.Text;


namespace _3d_editor._Textures
{
    class TextureGen: IDisposable
    {
        private readonly string outputPath;
        private readonly int width;
        private readonly int height;
        private readonly int fontSize;
        private readonly Color textColor;
        public TextureGen(string outputPath, int width, int height, int fontSize, Color color)
        {
            this.outputPath = outputPath;
            this.width = width;
            this.height = height;
            this.fontSize = fontSize;
            this.textColor = Color.Black;
            this.textColor = color;
        }



        public void CreateTransparentTextImage(string text, string fileName)
        {
            using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                var font = new Font("Arial", fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
                var format = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                var textBrush = new SolidBrush(textColor);
                var rect = new RectangleF(0, 0, width, height);
                graphics.DrawString(text, font, textBrush, rect, format);

            }

            bitmap.Save(Path.Combine(outputPath, fileName), ImageFormat.Png);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
