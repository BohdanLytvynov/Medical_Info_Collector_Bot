using IronOcr;
using IronSoftware.Drawing;
using SixLabors.ImageSharp;
using System.Diagnostics;

namespace OCR_Core
{
    public class OCR
    {
        #region Fields
        IronTesseract m_tess;

        Random m_random;

        OpenCvClient m_openCvClient;
        #endregion

        #region Properties

        #endregion

        #region Ctor
        public OCR()
        {
            m_random = new Random();

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



        public async Task<List<OcrResult>> ConvertPhotoToTextAsync(string ImgPath)
        {                       
            Image img = Image.Load(ImgPath);
                          
            var regions = m_openCvClient.FindTextRegions(img, 1, 1, false, false);

            List<OcrResult> result = new List<OcrResult>(); 

            foreach (var region in regions)
            {                
                var r = await m_tess.ReadAsync(img, region);

                result.Add(r);

                Debug.WriteLine(r.Text);
            }
                       
            return result;
        }
        #endregion
    }
}