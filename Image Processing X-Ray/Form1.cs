using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Image_Processing_X_Ray.Algorithms;
using Image_Processing_X_Ray.DataStructures;
using System.IO;

namespace Image_Processing_X_Ray
{
    public partial class Form1 : Form
    {
        Bitmap bitmap;
        int threshold;
        Bitmap otsu_bitmap;
        private DataStructures.Matrix InputMatrix;
        List<int> regionNumbers= new List<int>();
        Bitmap bitmap_label;
        int[] maxarray = new int[8];
        Matrix mask_label;
        Bitmap mask_bitmap;
        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            if(open.ShowDialog()==System.Windows.Forms.DialogResult.OK)
            {
                bitmap = new Bitmap(open.FileName);
                pictureBox1.Image = bitmap;
            }
            //MessageBox.Show(bitmap.Width + "\n" + bitmap.Height);
            
        }

        private void Form1_Load(object sender, EventArgs e) { }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Bitmap bitmaphist = (Bitmap)pictureBox1.Image.Clone(); 
            Bitmap bitmaphist = (Bitmap)pictureBox1.Image.Clone();
            //Bitmap h = Histogram.CreateHistogram(bitmap, false);
            Bitmap hist = Histogram.CreateHistogram(bitmaphist, true);
            pictureBox2.Image =hist;
        }

        private void otsuThresholdingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //int threshold
            Bitmap bitmapotsu = (Bitmap)pictureBox1.Image.Clone();
            otsu_bitmap = OtsuThresholding.Otsu_Threshold(bitmapotsu,true,ref threshold);
            pictureBox2.Image = otsu_bitmap;
            //MessageBox.Show("Thresholding value: " + threshold);
            textBox1.Text = threshold.ToString();
        }

        private void connectedSetsLabelingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Convert otsu Image to Input Matrix
            unsafe
            {
                BitmapData data = otsu_bitmap.LockBits(new Rectangle(0, 0, otsu_bitmap.Width, otsu_bitmap.Height), ImageLockMode.WriteOnly, otsu_bitmap.PixelFormat);
                byte* p = (byte*)data.Scan0;
                int offset = data.Stride - otsu_bitmap.Width * 3;

                InputMatrix = new Matrix(otsu_bitmap.Width,otsu_bitmap.Height);
                for(int y=0;y<otsu_bitmap.Height;y++)
                {
                    for(int x=0;x<otsu_bitmap.Width;x++)
                    {
                        InputMatrix.Values[x, y] = p[0]<255?1:0;
                        p += 3;
                    }
                    p += offset;
                }
                otsu_bitmap.UnlockBits(data);
            }
            //Assign otsu_bitmap to mask_bitmap
            
            //Start labeling
            DataStructures.Matrix regionMatrix = null;
            SetList regionSets = null;
            Vector finalRegionSets = null;

            regionMatrix = RegionFinding.MarkRegions(InputMatrix, out regionSets);
            regionMatrix = RegionFinding.MergeRegions(regionMatrix, regionSets, out finalRegionSets,out regionNumbers);
            //MessageBox.Show(regionNumbers.Count().ToString());
            //Write file regionMatrix
            InputMatrix.Values = regionMatrix.Values;
            mask_label = new Matrix(InputMatrix.Width,InputMatrix.Height);
            mask_label.Values = InputMatrix.Values;
            #region
            //FileStream fs = new FileStream(@"P:\MatrixRegion.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            //// var site = new int[regionMatrix.Width,regionMatrix.Height];
            //StreamWriter sw = new StreamWriter(fs);
            ////Declare array
            //for (int y = 0; y < regionMatrix.Height; y++)
            //{

            //    for (int x = 0; x < regionMatrix.Width; x++)
            //    {
            //        sw.Write(regionMatrix.Values[x, y] + " ");
            //    }
            //    sw.Write("\n");
            //}
            //sw.Flush();
            //sw.Close();
            //fs.Close();
            #endregion
            
            #region 
            //Finde maximize in regionMatrix.Values
            int max = (from int m in InputMatrix.Values select m).Max();

            //Find histogram of regionMatrix.Values
            int[] count = new int[max + 1];
            for (int j = 0; j < InputMatrix.Height; j++)
            {
                for (int i = 0; i < InputMatrix.Width; i++)
                {
                    count[InputMatrix.Values[i, j]]++;
                }
            }


            int loop = 0;
            while (loop < 7)
            {
                int s = 0;
                int m = count.Max();
                for (int i = 0; i <= max; i++)
                {
                    if (count[i] == m)
                    {
                        s = i;
                        maxarray[loop] = i;
                        count[i] = 0;
                    }
                }
                loop++;
            }
            #endregion

            #region

            unsafe
            {
                bitmap_label = new Bitmap(otsu_bitmap.Width, otsu_bitmap.Height, PixelFormat.Format24bppRgb);

                BitmapData data = bitmap_label.LockBits(new Rectangle(0, 0, bitmap_label.Width, bitmap_label.Height), ImageLockMode.ReadWrite, bitmap_label.PixelFormat);
                byte* p = (byte*)data.Scan0;
                int offset = data.Stride - bitmap_label.Width * 3;

                //InputMatrix = new Matrix(otsu_bitmap.Width, otsu_bitmap.Height);
                for (int y = 0; y < bitmap_label.Height; y++)
                {
                    for (int x = 0; x < bitmap_label.Width; x++)
                    {
                        if (InputMatrix.Values[x, y] == maxarray[0])
                        {
                            if (maxarray[0] == 0)
                            {
                                p[0] = 255;
                                p[1] = 255;
                                p[2] = 255;
                                //continue;
                            }
                            else
                            {
                                p[0] = 50;
                                p[1] = 0;
                                p[2] = 127;
                            }
                        }
                        else if (InputMatrix.Values[x, y] == maxarray[1])
                        {
                            if (maxarray[1] == 0)
                            {
                                p[0] = 255;
                                p[1] = 255;
                                p[2] = 255;
                                //continue;
                            }
                            else
                            {
                                p[0] = 204;
                                p[1] = 0;
                                p[2] = 204;
                            }
                        }
                        else if (InputMatrix.Values[x, y] == maxarray[2])
                        {
                            if (maxarray[2] == 0)
                            {
                                p[0] = 255;
                                p[1] = 255;
                                p[2] = 255;
                                //continue;
                            }
                            else
                            {
                                p[0] = 51;
                                p[1] = 0;
                                p[2] = 100;
                            }
                        }
                        else if (InputMatrix.Values[x, y] == maxarray[3])
                        {
                            if (maxarray[3] == 0)
                            {
                                p[0] = 255;
                                p[1] = 255;
                                p[2] = 255;
                                //continue;
                            }
                            else
                            {
                                p[0] = 200;
                                p[1] = 0;
                                p[2] = 152;
                            }
                        }
                        else if (InputMatrix.Values[x, y] == maxarray[4])
                        {
                            if (maxarray[4] == 0)
                            {
                                p[0] = 255;
                                p[1] = 255;
                                p[2] = 255;

                            }
                            else
                            {
                                p[0] = 127;
                                p[1] = 0;
                                p[2] = 90;
                            }
                        }
                        else if (InputMatrix.Values[x, y] == maxarray[5])
                        {
                            if (maxarray[5] == 0)
                            {
                                p[0] = 255;
                                p[1] = 255;
                                p[2] = 255;

                            }
                            else
                            {
                                p[0] = 127;
                                p[1] = 255;
                                p[2] = 0;
                            }
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
                bitmap_label.UnlockBits(data);
            }
            pictureBox2.Image = bitmap_label;
            #endregion
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void maskOfLungToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region
            int label_left_lung;
            int label_rigth_lung;
            label_left_lung = mask_label.Values[(mask_label.Width / 4), (mask_label.Height / 2)];
            label_rigth_lung = mask_label.Values[(mask_label.Width / 4) * 3, (mask_label.Height / 2)];

            for (int y = 0; y < mask_label.Height; y++)
            {
                for (int x = 0; x < mask_label.Width; x++)
                {
                    if (mask_label.Values[x, y] == label_left_lung)
                    {
                        //mask_label.Values[x, y] = label_left_lung;
                        continue;
                    }
                    else if (mask_label.Values[x, y] == label_rigth_lung)
                    {
                        //mask_label.Values[x, y] = label_rigth_lung;
                        continue;
                    }
                    else
                    {
                        mask_label.Values[x, y] = 0;
                    }
                }
            }
            #endregion

            #region
            //FileStream fs = new FileStream(@"P:\MatrixRegion.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            //StreamWriter sw = new StreamWriter(fs);
            //for (int y = 0; y < mask_label.Height; y++)
            //{
            //    for (int x = 0; x < mask_label.Width; x++)
            //    {
            //        sw.Write(mask_label.Values[x, y] + " ");
            //    }
            //    sw.Write("\n");
            //}
            //sw.Flush();
            //sw.Close();
            //fs.Close();
            #endregion

            #region
            unsafe
            {
                mask_bitmap = new Bitmap(otsu_bitmap.Width, otsu_bitmap.Height, PixelFormat.Format24bppRgb);
                BitmapData data = mask_bitmap.LockBits(new Rectangle(0, 0, mask_bitmap.Width, mask_bitmap.Height), ImageLockMode.ReadWrite, mask_bitmap.PixelFormat);
                byte* p = (byte*)data.Scan0;
                int offset = data.Stride - mask_bitmap.Width * 3;

                for (int y = 0; y < mask_bitmap.Height; y++)
                {
                    for (int x = 0; x < mask_bitmap.Width; x++)
                    {
                        if (mask_label.Values[x, y] == label_left_lung || mask_label.Values[x,y] == label_rigth_lung)
                        {
                            p[0] = 0;
                            p[1] = 0;
                            p[2] = 0;
                        }
                        else
                        {
                            p[0] = 255;
                            p[1] = 255;
                            p[2] = 255;
                        }
                        p += 3;
                    }
                    p += offset;
                }
                mask_bitmap.UnlockBits(data);
            }
            pictureBox2.Image = mask_bitmap;
            #endregion
        }

        private void complementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap complement_bitmap = new Bitmap(otsu_bitmap.Width,otsu_bitmap.Height);
            complement_bitmap = Complement.ImComplement(otsu_bitmap);
            pictureBox2.Image = complement_bitmap;
        }
    }
}
