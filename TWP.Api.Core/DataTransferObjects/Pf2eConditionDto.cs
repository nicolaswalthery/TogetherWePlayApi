namespace TWP.Api.Core.DataTransferObjects
{
    public class Pf2eConditionDto
    {
        public string _id { get; set; }
        public string img { get; set; }
        public string name { get; set; }
        public SystemDetails system { get; set; }
        public string type { get; set; }
    }

    public class SystemDetails
    {
        public bool? active { get; set; } // Nullable, as it can be missing in some conditions
        public Description description { get; set; }
        public Duration duration { get; set; }
        public string group { get; set; } // Changed to string since "group" can be "senses", "abilities", or null
        public List<string> overrides { get; set; }
        public Publication publication { get; set; }
        public References references { get; set; }
        public bool? removable { get; set; } // Nullable as it can be missing in some cases
        public List<Rule> rules { get; set; }
        public Traits traits { get; set; }
        public Value value { get; set; }
    }

    public class Description
    {
        public string value { get; set; }
    }

    public class Duration
    {
        public object expiry { get; set; } // Object, as it can be null or another type
        public bool perpetual { get; set; }
        public string text { get; set; }
        public string unit { get; set; }
        public int? value { get; set; }
    }

    public class Publication
    {
        public string license { get; set; }
        public bool remaster { get; set; }
        public string title { get; set; }
    }

    public class References
    {
        public List<string> children { get; set; }
        public List<string> immunityFrom { get; set; }
        public List<string> overriddenBy { get; set; }
        public List<string> overrides { get; set; }
    }

    public class Traits
    {
        public List<string> value { get; set; }
    }

    public class Value
    {
        public bool isValued { get; set; }
        public object? value { get; set; } // Can be null or another type
    }

    public class Rule
    {
        public string key { get; set; }
        public string selector { get; set; }
        public string slug { get; set; }
        public string type { get; set; }
        public int? value { get; set; }
        public string domain { get; set; }
        public string option { get; set; }
        public bool? inMemoryOnly { get; set; }
        public List<Predicate> predicate { get; set; }
        public string text { get; set; }
        public string title { get; set; }
        public string itemType { get; set; }
        public ItemAlteration adjustment { get; set; }
    }

    public class Predicate
    {
        public List<string> or { get; set; }
        public List<PredicateAnd> and { get; set; }
        public string not { get; set; }
    }

    public class PredicateAnd
    {
        public string and { get; set; }
    }

    public class ItemAlteration
    {
        public string all { get; set; }
        public string key { get; set; }
        public string mode { get; set; }
        public string property { get; set; }
        public List<string> value { get; set; }
    }
}
