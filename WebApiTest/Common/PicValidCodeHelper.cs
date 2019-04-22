using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
namespace WebApiTest.Common
{
    public class PicValidCodeHelper
    {
        ///<summary>
        /// 生成随机字符串
        ///</summary>
        ///<param name="codeCount">字符数量</param>
        ///<returns>随机字符串</returns>
        public string CreateRandomCode(int codeCount)
        {
            string allChar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,W,X,Y,Z";
            string[] allCharArray = allChar.Split(',');
            string randomCode = "";
            int temp = -1;
            Random rand = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(35);
                if (temp == t)
                {
                    return CreateRandomCode(codeCount);
                }
                temp = t;
                randomCode += allCharArray[t];
            }
            return randomCode;
        }

        ///<summary>
        /// 创建验证码图片
        ///</summary>
        ///<param name="code">字符串</param>
        ///<param name="width">宽</param>
        ///<param name="height">高</param>
        ///<param name="fontSize">字号</param>
        public string GetImgWithValidateCode(string code, int width, int height, int fontSize)
        {
            Random varandom = new Random();
            int BgRandKey = varandom.Next(100, 200);
            int FontRandKey = varandom.Next(50, 100);

            Color bgcolor = GetControllableColor(BgRandKey);
            Color color = GetControllableColor(FontRandKey);

            int charNum = code.Length;
            Bitmap bitMap = null;
            Graphics gph = null;
            //创建内存流 
            MemoryStream memStream = new MemoryStream();
            Random random = new Random();

            //创建位图对象
            bitMap = new Bitmap(width, height);
            //根据上面创建的位图对象创建绘图图面
            gph = Graphics.FromImage(bitMap);
            //设定验证码图片背景色
            gph.Clear(bgcolor);
            //产生随机干扰线条 
            for (int i = 0; i < 1; i++)
            {
                Pen backPen = new Pen(color, 2);
                int x = 0;
                int y = height / 2;
                int x2 = width;
                int y2 = random.Next(height);
                gph.DrawLine(backPen, x, y, x2, y2);
            }

            SolidBrush sb = new SolidBrush(color);
            PointF Cpoint1 = new PointF(5, 5);
            Random rnd1 = new Random();
            int x1 = 0, y1 = 0;
            //通过循环,绘制每个字符,
            for (int i = 0; i < code.Length; i++)
            {
                x1 = rnd1.Next(2) + ((width - 10) / code.Length) * i;
                y1 = rnd1.Next(bitMap.Height / 4);
                Cpoint1 = new PointF(x1, y1);
                Font textFont = new Font("Arial", fontSize, FontStyle.Bold);//字体随机,字号大小30,加粗

                //随机倾斜字符
                Matrix transform = gph.Transform;
                transform.Shear(Convert.ToSingle(rnd1.NextDouble() - 0.5), 0.001f);
                gph.Transform = transform;
                gph.DrawString(code.Substring(i, 1), textFont, sb, Cpoint1);
                gph.ResetTransform();
            }

            //画图片的前景噪音点
            for (int i = 0; i < 10; i++)
            {
                int x = random.Next(bitMap.Width);
                int y = random.Next(bitMap.Height);
                bitMap.SetPixel(x, y, Color.White);
            }
            //画图片的边框线
            gph.DrawRectangle(new Pen(Color.Black, 2), 0, 0, bitMap.Width - 1, bitMap.Height - 1);
            try
            {
                bitMap.Save(memStream, ImageFormat.Gif);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            bitMap.Dispose();
            System.Drawing.Image img = System.Drawing.Image.FromStream(memStream);
            gph.DrawImage(img, 50, 20, width, height);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] b = ms.GetBuffer();
            string str64 = Convert.ToBase64String(b);
            gph.Dispose();
            img.Dispose();
            return str64;
        }

        ///<summary> 
        /// 产生一种 R,G,B 均大于 colorBase 随机颜色，以确保颜色不会过深 
        ///</summary> 
        ///<param name="colorBase">The color base.</param> 
        ///<returns>Color</returns>
        public Color GetControllableColor(int colorBase)
        {
            Color color = Color.Black;
            if (colorBase > 200)
            {
                return color;
            }
            Random random = new Random();
            color = Color.FromArgb(random.Next(56) + colorBase, random.Next(56) + colorBase, random.Next(56) + colorBase);
            return color;
        }

    }
}