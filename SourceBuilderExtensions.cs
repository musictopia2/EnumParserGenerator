namespace EnumParserGenerator;
internal static class SourceBuilderExtensions
{
    public static void WriteParserEnumClass(this SourceCodeStringBuilder builder, string ns, Action<ICodeBlock> action, ResultsModel result)
    {
        builder.WriteLine("#nullable enable")
                .WriteLine(w =>
                {
                    w.Write("namespace ")
                    .Write($"{ns}.EnumParserImplementations")
                    .Write(";");
                })
                .WriteLine(w =>
                {
                    w.Write($"internal class {result.ClassName}EnumParserGenerator")
                    .Write($" : global::CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.TypeParsingHelpers.ITypeParsingProvider<global::{result.Namespace}.{result.ClassName}>");
                })
                .WriteCodeBlock(action.Invoke);
    }
    public static void WriteGlobalClass(this SourceCodeStringBuilder builder, string ns, Action<ICodeBlock> action)
    {
        builder.WriteLine("#nullable enable")
                .WriteLine(w =>
                {
                    w.Write("namespace ")
                    .Write($"{ns}.EnumTypeParsingRegistrations")
                    .Write(";");
                })
                .WriteLine(w =>
                {
                    w.Write($"public static class RegisterEnumParserClass");
                })
                .WriteCodeBlock(w =>
                {
                    w.WriteLine("public static void Register()")
                    .WriteCodeBlock(x =>
                    {
                        action.Invoke(x);
                    });
                });
    }
}