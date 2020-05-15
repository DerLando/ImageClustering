using ImageClusteringLibrary.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ImageClusteringLibrary.Algorithms;
using ImageClusteringLibrary.IO;
using ImageClusteringUI.Commands;
using ImageClusteringUI.Converters;

namespace ImageClusteringUI.ViewModels
{
    public class KMeansClusteringViewModel : ViewModelBase
    {
        #region private backing fields

        private Bitmap _originalBitmap = new Bitmap("D:\\Desktop\\cornwall_stock.jpg"); // Debug for now
        private Bitmap _clusteredBitmap;
        private int _superPixelCount;
        private int _superPixelCompactness;
        private int _kmeansClusterCount;
        private bool _shouldShowStatus;
        private string _currentWorkerTask;
        private int _progressValue;

        private IReadOnlyCollection<SuperPixelData> _superPixels;
        private IReadOnlyCollection<ImagePixel> _imagePixels;
        /// <summary>
        /// If we need to re-calculate super pixels
        /// </summary>
        private bool _needSuperPixelRecalculation = true;

        #endregion

        #region public properties

        /// <summary>
        /// The bitmap image on which the image segmentation algorithms are run
        /// </summary>
        public BitmapImage Image => BitmapToBitmapImageConverter.Convert(_clusteredBitmap);


        /// <summary>
        /// The number of super pixels to generate for the image
        /// </summary>
        public int SuperPixelCount
        {
            get => _superPixelCount;
            set
            {
                if (value == _superPixelCount)
                    return;

                _superPixelCount = value;
                _needSuperPixelRecalculation = true;
                RaisePropertyChanged(nameof(SuperPixelCount));
            }
        }

        /// <summary>
        /// The compactness measure of the generated superpixels.
        /// valid values range between 1 and 20, 10 is a good overall tradeoff
        /// </summary>
        public int SuperPixelCompactness
        {
            get => _superPixelCompactness;
            set
            {
                if (value == _superPixelCompactness)
                    return;

                _superPixelCompactness = value;
                _needSuperPixelRecalculation = true;
                RaisePropertyChanged(nameof(SuperPixelCompactness));
            }
        }

        /// <summary>
        /// Number of color clusters to segment the image in
        /// </summary>
        public int KmeansClusterCount
        {
            get => _kmeansClusterCount;
            set
            {
                _kmeansClusterCount = value;
                RaisePropertyChanged(nameof(KmeansClusterCount));
            }
        }

        /// <summary>
        /// If the viewmodel should show a status bar
        /// </summary>
        public bool ShouldShowStatus
        {
            get => _shouldShowStatus;
            set
            {
                _shouldShowStatus = value;
                RaisePropertyChanged(nameof(ShouldShowStatus));
            }
        }

        /// <summary>
        /// The current task the background worker is working on
        /// Can be an empty string if nothing is done right now
        /// </summary>
        public string CurrentWorkerTask
        {
            get => _currentWorkerTask;
            set
            {
                _currentWorkerTask = value;
                RaisePropertyChanged(nameof(CurrentWorkerTask));
            }
        }

        /// <summary>
        /// The progress value on the progress bar from 0 to 100
        /// </summary>
        public int ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = value;
                RaisePropertyChanged(nameof(ProgressValue));
            }
        }

        #endregion

        #region Constructor

        public KMeansClusteringViewModel()
        {
            SuperPixelCount = 200;
            SuperPixelCompactness = 10;
            KmeansClusterCount = 6;
            _clusteredBitmap = new Bitmap(_originalBitmap);

            RefreshCommand = new RelayCommand(o => RefreshButtonClick(null));
            SaveCommand = new RelayCommand(o => SaveButtonClick(null));
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command to select an image
        /// </summary>
        public ICommand SelectImageCommand { get; set; }

        public ICommand RefreshCommand { get; set; }

        public ICommand SaveCommand { get; set; }

        #endregion

        #region Methods

        private void SaveButtonClick(object sender)
        {
            _clusteredBitmap.Save("D:\\Desktop\\clustering-result.jpg");
        }

        private void RefreshButtonClick(object sender)
        {
            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_RefreshClustering;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_Completed;

            worker.RunWorkerAsync();
        }

        private void Worker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            // draw clustering
            DrawClustering();

            // disable progress
            CurrentWorkerTask = "";
            ShouldShowStatus = false;

            // update image ui
            RaisePropertyChanged(nameof(Image));
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressValue = e.ProgressPercentage;
        }

        private void Worker_RefreshClustering(object sender, DoWorkEventArgs e)
        {
            if (_needSuperPixelRecalculation)
            {
                // update ui
                _needSuperPixelRecalculation = false;
                CurrentWorkerTask = "Calculating superpixels ... ";
                ShouldShowStatus = true;

                // calculate the super pixels
                CalculateSuperPixels(sender);
            }

            // reset image
            ResetClustering();

            // update ui
            RaisePropertyChanged(nameof(Image));
            CurrentWorkerTask = "Calculating K-means clustering ... ";

            // Calculate the clustering
            CalculateImageClustering(sender);
        }

        private void CalculateSuperPixels(object sender)
        {
            // easy segmentator initialization :/
            var segmentator = SuperPixelSolver.EmitSegmentor(_originalBitmap, SuperPixelCount, SuperPixelCompactness);

            var error = double.MaxValue;
            while (error > 1.0)
            {
                error = segmentator.Next();

                var progress = (int) Math.Ceiling(100.0 / error);
                (sender as BackgroundWorker).ReportProgress(progress);
            }

            _superPixels = segmentator.EnforceConnectivity();
        }

        private void CalculateImageClustering(object sender)
        {
            // get segmentor
            var segmentator = KMeansSolver.EmitKMeansSegmentator(_originalBitmap, _superPixels, KmeansClusterCount);

            var error = double.MaxValue;
            while (error > 1.0)
            {
                error = segmentator.Next();

                var progress = (int)Math.Ceiling(100.0 / error);
                (sender as BackgroundWorker).ReportProgress(progress);
            }

            // set pixels
            _imagePixels = segmentator.GetPixels();
        }

        private void ResetClustering()
        {
            _clusteredBitmap = new Bitmap(_originalBitmap);
        }

        private void DrawClustering()
        {
            foreach (var imagePixel in _imagePixels)
            {
                _clusteredBitmap.SetPixel(imagePixel.Position.X, imagePixel.Position.Y, imagePixel.ColorRgb.AsColor());
            }
        }

        #endregion
    }
}
