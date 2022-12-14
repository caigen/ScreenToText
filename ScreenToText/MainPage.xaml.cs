using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ScreenToText
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public async Task ClipboardOCR()
        {
            DataPackageView dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Bitmap))
            {
                IRandomAccessStreamReference imageReceived = null;
                imageReceived = await dataPackageView.GetBitmapAsync();

                if (imageReceived != null)
                {
                    using (var imageStream = await imageReceived.OpenReadAsync())
                    {
                        var bitmapImage = new BitmapImage();
                        bitmapImage.SetSource(imageStream);
                        this.CapturedImage.Source = bitmapImage;

                        // decode
                        var decoder = await BitmapDecoder.CreateAsync(imageStream);
                        SoftwareBitmap bitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);

                        // OCR
                        OcrEngine ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
                        if (ocrEngine != null)
                        {
                            // Recognize text from image.
                            var ocrResult = await ocrEngine.RecognizeAsync(bitmap);

                            // Display recognized text.
                            this.CapturedText.Text = ocrResult.Text;
                        }
                    }
                }
            }
        }

        private async void Page_GotFocus(object sender, RoutedEventArgs e)
        {
            await ClipboardOCR();
        }
    }
}
