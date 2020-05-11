using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageClusteringLibrary.Algorithms;

namespace ImageClusteringUI
{
    public partial class Form1 : Form
    {
        private Solver _solver;
        private int clusterCount = 20;
        private string feininger = "C:\\Users\\Lando1\\Desktop\\lyonel-feininger-ostsee-schoner.jpg";

        public Form1()
        {
            InitializeComponent();

            var link = "C:\\Users\\Lando1\\Google Drive\\Wohnzimmer_auf_dem_sofa_000.png";
            _solver = new Solver(feininger, clusterCount);
            _solver.Next();
            pB_Result.Image = _solver.GetClusterImage();
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                _solver.Next();
            }

            pB_Result.Image = _solver.GetClusterImage();
        }

        private void btn_Reset_Click(object sender, EventArgs e)
        {
            var newFunction = _solver.Function;
            switch (newFunction)
            {
                case ClusterRandomFunction.RgbDistance:
                    newFunction = ClusterRandomFunction.HueShift;
                    break;
                case ClusterRandomFunction.HueShift:
                    newFunction = ClusterRandomFunction.RgbDistance;
                    break;
            }

            _solver = new Solver(feininger, clusterCount) {Function = newFunction};

            _solver.Restart(clusterCount);

            pB_Result.Image = _solver.GetClusterImage();
        }
    }
}
