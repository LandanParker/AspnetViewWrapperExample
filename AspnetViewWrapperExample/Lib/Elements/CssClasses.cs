using System.Collections.Generic;
using AspnetViewWrapperExample.Elements;

namespace AspnetViewWrapperExample.Lib.Elements
{
    public class CssClasses : IAttribute
    {
        public HashSet<string> ClassMap { get; set; }

        public string GetClasses() => $"{string.Join(" ", ClassMap)}";
        
        public string addClass
        {
            set => AddClass(value);
        }
        
        public void AddClass(string item)=> 
            (ClassMap??=new HashSet<string>()).Add(item);

        public string AttributeKey => "class";
        public string AttributeValue => GetClasses();

        public override string ToString() => GetClasses();
    }
}