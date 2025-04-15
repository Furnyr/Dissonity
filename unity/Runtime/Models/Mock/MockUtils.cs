
using System;

namespace Dissonity.Models.Mock
{
    public static class MockUtils
    {
        public static string ToLocaleString(MockLocale mocklocale)
        {
            switch (mocklocale)
            {
                case MockLocale.id:
                    return Locale.Indonesian;

                case MockLocale.da:
                    return Locale.Danish;

                case MockLocale.de:
                    return Locale.German;

                case MockLocale.en_GB:
                    return Locale.UkEnglish;

                case MockLocale.en_US:
                    return Locale.UsEnglish;

                case MockLocale.es_ES:
                    return Locale.SpainSpanish;

                case MockLocale.es_419:
                    return Locale.LatamSpanish;

                case MockLocale.fr:
                    return Locale.French;

                case MockLocale.hr:
                    return Locale.Croatian;

                case MockLocale.it:
                    return Locale.Italian;

                case MockLocale.lt:
                    return Locale.Lithuanian;

                case MockLocale.hu:
                    return Locale.Hungarian;

                case MockLocale.nl:
                    return Locale.Dutch;

                case MockLocale.no:
                    return Locale.Norwegian;

                case MockLocale.pl:
                    return Locale.Polish;

                case MockLocale.pt_BR:
                    return Locale.Portuguese;

                case MockLocale.ro:
                    return Locale.Romanian;

                case MockLocale.fi:
                    return Locale.Finnish;

                case MockLocale.sv_SE:
                    return Locale.Swedish;

                case MockLocale.vi:
                    return Locale.Vietnamese;

                case MockLocale.tr:
                    return Locale.Turkish;

                case MockLocale.cs:
                    return Locale.Czech;

                case MockLocale.el:
                    return Locale.Greek;

                case MockLocale.bg:
                    return Locale.Bulgarian;

                case MockLocale.ru:
                    return Locale.Russian;

                case MockLocale.uk:
                    return Locale.Ukrainian;

                case MockLocale.hi:
                    return Locale.Hindi;

                case MockLocale.th:
                    return Locale.Thai;

                case MockLocale.zh_CN:
                    return Locale.ChinaChinese;

                case MockLocale.ja:
                    return Locale.Japanese;

                case MockLocale.zh_TW:
                    return Locale.TaiwanChinese;

                case MockLocale.ko:
                    return Locale.Korean;

                default:
                    throw new Exception("Unknown MockLocale");
            }
        }
    
        public static string ToPlatformString(MockPlatform mockPlatform)
        {
            switch (mockPlatform)
            {
                case MockPlatform.Desktop:
                    return Platform.Desktop;

                case MockPlatform.Mobile:
                    return Platform.Mobile;

                default:
                    throw new Exception("Unknown MockPlatform");
            }
        }
    
    }
}
