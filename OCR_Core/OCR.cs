using IronOcr;

namespace OCR_Core
{
    public class OCR
    {
        #region Fields
        IronTesseract m_tess;
        #endregion

        #region Properties

        #endregion

        #region Ctor
        public OCR()
        {
            m_tess = new IronTesseract();

            m_tess.Language = OcrLanguage.UkrainianBest;                        
        }
        #endregion

        #region Methods
        public OcrResult ConvertPhotoToText(string ImgPath)
        {
            return m_tess.Read(ImgPath);
        }
        #endregion
    }
}