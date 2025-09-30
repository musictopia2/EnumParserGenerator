namespace EnumParserGenerator;
internal static class WriterExtensions
{
    private static string GetInterfacePrefix(ResultsModel result)
    {
        return $"global::CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.TypeParsingHelpers.ITypeParsingProvider<{result.Namespace}.{result.ClassName}>.";
    }
    public static ICodeBlock AppendGetSupportedList(this ICodeBlock w, ResultsModel result)
    {
        if (result.Category == EnumSimpleTypeCategory.CustomEnum)
        {
            w.WriteLine($"global::CommonBasicLibraries.CollectionClasses.BasicList<string> {GetInterfacePrefix(result)}GetSupportedList")
                .WriteCodeBlock(w =>
                {
                    w.WriteLine("get")
                    .WriteCodeBlock(w =>
                    {
                        w.WriteLine("global::CommonBasicLibraries.CollectionClasses.BasicList<string> output = [];")
                        .WriteLine($"var start = global::{result.Namespace}.{result.ClassName}.CompleteList;")
                        .WriteLine("foreach (var item in start)")
                        .WriteCodeBlock(w =>
                        {
                            w.WriteLine("output.Add(item.Name);");
                        })
                        .WriteLine("return output;");
                    });
                });
        }
        else
        {
            w.WriteLine($"global::CommonBasicLibraries.CollectionClasses.BasicList<string> {GetInterfacePrefix(result)}GetSupportedList => ")
                .WriteListBlock(result.Values);
        }
        return w;
    }
    public static ICodeBlock AppendTryParse(this ICodeBlock w, ResultsModel result)
    {
        w.WriteLine($"bool {GetInterfacePrefix(result)}TryParse(string input, out global::{result.Namespace}.{result.ClassName} result, out string? errorMessage)")
            .WriteCodeBlock(w =>
            {
                if (result.Category == EnumSimpleTypeCategory.StandardEnum)
                {
                    //standard enum code.
                    w.PrivateStandardParsing(result);
                }
                else
                {
                    w.PrivateCustomParsing(result);
                }
            });
        return w;
    }
    private static ICodeBlock PrivateStandardParsing(this ICodeBlock w, ResultsModel result)
    {
        w.WriteLine($"if (Enum.TryParse(input, out global::{result.Namespace}.{result.ClassName} possible) == false)")
            .WriteCodeBlock(w =>
            {
                w.PrivateReturnError(result);
            })
            .WriteLine("result = possible;")
            .PrivateReturnSuccess();
            ;
        return w;
    }
    private static ICodeBlock PrivateCustomParsing(this ICodeBlock w, ResultsModel result)
    {
        w.WriteLine("try")
            .WriteCodeBlock(w =>
            {
                w.WriteLine($"result = global::{result.Namespace}.{result.ClassName}.FromName(input);")
                .PrivateReturnSuccess();
            })
            .WriteLine("catch (global::System.Exception)")
            .WriteCodeBlock(w =>
            {
                w.PrivateReturnError(result);
            });
        return w;
    }
    private static ICodeBlock PrivateReturnError(this ICodeBlock w, ResultsModel result)
    {
        w.WriteLine($$"""
            errorMessage = $"Invalid enum value for {{result.ClassName}}: {input}";
            """)
            .WriteLine("result = default;")
            .WriteLine("return false;");
        return w;
    }
    private static ICodeBlock PrivateReturnSuccess(this ICodeBlock w)
    {
        w.WriteLine("errorMessage = null;")
            .WriteLine("return true;");
        return w;
    }
}