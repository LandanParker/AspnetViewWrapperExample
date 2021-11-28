using System;
using System.Globalization;

namespace AspnetViewWrapperExample
{
    public class TestClass
    {
        public string Value { get; set; } = "SomeValue";
        public string Time => DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
    }
}