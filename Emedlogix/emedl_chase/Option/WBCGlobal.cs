using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emedl_chase.Option
{
    public static class WBCGlobal
    {
        public const string ApiServiceUrl = "http://localhost:3500/";  //Python service url

        public const string EXCEPTION_LOG_PATH = @"/Error/Exception.log";
        public const string IO_LOG_PATH = @"/IO/req-res{0}.log";
        public const string EXCEL_CONTENT_TYPE = @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public static CultureInfo CURRENCY_CULTURE = null;
        public const int PageSize = 20;
        public const string StandardTimeFormat = "hh:mm tt";
        public const string StandardDateFormat = "dd-MM-yyyy";
        public const string CustomDateFormat = "dd-MM-yyyy";
        public const string StandardDateTimeFormat = "dd-MM-yyyy HH:mm";
        public const string REQUIRED_ERROR_MESSAGE = "{0} is required.";
        public const string LENGTH_ERROR_MESSAGE = "{0} allows {2} to {1} characters only.";
        public const string LENGTH_ERROR_MESSAGE2 = "{0} allowed only {1} characters";
        public const string EMAIL_FORMAT = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,8})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
        public const string STRING_FORMAT = @"^([a-zA-Z]+\s)*[a-zA-Z]+$";
        public const string ALPHA_NUMERIC_CHARACTER = @"^[a-zA-Z\s.,0-9@#$%*():;""'/?!+=_-]{1,30}$";
        public const string STRING_FORMAT_ERROR = "{0} allows alphabets only";
        public const string FORMAT_ERROR = "Incorrect format";
        public const string EMAIL_FORMAT_ERROR = "Email incorrect format";
        public const string NUMBER_FORMAT = @"^\d+$";
        public const string NUMBER_FORMAT_ERROR = "{0} allows number only";
        public const string UrlFormat = @"^/((?:https?\:\/\/|www\.)(?:[-a-z0-9]+\.)*[-a-z0-9]+.*)/i";

        public const string PASSWORD_FORMAT = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*(_|[^\w])).+$";
        public const string PASSWORD_FORMAT_ERROR = "Password needs at least 1 uppercase, 1 lowercase, 1 number and 1 symbol";
        public const string TIME_FORMAT_ERROR = "Incorrect format(HH:mm)";
        public const string RANGE_ERROR_MESSAGE = "The field {0} range from {1} to {2} only.";
        public const string INDIRECT_FINDING_XLSX = @"/Excel";

        public const string NEGATIVE_RANGE_ERROR_MESSAGE = "The field {0}  should not have nagative value.";
        public const string CONSOLE_LOG = "ConsoleLog";
        public const string ACCESS_LOG = "access_log";
        public const string TRY_LOG = "Trylog";
        public const string API_ACCESS_LOG = "request_log";
        /*  Upload file path */
        public const string EXCEL_UPLOAD_PATH = @"/Excel/";
        public const string FILE_PATH = @"/Files/";
        public const string JOB_FILES_PATH = "UploadedDocs";
        public const string PROVIDER_FILE_PATH = "UploadProviderFiles";
        public const string ATTACHMENT = "AttachMent";
        public const string ZIP_FILE_PATH = "ZipFilePathofFilteredImages";
        public const string UPLOADEXCEPTION = "UploadedErrorForDenial";
        public const string ChargeFiles = "ChargeFiles";
        public const string PaymnetFiles = "PaymentFiles";

        public const string RIPEATTACHMENT = "RIPEAttachments";

        public const string ERROR_UPLOAD_PATH = @"/ErrorFiles/";
        public const string DOC_UPLOAD_PATH = @"/Doc/";
        public const string PROFILE_IMAGE_UPLOAD_PATH = @"/Profile/";
        public const string EMAIL_TEMP_PATH = @"/Server/EmailFile/";


        public const string VISION_API_ANNOTATE = "annotate?key={0}";
        public const string VISION_API_ASYNCBATCHANNOTATE = "asyncBatchAnnotate?key={0}";
        public const string MIMETYPE_IMAGE = "image/tiff";
        public const string MIMETYPE_PDF = "application/pdf";


        //OCR
        public const string FN_UPLOAD_TO_OCR = "api/uploaddocs";
        public const string FN_GET_OCR = "api/docid/";

        public static List<string> ListOfStrings = new List<string>() { "Code", "RuleId", "Descs", "Score", "ValidFromDate", "ValidToDate", "Status", "CreatedBy", "CreatedDate", "ModifiedBy", "ModifiedDate", "IsDeleted", "ErrorMessage", "IncludedRules", "RowId", "RuleNo", "Source", "Priority", "Local_Descs", "IsNonHCC", "Reviewed", "NextRule", "IsStndAlone", "Rulewithoutwords", "NextRuleID", "ManualInclude", "ManualExclude", "WithoutWords", "RowId", "Conditions_UPPER", "CodeLevelWithoutWords" };

        public static List<string> NotNeededCPTRulesColumns = new List<string>() { "Id", "RuleId", "Cpt_code", "Speciality", "Section", "Category", "Group", "LongDesc", "MediumDesc", "ShortDesc", "score", "Procedure_desc", "created_by", "modfied_by", "created_on", "modfied_on", "procedure_desc_lower", "modified_by", "modified_on", "isdeleted", "cpt_rule_score", "WithOutWord", "WithWord", "CptCodeWithOutWord", "row_id", "modified_by", "modified_on", "isdeleted" };

        public static List<string> ListOfStringsForCpt = new List<string>() { "Procedure_desc" };

        public static List<string> IGNORED_COLOUMN = new List<string>() { "Category", "CategoryID", "HCC", "IsRxHcc" };

        public static List<string> UnKnownCloumns = new List<string>() {"Qualityspecificity", "NamePlace", "SequelaeHistory", "FamilialAutoimmune", "ComplicationProcess", "Manifestation", "Associatedcondition1", "Associatedcondition2", "Associatedcondition3", "Context",
 "Externalcontext", "AltKeyword",  "StageEncounter"};

        public static List<string> RuleHeaders = new List<string>() { "HCC", "Desc", "Condition", "Quality/specificity", "Name/Place", "Sequelae/History/Status/Dependence", "Familial/autoimmune/Acquired/Congenital/Premature/Foetal", "Complication/Process/Procedure/device", "Manifestation", "Associated condition / related to condition / involvement", "2 nd Associated condition", "3rd Associated condition", "Context", "External - context", "Alt Keyword", "Timing", "Severity", "Loc-prop", "Location-Body part", "Location-Organ", "Location-tissue/cells/Minor Areas/Gland/other components or substances", "Location-Laterality", "Location-Position", "Location-Body system", "Units", "Age related", "Gender related",
            "Stage Encounter", "Other specific", "Unspecified","Rule No","Source","Local_Desc","IsNonHCC","Reviewed","EfffDate","TermDate","RuleID","StandAlone" };

        public static List<string> stopWords = new List<string>(new string[]
{
    "a", "about", "above", "after", "again", "against", "all", "am", "an", "and", "any", "are", "aren't", "as", "at",
    "be", "because", "been", "before", "being", "below", "between", "both", "but", "by", "can't", "cannot", "could", "couldn't",
    "did", "didn't", "do", "does", "doesn't", "doing", "don't", "down", "during", "each", "few", "for", "from", "further",
    "had", "hadn't", "has", "hasn't", "have", "haven't", "having", "he", "he'd", "he'll", "he's", "her", "here", "here's",
    "hers", "herself", "him", "himself", "his", "how", "how's", "i", "i'd", "i'll", "i'm", "i've", "if", "in", "into",
    "is", "isn't", "it", "it's", "its", "itself", "let's", "me", "more", "most", "mustn't", "my", "myself", "no", "nor",
    "not", "of", "off", "on", "once", "only", "or", "other", "ought", "our", "ours", "ourselves", "out", "over", "own",
    "same", "shan't", "she", "she'd", "she'll", "she's", "should", "shouldn't", "so", "some", "such", "than", "that", "that's",
    "the", "their", "theirs", "them", "themselves", "then", "there", "there's", "these", "they", "they'd", "they'll", "they're",
    "they've", "this", "those", "through", "to", "too", "under", "until", "up", "very", "was", "wasn't", "we", "we'd", "we'll",
    "we're", "we've", "were", "weren't", "what", "what's", "when", "when's", "where", "where's", "which", "while", "who", "who's",
    "whom", "why", "why's", "with", "won't", "would", "wouldn't", "you", "you'd", "you'll", "you're", "you've", "your", "yours",
    "yourself", "yourselves"
});

        public static List<string> stopWords_icd_validation = new List<string>(new string[]
{
    "a","about","after","again","against","all","am","an","and","any","are","aren\'t","as","at","be","because","been","being","but","by","cannot","can\'t","could","couldn\'t","did","didn\'t","do","does","doesn\'t","doing","don\'t","for","from","further","had","hadn\'t","has","hasn\'t","have","haven\'t","having","he","he\'d","he\'ll","her","here","here\'s","hers","herself","he\'s","him","himself","his","how","how\'s","i","i\'d","if","i\'ll","i\'m","in","is","isn\'t","it","it\'s","its","itself","i\'ve","let\'s","me","most","mustn\'t","my","myself","nor","of","off","on","only","or","other","ought","our","ours","ourselves","out","over","own","shan\'t","she","she\'d","she\'ll","she\'s","should","shouldn\'t","so","such","than","that","that\'s","the","their","theirs","them","themselves","then","there","there\'s","these","they","they\'d","they\'ll","they\'re","they\'ve","this","those","through","to","too","until","up","very","was","wasn\'t","we","we\'d","we\'ll","we\'re","were","weren\'t","we\'ve","what","what\'s","when","when\'s","where","where\'s","which","while","who","whom","who\'s","why","why\'s","won\'t","would","wouldn\'t","you","you\'d","you\'ll","your","you\'re","yours","yourself","yourselves","you\'ve"
});

        public static List<string> NonReviewRuleHeaders = new List<string>() { "HCC", "Desc", "Condition", "Quality/specificity", "Name/Place", "Sequelae/History/Status/Dependence", "Familial/autoimmune/Acquired/Congenital/Premature/Foetal", "Complication/Process/Procedure/device", "Manifestation", "Associated condition / related to condition / involvement", "2 nd Associated condition", "3rd Associated condition", "Context", "External - context", "Alt Keyword", "Timing", "Severity", "Loc-prop", "Location-Body part", "Location-Organ", "Location-tissue/cells/Minor Areas/Gland/other components or substances", "Location-Laterality", "Location-Position", "Location-Body system", "Units", "Age related", "Gender related",
            "Stage Encounter", "Other specific", "Unspecified","Rule No","Source","Local_Desc","IsNonHCC","Reviewed","RuleID","Message" };

        public static List<string> MedicationHeader = new List<string>() { "Code", "Description", "DrugName" };
        public static List<string> SymptomHeader = new List<string>() { "Code", "Symptom", "SymptomCode" };
        public static string[] DATE_FORMATS = { "M/d/yyyy h:mm:ss tt", "yyyyMMdd", "M/d/yyyy h:mm tt", "MM/dd/yyyy hh:mm:ss", "M/d/yyyy h:mm:ss", "M/d/yyyy hh:mm tt", "M/d/yyyy hh tt", "M/d/yyyy h:mm", "M/d/yyyy h:mm", "MM/dd/yyyy hh:mm", "M/dd/yyyy hh:mm", "ddMMyyyy", "M/d/yyyy", "M/d/yy", "MM/dd/yy", "yy/MM/dd", "yyyy-MM-dd", "dd-MMM-yy", "dd-MM-yyyy hh:mm:ss tt", "dd-MMM-yyyy hh:mm:ss tt", "dd-MM-yyyy hh:mm:ss", "d-M-yyyy hh:mm:ss", "dd/MM/yyyy", "yyyy/MM/dd", "MM/dd/yyyy", "ddd, dd MMM yyyy HH:mm:ss GMT" };
        public static List<string> RuleIgnoredHeaders = new List<string>() { "RuleId", "RuleNo", "CreatedBy", "CreatedDate", "ModifiedBy", "ModifiedDate", "IsDeleted", "Priority", "Rulewithoutwords" };
        public static List<string> DateOfServiceTerms = new List<string>()
        {
            //"Patient admitted on",
            //"Exam date",
            //"Encounter date on",
            //"Date of service",
            //"Hospital",
            //"DOB",
            //"Date of birth",
            "Encounter Date:",
            "Date of service:",
            "DOS:",
            "Service Date:",
            "Date:"
        };
        public static List<string> NonDOSTerms = new List<string>()
        {
            "ADM Date:",
            "DIS Date:",
        };
        public static List<string> OnlyDateTerm = new List<string>()
        {
            "Date:"
        };
        public static List<string> MemberID = new List<string>()
        {
            "MEMBERID",
            "PATIENTID"
        };

        public static List<string> FemaleServiceTerms = new List<string>()
        {
            " She ",
            " female ",
            " /f ",
             "She ",
              " female.",

        };

        public static List<string> MaleServiceTerms = new List<string>()
        {
           " He ",
            " male ",
            " /m ",
            " male.",

        };

        public static List<string> PatientNameServiceTerms = new List<string>()
        {
            "PATIENT:",
            "RE:"
        };


        public static List<string> AgeServiceTerms = new List<string>()
        {
            " year-old",
            " y.o.",
            " old",
            " age:",
            "-year-old",
        };

        public static List<string> DOBServiceTerms = new List<string>()
        {
            "DOB: ",
        };
        public static List<string> CodeStatusList = new List<string>()
        {
            "Extracted",
            "Added",
            "Removed",
            "Potential",
            "PotentialToExtracted",
            "ExtractedToPotential",
            "Final"
        };
        public enum CodeStatus
        {
            Extracted,
            Added,
            Removed,
            Potential,
            PotentialToExtracted,
            ExtractedToPotential,
            Final
        };
    }
}

