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

        public BarcodeResults ReadBarCode(string path)
        {
            return BarcodeReader.Read(path);
        }

        #endregion
    }
}