using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Image_Processing_X_Ray
{
    class Histogram
    {
        //row , column , stride
        public static int indexOf(int row,int column,int stride)
        {
            return row* stride + column* 3;
        }

        unsafe

        public static Bitmap CreateHistogram(Bitmap bitmap,bool isGray)
        {
            //if(bitmap.PixelFormat==PixelFormat.Format24bppRgb)
            //{
                Bitmap histogram = new Bitmap(256, 256, PixelFormat.Format24bppRgb);
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                ImageLockMode.ReadWrite, bitmap.PixelFormat);
                byte *p =(byte*)data.Scan0;

                int offset = data.Stride - bitmap.Width * 3;

                if(isGray==false)
                {
                    for(int i=0;i<bitmap.Height;i++)
                    {
                        for(int j=0;j<bitmap.Width;j++)
                        {
                            byte t = (byte)(p[0]*0.07f + p[1]*0.72f + p[2]*0.21);
                            p[0] = p[1] = p[2] = t;
                            p += 3;
                        }
                        p += offset;
                    }
                    
                    p =(byte*)data.Scan0;
                }

                //Count probability

                int[] count = new int[256];
                int max=0;
                //i->height
                //j->row
                for(int i=0;i<bitmap.Height;i++)
                {
                    for(int j=0;j<bitmap.Width;j++)
                    {
                        count[p[0]]++;
                        
                        if(count[p[0]]>max)
                        {
                            max = count[p[0]];
                        }

                        p += 3;
                    }
                    p += offset;
                }
                bitmap.UnlockBits(data);

                //covert to scale of picture box 2
                //max->255
                //x=>(x*255)/max

                for(int i=0;i<256;i++)
                {
                    count[i]=(int)(count[i]*(histogram.Height-1)*1f/max*1f);
                }

               
                data=histogram.LockBits(new Rectangle(0,0,histogram.Width,histogram.Height),
                           ImageLockMode.ReadWrite,histogram.PixelFormat);
                
                p = (byte*)data.Scan0;
                offset = data.Stride - histogram.Width * 3;

                for (int row = 0; row < histogram.Height; row++)
                {
                    for (int column = 0; column < histogram.Width; column++)
                    {
                        byte value=255;

                        if(row<=(histogram.Height-count[column]))
                        {
                            value = 255;
                        }
                        else
                        {
                            value = 0;
                        }

                        p[indexOf(row, column, data.Stride)] = value;
                        p[indexOf(row, column, data.Stride) + 1] = value;
                        p[indexOf(row, column, data.Stride) + 2] = value;
                    }
                }
                histogram.UnlockBits(data);
                return histogram;
            //}
            //else
            //{
            //    MessageBox.Show("PixelFormat: "+bitmap.PixelFormat);
            //    return bitmap;
            //}
        }
    }
}
