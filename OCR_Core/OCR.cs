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

            m_openCvClient = OpenCvClient.Instance;
        }
        #endregion

        #region Methods



        public OcrResult ConvertPhotoToText(string ImgPath)
        {
            Image img = Image.Load(ImgPath);

            OcrInput input = new OcrInput();

            input.AddImage(img);

            input.Binarize();

            input.DeNoise();

            CropRectangle textCropArea = input.Pages[0].FindTextRegion();
           
            return m_tess.Read(ImgPath, textCropArea);                     
        }
        #endregion
    }
}