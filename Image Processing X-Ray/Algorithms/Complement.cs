using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Image_Processing_X_Ray.Algorithms
{
    class Complement
    {
        unsafe

        public static Bitmap ImComplement(Bitmap bitmap)
        {
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            byte* p = (byte*)data.Scan0;

            int offset = data.Stride - bitmap.Width * 3;

            for(int y = 0; y < bitmap.Height; y++)
            {
                for(int x = 0; x < bitmap.Width; x++)
                {
                    if( p[0] == 0)
                    {
                        p[0] = 255;
                        p[1] = 255;
                        p[2] = 255;
                    }
                    else
                    {
                        p[0] = 0;
                        p[1] = 0;
                        p[2] = 0;
                    }
                    p += 3;
                }
                p += offset;
            }
            bitmap.UnlockBits(data);
            return bitmap;
        }

        

    }
}
