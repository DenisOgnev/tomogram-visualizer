using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ognev_tomogram_visualizer.Classes;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Ognev_tomogram_visualizer
{
    public partial class Form1 : Form
    {
        Bin bin = new Bin();
        Classes.View view = new Classes.View();
        bool loaded = false;
        int currentLayer = 1;
        int frameCount;
        DateTime nextFPSupdate = DateTime.Now.AddSeconds(1);
        bool needReload = false;
        string mode = "Quad";

        
        public Form1()
        {
            InitializeComponent();
        }

        private void открытьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                bin.ReadBIN(str);
                view.SetupView(glControl1.Width, glControl1.Height);
                trackBar1.Maximum = Bin.Z - 1;
                trackBar2.Maximum = 100;
                trackBar3.Maximum = 2000;
                trackBar3.Value = trackBar3.Maximum;
                loaded = true;
                glControl1.Invalidate();
            }
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if(loaded)
            {
                if (mode == "Quad") 
                {
                    view.DrawQuads(currentLayer);
                    glControl1.SwapBuffers();
                }
                else if(mode == "QuadStrip")
                {
                    view.DrawQuadsStrip(currentLayer);
                    glControl1.SwapBuffers();
                }
                else if (mode == "Texture")
                {
                    if (needReload)
                    {
                        view.GenerateTextureImage(currentLayer);
                        view.Load2Dtexture();
                        needReload = true;
                    }
                    view.DrawTexture();
                    glControl1.SwapBuffers();
                }
                //view.DrawQuads(currentLayer);
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            needReload = true;
            //glControl1.Invalidate();
        }

        void ApplicationIdle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                DisplayFPS();
                glControl1.Invalidate();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += ApplicationIdle;    
        }

        void DisplayFPS()
        {
            if (DateTime.Now >= nextFPSupdate) 
            {
                this.Text = String.Format("CT Visualizer (fps = {0})", frameCount);
                nextFPSupdate = DateTime.Now.AddSeconds(1);
                frameCount = 0;
            }
            frameCount++;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                mode = "Quad";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
                mode = "Texture";
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            view.SetMin(trackBar2.Value);
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            view.SetWidth(trackBar3.Value);
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
                mode = "QuadStrip";
        }
    }
}
