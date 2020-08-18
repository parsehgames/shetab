#if SX_PARSI
using System.Collections.Generic;
using System.Text;
namespace SeganX
{
    public static class PersianTextShaper
    {
        private class CharacterInfo
        {
            public char Isolated;

            public char Final;

            public char Initial;

            public char Medial;

            public bool bJoinsNext = true;

            public bool bJoinsPrevious = true;

            public bool bRightToLeftOrdering = true;
        }

        private enum CharType
        {
            LTR,
            RTL,
            Neutral,
            Space
        }

        private static Dictionary<char, CharacterInfo> CharInfo;

        private static HashSet<char> NeutralCharacters;

        private static Dictionary<char, char> RTLSwappedCharacters;

        static PersianTextShaper()
        {
            CharInfo = new Dictionary<char, CharacterInfo>();
            NeutralCharacters = new HashSet<char>();
            RTLSwappedCharacters = new Dictionary<char, char>();
            CharInfo.Add('ا', new CharacterInfo
            {
                Isolated = 'ﺍ',
                Final = 'ﺎ',
                Initial = 'ﺍ',
                Medial = 'ﺎ',
                bJoinsNext = false
            });
            CharInfo.Add('آ', new CharacterInfo
            {
                Isolated = 'ﺁ',
                Final = 'ﺂ',
                Initial = 'ﺁ',
                Medial = 'ﺂ',
                bJoinsNext = false
            });
            CharInfo.Add('ب', new CharacterInfo
            {
                Isolated = 'ﺏ',
                Final = 'ﺐ',
                Initial = 'ﺑ',
                Medial = 'ﺒ'
            });
            CharInfo.Add('پ', new CharacterInfo
            {
                Isolated = 'ﭖ',
                Final = 'ﭗ',
                Initial = 'ﭘ',
                Medial = 'ﭙ'
            });
            CharInfo.Add('ت', new CharacterInfo
            {
                Isolated = 'ﺕ',
                Final = 'ﺖ',
                Initial = 'ﺗ',
                Medial = 'ﺘ'
            });
            CharInfo.Add('ث', new CharacterInfo
            {
                Isolated = 'ﺙ',
                Final = 'ﺚ',
                Initial = 'ﺛ',
                Medial = 'ﺜ'
            });
            CharInfo.Add('ج', new CharacterInfo
            {
                Isolated = 'ﺝ',
                Final = 'ﺞ',
                Initial = 'ﺟ',
                Medial = 'ﺠ'
            });
            CharInfo.Add('چ', new CharacterInfo
            {
                Isolated = 'ﭺ',
                Final = 'ﭻ',
                Initial = 'ﭼ',
                Medial = 'ﭽ'
            });
            CharInfo.Add('ح', new CharacterInfo
            {
                Isolated = 'ﺡ',
                Final = 'ﺢ',
                Initial = 'ﺣ',
                Medial = 'ﺤ'
            });
            CharInfo.Add('خ', new CharacterInfo
            {
                Isolated = 'ﺥ',
                Final = 'ﺦ',
                Initial = 'ﺧ',
                Medial = 'ﺨ'
            });
            CharInfo.Add('د', new CharacterInfo
            {
                Isolated = 'ﺩ',
                Final = 'ﺪ',
                Initial = 'ﺩ',
                Medial = 'ﺪ',
                bJoinsNext = false
            });
            CharInfo.Add('ذ', new CharacterInfo
            {
                Isolated = 'ﺫ',
                Final = 'ﺬ',
                Initial = 'ﺫ',
                Medial = 'ﺬ',
                bJoinsNext = false
            });
            CharInfo.Add('ر', new CharacterInfo
            {
                Isolated = 'ﺭ',
                Final = 'ﺮ',
                Initial = 'ﺭ',
                Medial = 'ﺮ',
                bJoinsNext = false
            });
            CharInfo.Add('ز', new CharacterInfo
            {
                Isolated = 'ﺯ',
                Final = 'ﺰ',
                Initial = 'ﺯ',
                Medial = 'ﺰ',
                bJoinsNext = false
            });
            CharInfo.Add('ژ', new CharacterInfo
            {
                Isolated = 'ﮊ',
                Final = 'ﮋ',
                Initial = 'ﮊ',
                Medial = 'ﮋ',
                bJoinsNext = false
            });
            CharInfo.Add('س', new CharacterInfo
            {
                Isolated = 'ﺱ',
                Final = 'ﺲ',
                Initial = 'ﺳ',
                Medial = 'ﺴ'
            });
            CharInfo.Add('ش', new CharacterInfo
            {
                Isolated = 'ﺵ',
                Final = 'ﺶ',
                Initial = 'ﺷ',
                Medial = 'ﺸ'
            });
            CharInfo.Add('ص', new CharacterInfo
            {
                Isolated = 'ﺹ',
                Final = 'ﺺ',
                Initial = 'ﺻ',
                Medial = 'ﺼ'
            });
            CharInfo.Add('ض', new CharacterInfo
            {
                Isolated = 'ﺽ',
                Final = 'ﺾ',
                Initial = 'ﺿ',
                Medial = 'ﻀ'
            });
            CharInfo.Add('ط', new CharacterInfo
            {
                Isolated = 'ﻁ',
                Final = 'ﻂ',
                Initial = 'ﻃ',
                Medial = 'ﻄ'
            });
            CharInfo.Add('ظ', new CharacterInfo
            {
                Isolated = 'ﻅ',
                Final = 'ﻆ',
                Initial = 'ﻇ',
                Medial = 'ﻈ'
            });
            CharInfo.Add('ع', new CharacterInfo
            {
                Isolated = 'ﻉ',
                Final = 'ﻊ',
                Initial = 'ﻋ',
                Medial = 'ﻌ'
            });
            CharInfo.Add('غ', new CharacterInfo
            {
                Isolated = 'ﻍ',
                Final = 'ﻎ',
                Initial = 'ﻏ',
                Medial = 'ﻐ'
            });
            CharInfo.Add('ف', new CharacterInfo
            {
                Isolated = 'ﻑ',
                Final = 'ﻒ',
                Initial = 'ﻓ',
                Medial = 'ﻔ'
            });
            CharInfo.Add('ق', new CharacterInfo
            {
                Isolated = 'ﻕ',
                Final = 'ﻖ',
                Initial = 'ﻗ',
                Medial = 'ﻘ'
            });
            CharInfo.Add('ک', new CharacterInfo
            {
                Isolated = 'ﮎ',
                Final = 'ﮏ',
                Initial = 'ﮐ',
                Medial = 'ﮑ'
            });
            CharInfo.Add('گ', new CharacterInfo
            {
                Isolated = 'ﮒ',
                Final = 'ﮓ',
                Initial = 'ﮔ',
                Medial = 'ﮕ'
            });
            CharInfo.Add('ل', new CharacterInfo
            {
                Isolated = 'ﻝ',
                Final = 'ﻞ',
                Initial = 'ﻟ',
                Medial = 'ﻠ'
            });
            CharInfo.Add('م', new CharacterInfo
            {
                Isolated = 'ﻡ',
                Final = 'ﻢ',
                Initial = 'ﻣ',
                Medial = 'ﻤ'
            });
            CharInfo.Add('ن', new CharacterInfo
            {
                Isolated = 'ﻥ',
                Final = 'ﻦ',
                Initial = 'ﻧ',
                Medial = 'ﻨ'
            });
            CharInfo.Add('و', new CharacterInfo
            {
                Isolated = 'ﻭ',
                Final = 'ﻮ',
                Initial = 'ﻭ',
                Medial = 'ﻮ',
                bJoinsNext = false
            });
            CharInfo.Add('ه', new CharacterInfo
            {
                Isolated = 'ﻩ',
                Final = 'ﻪ',
                Initial = 'ﻫ',
                Medial = 'ﻬ'
            });
            CharInfo.Add('ی', new CharacterInfo
            {
                Isolated = 'ﯼ',
                Final = 'ﯽ',
                Initial = 'ﯾ',
                Medial = 'ﯿ'
            });
            CharInfo.Add('ئ', new CharacterInfo
            {
                Isolated = 'ﺉ',
                Final = 'ﺊ',
                Initial = 'ﺋ',
                Medial = 'ﺌ'
            });
            CharInfo.Add('ء', new CharacterInfo
            {
                Isolated = 'ء',
                Final = 'ء',
                Initial = 'ء',
                Medial = 'ء',
                bJoinsNext = false,
                bJoinsPrevious = false
            });
            CharInfo.Add('ﻻ', new CharacterInfo
            {
                Isolated = 'ﻻ',
                Final = 'ﻼ',
                Initial = 'ﻻ',
                Medial = 'ﻼ',
                bJoinsNext = false
            });
            CharInfo.Add('0', new CharacterInfo
            {
                Isolated = '٠',
                Final = '٠',
                Initial = '٠',
                Medial = '٠',
                bJoinsNext = false,
                bJoinsPrevious = false,
                bRightToLeftOrdering = false
            });
            CharInfo.Add('1', new CharacterInfo
            {
                Isolated = '١',
                Final = '١',
                Initial = '١',
                Medial = '١',
                bJoinsNext = false,
                bJoinsPrevious = false,
                bRightToLeftOrdering = false
            });
            CharInfo.Add('2', new CharacterInfo
            {
                Isolated = '٢',
                Final = '٢',
                Initial = '٢',
                Medial = '٢',
                bJoinsNext = false,
                bJoinsPrevious = false,
                bRightToLeftOrdering = false
            });
            CharInfo.Add('3', new CharacterInfo
            {
                Isolated = '٣',
                Final = '٣',
                Initial = '٣',
                Medial = '٣',
                bJoinsNext = false,
                bJoinsPrevious = false,
                bRightToLeftOrdering = false
            });
            CharInfo.Add('4', new CharacterInfo
            {
                Isolated = '٤',
                Final = '٤',
                Initial = '٤',
                Medial = '٤',
                bJoinsNext = false,
                bJoinsPrevious = false,
                bRightToLeftOrdering = false
            });
            CharInfo.Add('5', new CharacterInfo
            {
                Isolated = '٥',
                Final = '٥',
                Initial = '٥',
                Medial = '٥',
                bJoinsNext = false,
                bJoinsPrevious = false,
                bRightToLeftOrdering = false
            });
            CharInfo.Add('6', new CharacterInfo
            {
                Isolated = '٦',
                Final = '٦',
                Initial = '٦',
                Medial = '٦',
                bJoinsNext = false,
                bJoinsPrevious = false,
                bRightToLeftOrdering = false
            });
            CharInfo.Add('7', new CharacterInfo
            {
                Isolated = '٧',
                Final = '٧',
                Initial = '٧',
                Medial = '٧',
                bJoinsNext = false,
                bJoinsPrevious = false,
                bRightToLeftOrdering = false
            });
            CharInfo.Add('8', new CharacterInfo
            {
                Isolated = '٨',
                Final = '٨',
                Initial = '٨',
                Medial = '٨',
                bJoinsNext = false,
                bJoinsPrevious = false,
                bRightToLeftOrdering = false
            });
            CharInfo.Add('9', new CharacterInfo
            {
                Isolated = '٩',
                Final = '٩',
                Initial = '٩',
                Medial = '٩',
                bJoinsNext = false,
                bJoinsPrevious = false,
                bRightToLeftOrdering = false
            });
            CharInfo.Add('،', new CharacterInfo
            {
                Isolated = '،',
                Final = '،',
                Initial = '،',
                Medial = '،',
                bJoinsNext = false,
                bJoinsPrevious = false
            });
            CharInfo.Add('؛', new CharacterInfo
            {
                Isolated = '؛',
                Final = '؛',
                Initial = '؛',
                Medial = '؛',
                bJoinsNext = false,
                bJoinsPrevious = false
            });
            NeutralCharacters.Add(' ');
            NeutralCharacters.Add('.');
            NeutralCharacters.Add(':');
            NeutralCharacters.Add(';');
            NeutralCharacters.Add('\'');
            NeutralCharacters.Add('"');
            NeutralCharacters.Add('+');
            NeutralCharacters.Add('-');
            NeutralCharacters.Add('*');
            NeutralCharacters.Add('/');
            NeutralCharacters.Add('\\');
            NeutralCharacters.Add('\n');
            RTLSwappedCharacters['('] = ')';
            RTLSwappedCharacters[')'] = '(';
            RTLSwappedCharacters['['] = ']';
            RTLSwappedCharacters[']'] = '[';
            RTLSwappedCharacters['{'] = '}';
            RTLSwappedCharacters['}'] = '{';
            RTLSwappedCharacters['<'] = '>';
            RTLSwappedCharacters['>'] = '<';
            RTLSwappedCharacters['«'] = '»';
            RTLSwappedCharacters['»'] = '«';
        }

        private static bool JoinsNext(char c)
        {
            if (CharInfo.ContainsKey(c))
            {
                return CharInfo[c].bJoinsNext;
            }
            return false;
        }

        private static bool JoinsPrevious(char c)
        {
            if (CharInfo.ContainsKey(c))
            {
                return CharInfo[c].bJoinsPrevious;
            }
            return false;
        }

        private static bool IsRightToLeftOrdered(char c, bool bTextRightToLeft)
        {
            if (RTLSwappedCharacters.ContainsKey(c))
            {
                return bTextRightToLeft;
            }
            if (CharInfo.ContainsKey(c))
            {
                return CharInfo[c].bRightToLeftOrdering;
            }
            return false;
        }

        private static bool IsNeutral(char c)
        {
            return NeutralCharacters.Contains(c);
        }

        private static CharType GetCharType(char c, bool bTextRightToLeft)
        {
            if (c != ' ')
            {
                if (!IsNeutral(c))
                {
                    if (!IsRightToLeftOrdered(c, bTextRightToLeft))
                    {
                        return CharType.LTR;
                    }
                    return CharType.RTL;
                }
                return CharType.Neutral;
            }
            return CharType.Space;
        }

        private static bool CharTypesCompatibleForToken(CharType CT1, CharType CT2)
        {
            if (CT1 != CT2 && (CT1 != CharType.Neutral || CT2 == CharType.Space))
            {
                if (CT2 == CharType.Neutral)
                {
                    return CT1 != CharType.Space;
                }
                return false;
            }
            return true;
        }

        private static bool CharTypesCompatibleForRun(CharType CT1, CharType CT2)
        {
            if (CT1 == CharType.Space)
            {
                return CT2 == CharType.Space;
            }
            if (CT1 != CT2 && CT1 != CharType.Neutral && CT2 != CharType.Neutral)
            {
                return CT2 == CharType.Space;
            }
            return true;
        }

        private static char GetGlyph(char C, char Prev, char Next, bool bRightToLeft)
        {
            if (CharInfo.ContainsKey(C))
            {
                bool flag = JoinsNext(Prev);
                bool flag2 = JoinsPrevious(Next);
                if (flag)
                {
                    if (flag2)
                    {
                        return CharInfo[C].Medial;
                    }
                    return CharInfo[C].Final;
                }
                if (flag2)
                {
                    return CharInfo[C].Initial;
                }
                return CharInfo[C].Isolated;
            }
            if (bRightToLeft && RTLSwappedCharacters.ContainsKey(C))
            {
                return RTLSwappedCharacters[C];
            }
            return C;
        }

        private static string ShapeTokenPersian(string Token, bool bRightToLeft)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Token = '\0' + Token + '\0';
            for (int i = 1; i < Token.Length - 1; i++)
            {
                if (Token[i] == 'ل' && Token[i + 1] == 'ا')
                {
                    if (bRightToLeft)
                    {
                        stringBuilder.Insert(0, GetGlyph('ﻻ', Token[i - 1], Token[i + 2], bRightToLeft));
                    }
                    else
                    {
                        stringBuilder.Append(GetGlyph('ﻻ', Token[i - 1], Token[i + 2], bRightToLeft));
                    }
                    i++;
                }
                else if (bRightToLeft)
                {
                    stringBuilder.Insert(0, GetGlyph(Token[i], Token[i - 1], Token[i + 1], bRightToLeft));
                }
                else
                {
                    stringBuilder.Append(GetGlyph(Token[i], Token[i - 1], Token[i + 1], bRightToLeft));
                }
            }
            return stringBuilder.ToString();
        }

        private static string GetNextToken(ref string Text, ref int ScanIdx, bool bTextRightToLeft, out CharType CharType)
        {
            StringBuilder stringBuilder = new StringBuilder();
            CharType = GetCharType(Text[ScanIdx], bTextRightToLeft);
            CharType charType;
            while (ScanIdx < Text.Length && CharTypesCompatibleForToken(CharType, charType = GetCharType(Text[ScanIdx], bTextRightToLeft)))
            {
                if (CharType == CharType.Neutral && charType != CharType.Neutral && charType != CharType.Space)
                {
                    CharType = charType;
                }
                stringBuilder.Append(Text[ScanIdx]);
                ScanIdx++;
            }
            if (CharType == CharType.Neutral)
            {
                CharType = (bTextRightToLeft ? CharType.RTL : CharType.LTR);
            }
            return stringBuilder.ToString();
        }

        private static string ShapeRun(string Text, bool bRightToLeft)
        {
            List<string> list = new List<string>();
            List<CharType> list2 = new List<CharType>();
            int ScanIdx = 0;
            while (ScanIdx < Text.Length)
            {
                CharType CharType;
                string nextToken = GetNextToken(ref Text, ref ScanIdx, bRightToLeft, out CharType);
                list.Add(nextToken);
                list2.Add(CharType);
            }
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = ShapeTokenPersian(list[i], list2[i] == CharType.RTL);
            }
            StringBuilder stringBuilder = new StringBuilder();
            if (bRightToLeft)
            {
                for (int num = list.Count - 1; num >= 0; num--)
                {
                    stringBuilder.Append(list[num]);
                }
            }
            else
            {
                for (int j = 0; j < list.Count; j++)
                {
                    stringBuilder.Append(list[j]);
                }
            }
            return stringBuilder.ToString();
        }

        private static string GetNextRun(ref string Text, ref int ScanIdx, bool bTextRightToLeft, out bool bRunRightToLeft)
        {
            StringBuilder stringBuilder = new StringBuilder();
            CharType charType = GetCharType(Text[ScanIdx], bTextRightToLeft);
            CharType charType2;
            while (ScanIdx < Text.Length && CharTypesCompatibleForRun(charType, charType2 = GetCharType(Text[ScanIdx], bTextRightToLeft)))
            {
                if (charType == CharType.Neutral && charType2 != CharType.Neutral && charType2 != CharType.Space)
                {
                    charType = charType2;
                }
                stringBuilder.Append(Text[ScanIdx]);
                ScanIdx++;
            }
            switch (charType)
            {
                case CharType.Neutral:
                case CharType.Space:
                    bRunRightToLeft = bTextRightToLeft;
                    break;
                case CharType.RTL:
                    bRunRightToLeft = true;
                    break;
                default:
                    bRunRightToLeft = false;
                    break;
            }
            if (charType != CharType.Neutral)
            {
                while (GetCharType(stringBuilder[stringBuilder.Length - 1], bTextRightToLeft) == CharType.Neutral)
                {
                    stringBuilder.Remove(stringBuilder.Length - 1, 1);
                    ScanIdx--;
                }
            }
            if (charType != CharType.Space)
            {
                while (GetCharType(stringBuilder[stringBuilder.Length - 1], bTextRightToLeft) == CharType.Space)
                {
                    stringBuilder.Remove(stringBuilder.Length - 1, 1);
                    ScanIdx--;
                }
            }
            return stringBuilder.ToString();
        }

        private static string ShapeLine(string Text, bool bRightToLeft)
        {
            List<string> list = new List<string>();
            List<bool> list2 = new List<bool>();
            int ScanIdx = 0;
            while (ScanIdx < Text.Length)
            {
                bool bRunRightToLeft;
                string nextRun = GetNextRun(ref Text, ref ScanIdx, bRightToLeft, out bRunRightToLeft);
                list.Add(nextRun);
                list2.Add(bRunRightToLeft);
            }
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = ShapeRun(list[i], list2[i]);
            }
            StringBuilder stringBuilder = new StringBuilder();
            if (bRightToLeft)
            {
                for (int num = list.Count - 1; num >= 0; num--)
                {
                    stringBuilder.Append(list[num]);
                }
            }
            else
            {
                for (int j = 0; j < list.Count; j++)
                {
                    stringBuilder.Append(list[j]);
                }
            }
            return stringBuilder.ToString();
        }

        public static string ShapeText(string Text, bool bRightToLeft = true)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string[] array = Text.Split('\n');
            for (int i = 0; i < array.Length; i++)
            {
                stringBuilder.Append(ShapeLine(array[i], bRightToLeft));
                if (i < array.Length - 1)
                {
                    stringBuilder.Append('\n');
                }
            }
            return stringBuilder.ToString();
        }
    }
}
#endif