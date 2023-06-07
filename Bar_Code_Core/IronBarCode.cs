using IronBarCode;

namespace Bar_Code_Core
{
    public class IronBarCode
    {
        #region Fields
        
        #endregion

        #region Ctor

        public IronBarCode()
        {
            
        }

        #endregion

        #region Methods

        public BarcodeResult ReadBarCode(string path)
        {
            return BarcodeReader.ReadASingleBarcode(path, 
                BarcodeEncoding.AllOneDimensional, 
                BarcodeReader.BarcodeRotationCorrection.Medium, 
                BarcodeReader.BarcodeImageCorrection.DeepCleanPixels);
        }

        #endregion
    }
}