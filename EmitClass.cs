namespace EnumParserGenerator;
internal class EmitClass(CompleteModel complete, SourceProductionContext context)
{
    public void Emit()
    {
        foreach (var item in complete.Results)
        {
            WriteItem(item);
        }
        WriteGlobal();
    }
    private void WriteGlobal()
    {
        //now i have to do the global one.
        SourceCodeStringBuilder builder = new();
        builder.WriteGlobalClass(complete.AssemblyName, w =>
        {
            foreach (var item in complete.Results)
            {
                w.WriteLine($"global::CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.TypeParsingHelpers.CustomTypeParsingHelpers<global::{item.Namespace}.{item.ClassName}>.MasterContext = new global::{complete.AssemblyName}.EnumParserImplementations.{item.ClassName}EnumParserGenerator();");
            }
        });
        context.AddSource("EnumTypeParserRegistrations.g.cs", builder.ToString());
    }
    private void WriteItem(ResultsModel item)
    {
        SourceCodeStringBuilder builder = new();
        builder.WriteParserEnumClass(complete.AssemblyName, w =>
        {
            PopulateDetails(w, item);
        }, item);
        context.AddSource($"{item.ClassName}.EnumImplementation.g.cs", builder.ToString()); //change sample to what you want.
    }
    private void PopulateDetails(ICodeBlock w, ResultsModel result)
    {
        w.AppendGetSupportedList(result)
            .AppendTryParse(result);
    }
}