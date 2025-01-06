using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiBase.Services
{
    public interface ILocalizationService
    {
        bool Init();
        event LanguageChangedEventHandler LanguageChanged;
        string this[string key] { get; }
        /// <summary>
        /// 按Key查找
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string Translate(string key);

        /// <summary>
        /// 按照规则进行翻译，如：操作记录翻译，包含主翻译/子翻译/点位名/值名
        /// </summary>
        /// <param name="transcode">idm=""||ids=""|ntr="";idm"主翻译ID"（唯一）,ids"子翻译ID"（可空,可多个）,ntr"不需翻译字符,直接使用"（可空）;</param>
        /// <returns></returns>
        string TranslateBaseOnRules(string transcode);

        string Language { get; set; }

        IDictionary<string, string> Languages { get; }
        bool IsCurrentLanguage(string language);
    }

    public delegate void LanguageChangedEventHandler(LanguageChangedEvent e);
    public class LanguageChangedEvent
    {
        public LanguageChangedEvent(string language)
        {
            Language = language;
        }
        public string Language { get; private set; }
    }
}
