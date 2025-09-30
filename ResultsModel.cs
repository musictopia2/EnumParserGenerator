namespace EnumParserGenerator;
internal record ResultsModel : ICustomResult
{
    public string ClassName { get; set; } = "";
    public string Namespace { get; set; } = "";
    public EnumSimpleTypeCategory Category { get; set; }
    public BasicList<string> Values { get; set; } = []; //sometimes we need values from them (does for simple enums)
}