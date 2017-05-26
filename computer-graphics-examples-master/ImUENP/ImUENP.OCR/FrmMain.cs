using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;

namespace ImUENP.OCR
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox.Image = System.Drawing.Image.FromFile(openFileDialog.FileName);
            }
        }
        Bitmap back;
        private void btnOCR_Click(object sender, EventArgs e)
        {
            var datapath = @"tessdata";
            var lang = "por";
            var img = (Bitmap)pictureBox.Image;
            var imageBackup = img;
            //Cinza
            img = Grayscale.CommonAlgorithms.Y.Apply(img);
            //Invert
            img = new Invert().Apply(img);
            //threshold
            //img = new OtsuThreshold().Apply(img);
            img = new Threshold(150).Apply(img);
            back = img;
            back = new Invert().Apply(back);
            for (int i = 0; i < 4; i++)
            {
                img = new Erosion().Apply(img);
            }
            img = new Invert().Apply(img);
            //Mediana
            //g = new Median(3).Apply(img);
            //Otsu
            //mg = new OtsuThreshold().Apply(img);
            BlobCounter blobcount = new BlobCounter();
            blobcount.ProcessImage(img);
            List<Rectangle> lista = new List<Rectangle>(blobcount.GetObjectsRectangles());
            List<Bitmap> bitmaps = new List<Bitmap>();

            for (int i = 0; i < lista.Count; i++)
            {
                Crop crop = new Crop(lista[i]);
                Bitmap a = crop.Apply(back);

                if (a.Width > a.Height)
                {
                    
                    if (a.Width < 300 && a.Height < 300 && a.Width > 50 && a.Height > 50)
                    {
                        if (a.Height > (a.Width * 0.25))
                        {

                            if ((back.Width * 0.8) > a.Width && (back.Height * 0.8) > a.Height)
                            {


                                a.Save(@"C:\Users\MatheusBento\Desktop\teste\" + a + i + ".jpg");
                                //a = new Opening().Apply(a);
                                // a = new Median().Apply(a);


                                bitmaps.Add(a);
                            }
                        }
                    }
                }
            }

            //this.pictureBox.Image = img;
            // this.pictureBox.Refresh();

            //var ocr = new TesseractEngine(datapath, lang, EngineMode.TesseractOnly);
            //var page = ocr.Process(bitmaps[49]);
            //txtText.Text += page.GetText();

            for (int i = 0; i < bitmaps.Count; i++)
            {

                // inverter e da blob extration e inverter

                // bitmaps[i] = new Median().Apply(bitmaps[i]);
                bitmaps[i].Save(@"C:\Users\MatheusBento\Desktop\teste\segundaetapa\" + bitmaps[i] + i + ".jpg");

                List<Bitmap> teste = analise(bitmaps[i]);
                for (int b = 0; b < teste.Count; b++)
                {
                    //teste[b] = new Median().Apply(teste[b]);
                    //teste[b] = new Invert().Apply(teste[b]);
                    Bitmap bitmapatual = new Bitmap(teste[b], new Size((int)(teste[b].Width * 50) / 100, (int)(teste[b].Height * 50) / 100));
                    Bitmap flag = new Bitmap(100, 100);

                    for (int ab = 0; ab < flag.Width; ab++)
                    {
                        for (int bb = 0; bb < flag.Height; bb++)
                        {
                            flag.SetPixel(ab, bb, Color.FromArgb(255, 255, 255));
                        }
                    }

                    for (int ab = 0; ab < bitmapatual.Width; ab++)
                    {
                        for (int bb = 0; bb < bitmapatual.Height; bb++)
                        {
                            flag.SetPixel(ab + 10, bb + 10, bitmapatual.GetPixel(ab, bb));


                        }
                    }
                    //flag = new Median().Apply(flag);

                    flag.Save(@"C:\Users\MatheusBento\Desktop\teste\criando\" + b + ".jpg");



                    var ocr = new TesseractEngine(datapath, lang, EngineMode.TesseractOnly);
                    var page = ocr.Process(flag);
                    txtText.Text += page.GetText();
                }

            }
        }
        private List<Bitmap> analise(Bitmap a)
        {
            a = new Closing().Apply(a);
            a = new Invert().Apply(a);
            BlobsFiltering blobfiltering = new BlobsFiltering();
            blobfiltering.MinHeight = 10;
            blobfiltering.MinWidth = 5;
            blobfiltering.Apply(a);

            BlobCounter blobcount = new BlobCounter();
            blobcount.ProcessImage(a);
            List<Rectangle> lista = new List<Rectangle>(blobcount.GetObjectsRectangles());
            List<Bitmap> bitmaps = new List<Bitmap>();


            for (int i = 1; i < lista.Count; i++)
            {
                Crop crop = new Crop(lista[i]);

                Bitmap aa = crop.Apply(a);

                if (aa.Height < 50 && aa.Width < 50)
                {
                    if (aa.Height > 15 && aa.Width > 5)
                    {
                        aa.Save(@"C:\Users\MatheusBento\Desktop\teste\pedacos\" + aa + i + ".jpg");

                        aa = new Invert().Apply(aa);
                        bitmaps.Add(aa);
                    }
                }
            }
            return bitmaps;
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var img = (Bitmap)pictureBox.Image;
            img = new Invert().Apply(img);
            BlobCounter bc = new BlobCounter();
            bc.ProcessImage(img);
            var blobs = bc.GetObjectsRectangles();
            MessageBox.Show("Objetos: " + blobs.Length);
        }
    }
}
