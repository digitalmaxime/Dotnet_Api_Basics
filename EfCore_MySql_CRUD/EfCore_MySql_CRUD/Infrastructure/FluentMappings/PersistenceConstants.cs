namespace EfCore_MySql_CRUD.Infrastructure.FluentMappings;

public static class PersistenceConstants
{
    public static class Database
    {
        public const string DefaultCreatedBy = "system";

        public static class Types
        {
            public const string Boolean = "tinyint(1)"; // In MySql, its the way to represent booleans (0 or 1)
            public const string Int = "int";
            public const string Varchar = "varchar";
            public const string Nvarchar = "nvarchar";
            public const string LongText = "longtext";
            public const string Text = "text";
            public const string MediumText = "mediumtext";
            public const string DateTime  = "DATETIME ";
        }

        public static class Lengths
        {
            public const int MaxVarchar = 65535;
            public const int MaxNvarchar = 21845;
            public const int VersionLength = 20;
            public const int RefCodeLength = 50;
            public const int LabelRefCodeLength = 250;
            public const int DescriptionLength = 250;
            public const int ApiEndPointLength = 50;
            public const int NameLength = 50;
            public const int FunctionLength = 250;
            public const int UrlLength = 250;
            public const int CorrelationIdLength = 250;
        }
    }
}