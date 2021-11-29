using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Linq;
using AspnetViewWrapperExample.Elements;

namespace AspnetViewWrapperExample.Lib.Elements
{
    public class Styles : IAttribute
    {
        public Dictionary<string, string> StyleMap { get; set; }

        public Styles AddStyle(string key, string val)
        {
            (StyleMap ??= new Dictionary<string, string>())[key] = val;
            return this;
        }

        public string GetStyles() =>
            $"{string.Join("; ", StyleMap.Select(e => $"{e.Key}: {e.Value}"))}";

        public string AttributeKey => "style";
        public string AttributeValue => GetStyles();

        public override string ToString() => GetStyles();
    }
}