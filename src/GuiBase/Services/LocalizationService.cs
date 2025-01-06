using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utillity.File;
using Utillity.Data;
using DataServer;
using DataServer.Log;

namespace GuiBase.Services
{
    public class LocalizationService : ILocalizationService
    {
        private Dictionary<string, Dictionary<string, string>> _languageMapping;
        private Dictionary<string, string> _isoLanguageCodes;
        private readonly string _folderPath = "../../../../conf/translationfiles";
        private ILog _log;
        private string _taskName = "Localization Service";
        public event LanguageChangedEventHandler LanguageChanged;

        public string this[string key] => Translate(key);

        private string _language;
        public string Language
        {
            get
            {
                return _language;
            }
            set
            {
                if (_language != value)
                {
                    if (_isoLanguageCodes.ContainsKey(value))
                    {
                        _language = value;
                        LanguageChanged?.Invoke(new LanguageChangedEvent(_language));
                    }
                }
            }
        }

        public IDictionary<string, string> Languages => _isoLanguageCodes;


        public LocalizationService(ILog log)
        {
            _languageMapping = new Dictionary<string, Dictionary<string, string>>();
            _log = log;

        }
        public bool Init()
        {
            _isoLanguageCodes = new Dictionary<string, string>
        {
            {"aa", "Afar"},
            {"ab", "Abkhaz"},
            {"ae", "Avestan"},
            {"af", "Afrikaans"},
            {"ak", "Akan"},
            {"am", "Amharic"},
            {"an", "Aragonese"},
            {"ar", "Arabic"},
            {"as", "Assamese"},
            {"av", "Avaric"},
            {"ay", "Aymara"},
            {"az", "Azerbaijani"},
            {"ba", "Bashkir"},
            {"be", "Belarusian"},
            {"bg", "Bulgarian"},
            {"bh", "Bihari"},
            {"bi", "Bislama"},
            {"bm", "Bambara"},
            {"bn", "Bengali"},
            {"bo", "Tibetan"},
            {"br", "Breton"},
            {"bs", "Bosnian"},
            {"ca", "Catalan"},
            {"ce", "Chechen"},
            {"ch", "Chamorro"},
            {"co", "Corsican"},
            {"cr", "Cree"},
            {"cs", "Czech"},
            {"cu", "Old Church Slavonic"},
            {"cv", "Chuvash"},
            {"cy", "Welsh"},
            {"da", "Danish"},
            {"de", "German"},
            {"dv", "Divehi"},
            {"dz", "Dzongkha"},
            {"ee", "Ewe"},
            {"el", "Greek"},
            {"en", "English"},
            {"eo", "Esperanto"},
            {"es", "Spanish"},
            {"et", "Estonian"},
            {"eu", "Basque"},
            {"fa", "Persian"},
            {"ff", "Fula"},
            {"fi", "Finnish"},
            {"fj", "Fijian"},
            {"fo", "Faroese"},
            {"fr", "French"},
            {"fy", "Western Frisian"},
            {"ga", "Irish"},
            {"gd", "Scottish Gaelic"},
            {"gl", "Galician"},
            {"gn", "Guarani"},
            {"gu", "Gujarati"},
            {"gv", "Manx"},
            {"ha", "Hausa"},
            {"he", "Hebrew"},
            {"hi", "Hindi"},
            {"ho", "Hiri Motu"},
            {"hr", "Croatian"},
            {"ht", "Haitian"},
            {"hu", "Hungarian"},
            {"hy", "Armenian"},
            {"hz", "Herero"},
            {"ia", "Interlingua"},
            {"id", "Indonesian"},
            {"ie", "Interlingue"},
            {"ig", "Igbo"},
            {"ii", "Nuosu"},
            {"ik", "Inupiaq"},
            {"io", "Ido"},
            {"is", "Icelandic"},
            {"it", "Italian"},
            {"iu", "Inuktitut"},
            {"ja", "Japanese"},
            {"jv", "Javanese"},
            {"ka", "Georgian"},
            {"kg", "Kongo"},
            {"ki", "Kikuyu"},
            {"kj", "Kwanyama"},
            {"kk", "Kazakh"},
            {"kl", "Kalaallisut"},
            {"km", "Khmer"},
            {"kn", "Kannada"},
            {"ko", "Korean"},
            {"kr", "Kanuri"},
            {"ks", "Kashmiri"},
            {"ku", "Kurdish"},
            {"kv", "Komi"},
            {"kw", "Cornish"},
            {"ky", "Kyrgyz"},
            {"la", "Latin"},
            {"lb", "Luxembourgish"},
            {"lg", "Ganda"},
            {"li", "Limburgish"},
            {"ln", "Lingala"},
            {"lo", "Lao"},
            {"lt", "Lithuanian"},
            {"lu", "Luba-Katanga"},
            {"lv", "Latvian"},
            {"mg", "Malagasy"},
            {"mh", "Marshallese"},
            {"mi", "Māori"},
            {"mk", "Macedonian"},
            {"ml", "Malayalam"},
            {"mn", "Mongolian"},
            {"mr", "Marathi"},
            {"ms", "Malay"},
            {"mt", "Maltese"},
            {"my", "Burmese"},
            {"na", "Nauru"},
            {"nb", "Norwegian Bokmål"},
            {"nd", "North Ndebele"},
            {"ne", "Nepali"},
            {"ng", "Ndonga"},
            {"nl", "Dutch"},
            {"nn", "Norwegian Nynorsk"},
            {"no", "Norwegian"},
            {"nr", "South Ndebele"},
            {"nv", "Navajo"},
            {"ny", "Nyanja"},
            {"oc", "Occitan"},
            {"oj", "Ojibwe"},
            {"om", "Oromo"},
            {"or", "Odia"},
            {"os", "Ossetian"},
            {"pa", "Panjabi"},
            {"pi", "Pāli"},
            {"pl", "Polish"},
            {"ps", "Pashto"},
            {"pt", "Portuguese"},
            {"qu", "Quechua"},
            {"rm", "Romansh"},
            {"rn", "Kirundi"},
            {"ro", "Romanian"},
            {"ru", "Russian"},
            {"rw", "Kinyarwanda"},
            {"sa", "Sanskrit"},
            {"sc", "Sardinian"},
            {"sd", "Sindhi"},
            {"se", "Northern Sami"},
            {"sg", "Sango"},
            {"si", "Sinhala"},
            {"sk", "Slovak"},
            {"sl", "Slovenian"},
            {"sm", "Samoan"},
            {"sn", "Shona"},
            {"so", "Somali"},
            {"sq", "Albanian"},
            {"sr", "Serbian"},
            {"ss", "Swati"},
            {"st", "Southern Sotho"},
            {"su", "Sundanese"},
            {"sv", "Swedish"},
            {"sw", "Swahili"},
            {"ta", "Tamil"},
            {"te", "Telugu"},
            {"tg", "Tajik"},
            {"th", "Thai"},
            {"ti", "Tigrinya"},
            {"tk", "Turkmen"},
            {"tl", "Tagalog"},
            {"tn", "Tswana"},
            {"to", "Tongan"},
            {"tr", "Turkish"},
            {"ts", "Tsonga"},
            {"tt", "Tatar"},
            {"tw", "Twice"},
            {"ty", "Tahitian"},
            {"ug", "Uyghur"},
            {"uk", "Ukrainian"},
            {"ur", "Urdu"},
            {"uz", "Uzbek"},
            {"ve", "Venda"},
            {"vi", "Vietnamese"},
            {"vo", "Volapük"},
            {"vun", "Vunjo"},
            {"wa", "Walloon"},
            {"wo","Wolof" },
            {"xh","Xhosa" },
            {"yi","Yiddish" },
            {"yo","Yoruba" },
            {"za","Zhuang" },
            {"zh","Chinese" },
            {"zu","Zulu" },

           };
            _language = "zh";
            initLanguageMapping();
            return true;
        }

        private bool initLanguageMapping()
        {
            
            _languageMapping = new Dictionary<string, Dictionary<string, string>>();
            try
            {
                var filePaths = FileDialog.GetFilePaths(_folderPath);


                foreach (var filePath in filePaths)
                {
                    var llt = NpoiExcelFunction.ExcelRead(filePath, "Sheet1", s => s);
                    if (llt.Count > 1)
                    {
                        ///获取第一行,处理列相关索引信息;
                        var columns = llt[0];
                        var columnNoLanguageMapping = new Dictionary<int, string>();
                        int idColumnNo = -1;
                        for (int i = 0; i < columns.Count; i++)
                        {
                            if (columns[i] == "Id")
                            {
                                idColumnNo = i;
                            }
                            else if (_isoLanguageCodes.ContainsKey(columns[i]))
                            {
                                columnNoLanguageMapping.Add(i, columns[i]);
                                if (!_languageMapping.ContainsKey(columns[i]))
                                {
                                    _languageMapping.Add(columns[i], new Dictionary<string, string>());
                                }
                            }
                        }
                        ///从第二行开始,为语言具体内容信息

                        if (idColumnNo == -1)
                        {
                            _log.ErrorLog($"{_taskName}:Load language file error 'can not find Id column' !");
                            return false;
                        }
                        else
                        {
                            for (int i = 1; i < llt.Count; i++)
                            {
                                var id = llt[i][idColumnNo];
                                foreach (var columNoLanguage in columnNoLanguageMapping)
                                {
                                    if (_languageMapping[columNoLanguage.Value].ContainsKey(id))
                                    {
                                        _log.ErrorLog($"{_taskName}:Load language file error'{id} have exit'!");
                                    }
                                    else
                                    {
                                        _languageMapping[columNoLanguage.Value].Add(id, llt[i][columNoLanguage.Key]);
                                    }
                                }
                            }
                        }
                    }
                }
                return true;

            }
            catch (Exception e)
            {

                _log.ErrorLog($"{_taskName}:Load language file error'{e.Message}'!");
                return false;
            }
        }
        /// <summary>
        /// 判断是否是当前语言
        /// </summary>
        /// <param name="language">ISO标准码</param>
        /// <returns></returns>
        public bool IsCurrentLanguage(string language)
        {
            return _language == language;
        }

        public string Translate(string key)
        {
            string id;
            if (key.Contains("G_Text_"))
            {
                id = key;
            }
            else
            {
                id = $"G_Text_{key.Trim()}";
            }
            if (_languageMapping.ContainsKey(_language))
            {

                if (_languageMapping[_language].TryGetValue(id, out string content))
                {
                    return content;
                }
                else
                {
                    return id;
                }
            }
            else
            {
                return id;
            }
        }

        public string TranslateBaseOnRules(string transcode)
        {
            string result;
            string idmTag = "idm=";
            string idsTag = "ids=";
            string ntrTag = "ntr=";
            if (transcode.Contains(idmTag))
            {
                string[] temps = StringHandler.Split(transcode, "||");
                if (temps[0].Contains(idmTag) && temps.Length>1)
                {
                    string mDes =Translate( temps[0].Replace(idmTag, ""));
                    string[] temps1 = new string[temps.Length - 1];
                    for (int i = 1; i < temps.Length; i++)
                    {
                        if (temps[i].Contains(idsTag))
                        {
                            temps1[i - 1] = Translate(temps[i].Replace(idsTag, ""));
                        }
                        else if(temps[i].Contains(ntrTag))
                        {
                            temps1[i - 1] = temps[i].Replace(ntrTag, "");
                        }
                    }
                    result = string.Format(mDes, temps1);
                }
                else
                {
                    result = Translate(transcode);
                }
            }
            else
            {
                result = Translate(transcode);
            }
            return result;
        }
    }
}
