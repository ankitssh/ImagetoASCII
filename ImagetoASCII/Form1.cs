using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ImagetoASCII
{
    public partial class Form1 : Form
    {
        OpenFileDialog fd = new OpenFileDialog(); //Open File Dialog Box to import images
        Dictionary<string, string> previousFilePath = new Dictionary<string, string>(); //Holds the Filename and filepath of images
        private string pixels = "$&#MHGw*+-. "; //Characters to replace the pixels according to their brightness


        public Form1()
        {
            InitializeComponent();
            imagePreview.AllowDrop = true;
            label1.Text = "Drag n Drop an image or use File Open";
          
           
        }

        private void openFile_Click(object sender, EventArgs e)
        {
            
            fd.Title = "Open File";
            fd.Filter ="Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            fd.DefaultExt = ".jpg";

            if (fd.ShowDialog() == DialogResult.OK&&!imageHistory.Items.Contains(fd.SafeFileName))
            {
               
              
                imageHistory.Items.Insert(0,fd.SafeFileName); //Inserts the latest file at the top
                imagePreview.ImageLocation = fd.FileName; //Changes the picture box image to the location of latest image
                previousFilePath.Add(fd.SafeFileName,fd.FileName); 
                
            
            }
        }

        private void quitApp_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "MyASCIIArt.txt";
            sfd.Title = "Save File";
            sfd.Filter = "Text Files | *.txt";
            sfd.DefaultExt = ".txt";
            if (imagePreview.ImageLocation == null)
            {
                //Can't save image without having an image first
                MessageBox.Show("Please open an image first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
            if (sfd.ShowDialog() == DialogResult.OK)
            {

                StreamWriter sw = new StreamWriter(sfd.FileName);

                //ASCII Conversion Start
                try
                {
                    int percentHeight=0;
                    int percentWidth = 0;

                    var img = new Bitmap(imagePreview.ImageLocation);
                    if (img.Width > img.Height)
                    {

                         percentHeight = img.Height - (int)(img.Height * (30.00 / 100.00));
                         percentWidth = img.Width - (int)(img.Width * (70.00 / 100.00));
                    }
                    else if (img.Width < img.Height)
                    {
                        percentHeight = img.Height - (int)(img.Height * (70.00 / 100.00));
                        percentWidth = img.Width - (int)(img.Width * (30.00 / 100.00));

                    }
                    else
                    {
                        percentHeight = img.Height - (int)(img.Height * (50.00 / 100.00));
                        percentWidth = img.Width - (int)(img.Width * (50.00 / 100.00));
                    
                    }




                    img = new Bitmap(img, percentHeight, percentWidth); //Decreasing the Height and Width of the image
                    saveProgressBar.Minimum = 0;
                    saveProgressBar.Maximum = img.Height;

                    for (int y = 0; y < img.Height; y++)
                    {
                        for (int x = 0; x < img.Width; x++)
                        {
                            var color = img.GetPixel(x, y); //Reading color of each pixel on the image
                            var brightness = Brightness(color); //Get the lightness or darkness of the color
                            var idx = brightness / 255 * (pixels.Length - 1); //Help us get a value to pick a character from the pixel string
                            var pxl = pixels[(int)Math.Round(idx)];
                            sw.Write(pxl);//Writing the pixel



                        }
                        saveProgressBar.Value = saveProgressBar.Value + 1; //Increasing the progress bar

                        sw.WriteLine();

                    }
                    saveProgressBar.Value = 0; //Resets the progress bar
                    sw.Close();
                }


                catch (Exception ex)
                {
                    MessageBox.Show("Uh-oh! Something unexpected happened. Please check Log file for more info.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    StreamWriter sr = new StreamWriter("Log.txt", true);
                    sr.WriteLine(DateTime.Now + "\t" + ex.Message);
                    sr.Close();


                }

                //ASCII Conversion End

            }
        }
        private static double Brightness(Color c)
        {
            return Math.Sqrt(
                c.R*c.R*.241 +
                c.G*c.G*.691+
                c.B*c.B*.068
                );
 
        }

       

        private void imageHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
           imagePreview.ImageLocation=previousFilePath[imageHistory.SelectedItem.ToString()]; //Click on an image in Recent to load it

        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Created By\n Ankit Sharma (Soul_Hacker)\n Software is for non-commercial use only","About",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void imagePreview_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void imagePreview_DragDrop(object sender, DragEventArgs e)
        {
            //The drag and drop code 
            try
            {
                string dragLocation = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                string safeFileName = dragLocation.Substring(1 + dragLocation.LastIndexOf('\\'));
                if (dragLocation == null || imageHistory.Items.Contains(safeFileName))
                    return;

                imagePreview.ImageLocation = dragLocation;
                imageHistory.Items.Insert(0, safeFileName);
                previousFilePath.Add(safeFileName, dragLocation);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Uh-oh! Something unexpected happened. Please check Log file for more info.","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);

                StreamWriter sr = new StreamWriter("Log.txt", true);
                sr.WriteLine(DateTime.Now + "\t" + ex.Message);
                sr.Close();
            }

           
            
        }
    }
}
