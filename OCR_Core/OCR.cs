using IronOcr;
using IronSoftware.Drawing;
using SixLabors.ImageSharp;


namespace OCR_Core
{
    public class OCR
    {
        #region Fields
        IronTesseract m_tess;

        OpenCvClient m_openCvClient;
        #endregion

        #region Properties

        #endregion

        #region Ctor
        public OCR()
        {
            m_tess = new IronTesseract();

            m_tess.Language = OcrLanguage.UkrainianBest;

            var configuration = new TesseractConfiguration()
            {
                ReadBarCodes = false,
                BlackListCharacters = "`ë|^",
                RenderSearchablePdfsAndHocr = true,
                PageSegmentationMode = TesseractPageSegmentationMode.AutoOsd,
            };

            m_tess.Configuration = configuration;

            m_openCvClient = OpenCvClient.Instance;
        }
        #endregion

        #region Methods



        public async Task<OcrResult> ConvertPhotoToTextAsync(string ImgPath)
        {            
            Image img = Image.Load(ImgPath);

            OcrInput input = new OcrInput();

            input.AddImage(img);

            input.Binarize();

            input.DeNoise();

            input.Deskew();
            
            CropRectangle textCropArea = input.Pages[0].FindTextRegion();
           
            var r=  await m_tess.ReadAsync(ImgPath, textCropArea);

            input.Dispose();

            return r;
        }
        #endregion
    }
}