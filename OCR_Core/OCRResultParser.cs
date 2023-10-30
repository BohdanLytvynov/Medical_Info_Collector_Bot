using IronOcr;
using OCR_Core.Dependencies.Interfaces;
using SixLabors.Fonts.Unicode;
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

        readonly Regex m_code;

        readonly Regex m_Name_or_Surename_or_Lastname;

        readonly Regex m_Surename_Name;

        readonly Regex m_Surename_Name_Lastname;

        #endregion

        #region Ctor

        public OCRResultParser(Regex Code, Regex Name_or_Surename_or_Lastname,
            Regex Surename_Name, Regex Surename_Name_Laastname)
        {
            m_code = Code;

            m_Name_or_Surename_or_Lastname = Name_or_Surename_or_Lastname;

            m_Surename_Name = Surename_Name;

            m_Surename_Name_Lastname = Surename_Name_Laastname;
        }

        #endregion

        #region Methods

        public async Task<string[]> ParseAsync(IEnumerable<OcrResult> res)
        {
            string[] r = new string[4];

            await Task.Run(() =>
            {
                //Debug.WriteLine("In Method!!");

                bool CodeCorrect = false;

                bool SNLFound = false;
                                                                
                if (res != null)
                    foreach (var ocrResult in res)
                    {
                        if (ocrResult.Text.Length == 0)
                            continue;

                        #region Old version

                        //var Lines = ocrResult.Lines;

                        //foreach (var item in Lines)
                        //{
                        //    if (!CodeCorrect)
                        //    {
                        //        foreach (var word in item.Words)
                        //        {
                        //            CodeCorrect = m_code.IsMatch(word.Text);

                        //            if (CodeCorrect)// El refferal was found
                        //            {
                        //                code = word.Text;

                        //                break;
                        //            }
                        //        }
                        //    }

                        //    if (!SNFound)
                        //    {
                        //        SNFound = AreWordsValid(item.Words, out snl, 2);
                        //    }

                        //    if (!LastNameFound)
                        //    {
                        //        LastNameFound = AreWordsValid(item.Words, out l, 1);
                        //    }

                        //    if (CodeCorrect && SNFound && LastNameFound)
                        //    {
                        //        break;
                        //    }
                        //}

                        //if (CodeCorrect && SNFound && LastNameFound)
                        //{
                        //    r[0] = snl[0];

                        //    r[1] = snl[1];

                        //    r[2] = l[0];

                        //    r[3] = code;
                        //}

                        #endregion
                        //SNL Found
                        if ((m_Surename_Name_Lastname.IsMatch(ocrResult.Text)
                        || m_Surename_Name.IsMatch(ocrResult.Text))
                        && !SNLFound)
                        {
                            int start  = (ocrResult.Words.Length <= 3) ? ocrResult.Words.Length-1 : 0;

                            //int mod = (start==0)? 0 : 

                            for (; start < 3; start++)
                            {
                                r[start] = ocrResult.Words[start].Text;
                            }

                            SNLFound = true;
                        }
                        else if (m_code.IsMatch(ocrResult.Text) && !CodeCorrect)
                        {
                            r[r.Length - 1] = ocrResult.Text;

                            CodeCorrect = true;
                        }

                        if (SNLFound && CodeCorrect)                        
                            break;                                                                        
                    }

                if (!SNLFound)
                { 
                
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
