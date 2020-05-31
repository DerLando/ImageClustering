using ImageClusteringLibrary.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageClusteringLibrary.Algorithms;
using ImageClusteringLibrary.IO;
using ImageClusteringUI.Commands;
using ImageClusteringUI.Converters;
using Microsoft.Win32;
using Color = System.Drawing.Color;

namespace ImageClusteringUI.ViewModels
{
    public class KMeansClusteringViewModel : ViewModelBase
    {
        //TODO: Open child window for cluster color picking

        #region private backing fields

        private Bitmap _originalBitmap;
        private Bitmap _clusteredBitmap;
        private int _superPixelCount;
        private int _superPixelCompactness;
        private int _kmeansClusterCount;
        private bool _shouldShowStatus;
        private string _currentWorkerTask;
        private int _progressValue;

        private IReadOnlyCollection<SuperPixelData> _superPixels;
        private IReadOnlyCollection<ImagePixel> _imagePixels;
        private IReadOnlyDictionary<Color, List<int>> _clusterColors;
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

        public ObservableCollection<ClusterColorViewModel> ClusterColors { get; set; } =
            new ObservableCollection<ClusterColorViewModel>();


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
            _clusteredBitmap = new Bitmap(400, 300);

            RefreshCommand = new RelayCommand(o => RefreshButtonClick(null));
            SaveCommand = new RelayCommand(o => SaveButtonClick(null));
            SelectImageCommand = new RelayCommand(o => SelectImageButtonClick(null));

            ClusterColors.CollectionChanged += ClusterColorsOnCollectionChanged;
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

        private void SelectImageButtonClick(object sender)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _originalBitmap = new Bitmap(dialog.FileName);
                    _clusteredBitmap = new Bitmap(_originalBitmap);
                    RaisePropertyChanged(nameof(Image));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

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
            UpdateClusterColors();

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
            ShouldShowStatus = true;

            // Calculate the clustering
            CalculateImageClustering(sender);
        }

        private void CalculateSuperPixels(object sender)
        {
            // easy segmentator initialization :/
            var segmentator = SuperPixelSolver.EmitSegmentor(_originalBitmap, SuperPixelCount, SuperPixelCompactness);

            var initialError = double.MaxValue;
            var error = segmentator.Next();
            while (Math.Abs(initialError - error) > 1)
            {
                initialError = error;
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

            var initialError = segmentator.Next();
            var threshold = initialError / 100.0;
            var error = initialError;
            while (error > threshold)
            {
                error = segmentator.Next();

                var progress = (int)Math.Ceiling(100.0 / error);
                (sender as BackgroundWorker).ReportProgress(progress);
            }

            // set pixels
            _imagePixels = segmentator.GetPixels();

            // set cluster colors
            _clusterColors = segmentator.GetSuperPixelColors();
        }

        private void ClusterColorsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    item.PropertyChanged -= ClusterColor_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    item.PropertyChanged += ClusterColor_PropertyChanged;
                }
            }
        }

        private void ClusterColor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // get sender as model
            var model = sender as ClusterColorViewModel;

            // and the color of the model as system.drawing
            var color = Color.FromArgb(model.Color.A, model.Color.R, model.Color.G, model.Color.B);

            // iterate over stored pixel indices
            foreach (var superPixelIndex in model.SuperPixelIndices)
            {
                // get the pixel
                var superPixel = _superPixels.ElementAt(superPixelIndex);

                // iterate over stored pixels
                foreach (var position in superPixel.Positions)
                {
                    // set the new color
                    _clusteredBitmap.SetPixel(position.Vector.X, position.Vector.Y, color);
                }
            }

            RaisePropertyChanged(nameof(Image));
        }

        private void ResetClustering()
        {
            _clusteredBitmap = new Bitmap(_originalBitmap);
        }

        private void UpdateClusterColors()
        {
            ClusterColors.Clear();

            foreach (var model in from color in _clusterColors.Keys
                select new ClusterColorViewModel(
                    System.Windows.Media.Color.FromRgb(color.R, color.G, color.B), _clusterColors[color]))
            {
                ClusterColors.Add(model);
            }

            RaisePropertyChanged(nameof(ClusterColors));
        }

        private void DrawClustering()
        {
            foreach (var imagePixel in _imagePixels)
            {
                _clusteredBitmap.SetPixel(imagePixel.Position.X, imagePixel.Position.Y, imagePixel.ColorRgb.AsColor());
            }

            // draw borders
            // TODO: expose as option to UI
            //foreach (var boundaryPosition in SuperPixelSolver.SolveSuperPixelBoundaries(_superPixels))
            //{
            //    _clusteredBitmap.SetPixel(boundaryPosition.Vector.X, boundaryPosition.Vector.Y, Color.Red);
            //}

        }

        #endregion
    }
}
