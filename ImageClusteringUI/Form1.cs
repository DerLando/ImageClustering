﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageClusteringLibrary.Algorithms;
using ImageClusteringLibrary.IO;

namespace ImageClusteringUI
{
    public partial class Form1 : Form
    {
        private Solver _solver;
        private int clusterCount = 20;
        private string feininger = "D:\\Desktop\\cornwall_stock.jpg";
        private Bitmap image;

        public Form1()
        {
            InitializeComponent();

            _solver = new Solver(feininger, clusterCount);
            _solver.Next();
            //pB_Result.Image = _solver.GetClusterImage();

            image = (new Bitmap(feininger)).ResizeImage(400, 300);

            var collection = SuperPixelSolver.Solve(image, 50, 10);
            var boundaries = collection.GetPixelBoundaryPositions();

            foreach (var boundaryCollection in boundaries)
            {
                foreach (var position in boundaryCollection)
                {
                    image.SetPixel(position.Vector.X, position.Vector.Y, Color.Red);
                }
            }

            pB_Result.Image = image;


            // DEBUG:
            //var test = SuperPixelSolver.CalculateGrid(new Rectangle(0, 0, 70, 100), 20);
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

        private void pB_Result_Click(object sender, EventArgs e)
        {

        }
    }
}
