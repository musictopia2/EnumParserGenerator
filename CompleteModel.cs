namespace EnumParserGenerator;
internal record CompleteModel(
    string AssemblyName,
    ImmutableArray<ResultsModel> Results
);