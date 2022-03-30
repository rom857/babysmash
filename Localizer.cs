using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabySmash
{
    internal class Localizer
    {
        private Locale currentLocale;

        public Localizer()
        {
            CultureInfo keyboardLanguage = System.Windows.Forms.InputLanguage.CurrentInputLanguage.Culture;
            string culture = keyboardLanguage.Name;
            string path = $@"Resources\Strings\{culture}.json";
            string path2 = @"Resources\Strings\en-EN.json";
            string jsonConfig = null;
            if (File.Exists(path))
            {
                jsonConfig = File.ReadAllText(path);
            }
            else if (File.Exists(path2))
            {
                jsonConfig = File.ReadAllText(path2);
            }

            if (jsonConfig != null)
            {
                currentLocale = JsonConvert.DeserializeObject<Locale>(jsonConfig);
                
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "No file");
            }
        }

        /// <summary>Returns <param name="key"></param> if value or culture is not found.</summary>
        public string GetLocalizedPhrase(string color, string shape)
        {
            LocalizedShape localizedShape = currentLocale.GetLocalizedShape(shape);

            if (currentLocale.isAdjectiveAfterNoun)
                return String.Format("{0} {1}", localizedShape.Name, currentLocale.GetLocalizedColor(color, localizedShape.isFeminine));
            else
                return String.Format("{0} {1}", currentLocale.GetLocalizedColor(color, localizedShape.isFeminine), localizedShape.Name);
        }

    }

    public class Locale
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "adjectiveAfterNoun")]
        public bool isAdjectiveAfterNoun { get; set; } = false;

        [JsonProperty(PropertyName = "shapes")]
        public Dictionary<string, LocalizedShape> Shapes { get; set; }

        [JsonProperty(PropertyName = "colors")]
        public Dictionary<string, LocalizedColor> Colors { get; set; }

        public string GetLocalizedColor(string color, bool feminine)
        {
            if (Colors.ContainsKey(color))
            {
                if (feminine && !string.IsNullOrEmpty(Colors[color].Feminine))
                    return Colors[color].Feminine;
                else
                    return Colors[color].Masculine;
            }
            return color;
        }

        public LocalizedShape GetLocalizedShape(string shape)
        {
            if(Shapes.ContainsKey(shape))
            {
                return Shapes[shape];
            }

            return null;
        }
    }

    public class LocalizedShape
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        public bool isFeminine { get; set; } = false;
    }
    public class LocalizedColor
    {
        [JsonProperty(PropertyName = "name")]
        public string Masculine { get; set; }

        [JsonProperty(PropertyName = "feminine")]
        public string Feminine { get; set; }
    }
}
