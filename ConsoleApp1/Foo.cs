using System;
using CsvHelper.Configuration.Attributes;

namespace ConsoleApp1
{
    public class Foo
    {
        [Index(0)]
        public int A { get; set; }
        [Index(1)]
        public int B { get; set; }
        [Index(2)]
        public int C { get; set; }
        [Index(3)]
        public int D { get; set; }
        [Index(4)]
        public int E { get; set; }
    }
}
