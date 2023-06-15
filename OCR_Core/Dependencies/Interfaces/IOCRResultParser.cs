using IronOcr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_Core.Dependencies.Interfaces
{
    public interface IOCRResultParser<Tout>
    {
        Task<Tout> ParseAsync(OcrResult res);
    }
}
