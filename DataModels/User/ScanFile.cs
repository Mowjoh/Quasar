namespace DataModels.User
{
    public class ScanFile
    {
        public string SourcePath { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string OriginPath { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string RootPath { get; set; }
        public string FilePath { get; set; }
        public string PreProcessedOutputFilePath { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string Hash { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string Slot { get; set; }
        public int GameElementID { get; set; }
        public int QuasarModTypeID { get; set; }
        public int QuasarModTypeFileDefinitionID { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string QuasarModTypeName { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string QuasarModTypeFileRule { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public bool Scanned { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public bool Ignored { get; set; }
    }
}
