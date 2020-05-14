using ImageClusteringLibrary.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ImageClusteringLibrary.Algorithms;
using ImageClusteringUI.Commands;
using ImageClusteringUI.Converters;

namespace ImageClusteringUI.ViewModels
{
    public class KMeansClusteringViewModel : ViewModelBase
    {
        #region private backing fields

        private Bitmap _originalBitmap = new Bitmap("D:\\Desktop\\feininger_yachten.jpg"); // Debug for now
        private Bitmap _clusteredBitmap;
        private int _superPixelCount;
        private int _superPixelCompactness;
        private int _kmeansClusterCount;

        private IReadOnlyCollection<SuperPixelData> _superPixels;

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
                _superPixelCount = value;
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
                _superPixelCompactness = value;
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

        #endregion

        #region Constructor

        public KMeansClusteringViewModel()
        {
            SuperPixelCount = 200;
            SuperPixelCompactness = 10;
            KmeansClusterCount = 6;
            _clusteredBitmap = new Bitmap(_originalBitmap);

            RefreshCommand = new RelayCommand(o => RefreshButtonClick(null));
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command to select an image
        /// </summary>
        public ICommand SelectImageCommand { get; set; }

        public ICommand RefreshCommand { get; set; }

        #endregion

        #region Methods

        private void RefreshButtonClick(object sender)
        {
            if (_superPixels?.Count != SuperPixelCount)
            {
                CalculateSuperPixels();
            }

            CalculateImageClustering();

            RaisePropertyChanged(nameof(Image));
            
        }

        private void CalculateSuperPixels()
        {
            _superPixels = SuperPixelSolver.Solve(_originalBitmap, SuperPixelCount, SuperPixelCompactness);
        }

        private void CalculateImageClustering()
        {
            // reset image
            ResetClustering();

            foreach (var pixel in KMeansSolver.Solve(_originalBitmap, _superPixels, KmeansClusterCount))
            {
                _clusteredBitmap.SetPixel(pixel.Position.X, pixel.Position.Y, pixel.ColorRgb.AsColor());
            }
        }

        private void ResetClustering()
        {
            _clusteredBitmap = new Bitmap(_originalBitmap);
        }

        #endregion
    }
}
