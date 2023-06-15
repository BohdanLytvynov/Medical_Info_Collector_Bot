using IronOcr;
using OCR_Core.Dependencies.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static IronOcr.OcrResult;

namespace OCR_Core
{
    public class OCRResultParser : IOCRResultParser<string[]>
    {
        #region Fields

        Regex m_code;

        #endregion

        #region Ctor

        public OCRResultParser(Regex Code)
        {
            m_code = Code;
        }

        #endregion

        #region Methods

        public async Task<string[]> ParseAsync(OcrResult res)
        {
            var r = new string[4];

            await Task.Run(() =>
            {                
                //Debug.WriteLine("In Method!!");

                bool CodeCorrect = false;

                bool SNFound = false;

                bool LastNameFound = false;

                string[] snl = null;

                string[] l = null;

                string code = String.Empty;

                if (res != null && res.Lines.Length > 0)
                {
                    var Lines = res.Lines;

                    foreach (var item in Lines)
                    {
                        if (!CodeCorrect)
                        {
                            foreach (var word in item.Words)
                            {
                                CodeCorrect = m_code.IsMatch(word.Text);

                                if (CodeCorrect)// El refferal was found
                                {
                                    code = word.Text;

                                    break;
                                }
                            }
                        }

                        if (!SNFound)
                        {
                            SNFound = AreWordsValid(item.Words, out snl, 2);
                        }

                        if (!LastNameFound)
                        {
                            LastNameFound = AreWordsValid(item.Words, out l, 1);
                        }

                        if (CodeCorrect && SNFound && LastNameFound)
                        {
                            break;
                        }
                    }

                    if (CodeCorrect && SNFound && LastNameFound)
                    {
                        r[0] = snl[0];

                        r[1] = snl[1];

                        r[2] = l[0];

                        r[3] = code;
                    }
                }
                
            });

            return r;
        }

        private bool AreWordsValid(Word[] words, out string[] snl, int Count)
        {
            snl = null;

            if (words != null)
            {
                int l = words.Length;

                if (!(l == Count))
                {
                    return false;
                }

                snl = new string[words.Length];

                for (int i = 0; i < l; i++)
                {
                    var Word = words[i];

                    if (!Char.IsLetter(Word.Text[0]))
                    {
                        return false;
                    }

                    if (!Char.IsUpper(Word.Text[0])) //first letter of each word is in Upper Case
                    {
                        return false;
                    }
                    else
                    {
                        int charCount = Word.Text.Length;

                        for (int j = 1; j < charCount; j++)
                        {
                            if (Char.IsLetter(Word.Text[j]) && Char.IsUpper(Word.Text[j]))
                            {
                                return false;
                            }
                        }
                    }

                    snl[i] = Word.Text;
                }
            }

            return true;
        }

        #endregion



    }
}
