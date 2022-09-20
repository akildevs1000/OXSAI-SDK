using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;

namespace FCardProtocolAPI.Common
{
    public class ImageTool
    {
        static int faceMaxSize = 150 * 1024;
        static int faceMinSize = 20 * 1024;
        public static bool CheckFaceImage(string base64Fiel, out byte[] bImage)
        {
            bImage = null;
            try
            {
                if (!ChekcSize(base64Fiel))
                    return false;
                bImage = Convert.FromBase64String(base64Fiel);
                Image img = Image.FromStream(new System.IO.MemoryStream(bImage));
                var image = new Bitmap(img);//图片全路径
                if (image.Width > 480 || image.Height > 640 || image.Width < 102 || image.Height < 126)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        private static bool ChekcSize(string base64Fiel)
        {
            if (base64Fiel.StartsWith("data:image/jpg;base64,"))
            {
                base64Fiel = base64Fiel.Replace("data:image/jpg;base64,", "");
            }
            var strLen = base64Fiel.Length;
            var fileSize = strLen - (strLen / 8) * 2;
            if (fileSize > faceMaxSize || fileSize < faceMinSize)
            {
                return false;
            }
            return true;
        }


        //
        // 摘要:
        //     文件最大尺寸
        private const int ImageSizeMax = 153600;

        //
        // 摘要:
        //     进行图片转换，图片像素不能超过 480*640，大小尺寸不能超过50K
        //
        // 参数:
        //   strFile:
        public static byte[] ConvertImage(byte[] bImage)
        {
            Image image = Image.FromStream(new MemoryStream(bImage));
            float num = 1f;
            if (image.Width > 480 || image.Height > 640 || bImage.Length > 153600)
            {
                float num2 = 480f / (float)image.Width;
                float num3 = 640f / (float)image.Height;
                num = ((num2 > num3) ? num3 : num2);
                int width = image.Width;
                int height = image.Height;
                width = (int)((float)width * num);
                height = (int)((float)height * num);
                byte[] array = null;
                ImageCodecInfo encoder = GetEncoder(ImageFormat.Jpeg);
                System.Drawing.Imaging.Encoder quality = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters encoderParameters = new EncoderParameters(1);
                using (Bitmap bitmap = new Bitmap(480, 640, PixelFormat.Format32bppArgb))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.PageUnit = GraphicsUnit.Pixel;
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.Clear(Color.White);
                        graphics.DrawImage(image, new Rectangle((480 - width) / 2, (640 - height) / 2, width, height));
                        graphics.Dispose();
                    }

                    long num4 = 80L;
                    bool flag = false;
                    do
                    {
                        EncoderParameter encoderParameter = new EncoderParameter(quality, num4);
                        encoderParameters.Param[0] = encoderParameter;
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            bitmap.Save(memoryStream, encoder, encoderParameters);
                            encoderParameter.Dispose();
                            int num5 = (int)memoryStream.Length;
                            if (num5 <= 153600)
                            {
                                array = new byte[num5];
                                memoryStream.Position = 0L;
                                memoryStream.Read(array, 0, num5);
                                flag = true;
                            }

                            memoryStream.Close();
                            memoryStream.Dispose();
                            num4 -= 5;
                        }
                    }
                    while (!flag);
                    return array;
                }
            }

            image.Dispose();
            image = null;
            return bImage;
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] imageDecoders = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo imageCodecInfo in imageDecoders)
            {
                if (imageCodecInfo.FormatID == format.Guid)
                {
                    return imageCodecInfo;
                }
            }

            return null;
        }
    }
}
