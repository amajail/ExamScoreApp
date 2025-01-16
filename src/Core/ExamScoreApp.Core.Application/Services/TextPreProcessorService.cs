using System.Text.RegularExpressions;

public class TextPreprocessor
{
    private static List<string> RemoveStopWords(string text)
    {
        // Convert to lowercase
        text = text.ToLower();

        // Remove special characters and numbers
        text = Regex.Replace(text, @"[^a-z\s]", "");

        // Split text into words
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

        // Remove common stopwords
        words.RemoveAll(word => SpanishStopwords.Contains(word));

        return words;
    }

    private static readonly HashSet<string> SpanishStopwords = new HashSet<string>
    {
        "a", "al", "algo", "algunas", "algunos", "ante", "antes", "como", "con", "contra",
        "cual", "cuales", "cuando", "de", "del", "desde", "donde", "durante", "e", "el",
        "ella", "ellas", "ellos", "en", "entre", "era", "erais", "eran", "eras", "eres",
        "es", "esa", "esas", "ese", "esos", "esta", "estaba", "estaban", "estabas",
        "estad", "estada", "estadas", "estado", "estados", "estais", "estamos", "estan",
        "estando", "estar", "estará", "estarán", "estarás", "estaré", "estaríamos",
        "está", "esté", "estos", "estoy", "fui", "fuimos", "fue", "fueron", "fui",
        "fuese", "fuesen", "fueses", "fuerais", "fueran", "fuéramos", "fuesen", "fui",
        "haya", "hayas", "hayamos", "hayáis", "hayan", "hay", "he", "hemos", "han",
        "hubiera", "hubieran", "hubieras", "hubiésemos", "hubiesen", "hubo", "hubieron",
        "hubiera", "hubiese", "hubiésemos", "hubieron", "hubo", "hizo", "hice", "hiciste",
        "hizo", "hicimos", "hicieron", "la", "las", "lo", "los", "me", "mi", "mis",
        "mía", "mías", "mío", "míos", "nuestra", "nuestro", "nuestras", "nuestros",
        "o", "os", "para", "pero", "por", "que", "qué", "quien", "quienes", "se", "sea",
        "sean", "ser", "será", "serán", "si", "sí", "sido", "sin", "sobre", "su", "sus",
        "también", "tan", "tanto", "te", "tenía", "tendría", "tendrán", "tendré",
        "tenemos", "tengo", "ti", "tiene", "tienen", "toda", "todas", "todo", "todos",
        "tu", "tus", "tú", "un", "una", "uno", "unos", "usted", "ustedes", "va",
        "van", "vaya", "ve", "veces", "ver", "vosotros", "y", "ya"
    };

    // Method to clean text by removing special characters and extra spaces
    private static string CleanText(string text)
    {
        // Remove special characters except spaces and punctuation
        text = Regex.Replace(text, @"[^\w\s.,!?]", "");

        // Trim extra spaces
        text = Regex.Replace(text, @"\s+", " ").Trim();

        // Convert to lowercase for normalization
        return text.ToLower();
    }

    // Method to tokenize text into sentences
    private static List<string> TokenizeSentences(string text)
    {
        // Split text into sentences based on punctuation marks
        var sentences = Regex.Split(text, @"(?<=[.!?])\s+");
        return new List<string>(sentences);
    }

    // Method to add context and format the text for ChatGPT
    private static string AddContext(string taskDescription, string text)
    {
        return $"### Task: {taskDescription}\n\n**Text:**\n{text}";
    }
    private static string PreProcessNotes(string notes)
    {
        // Tokenize the text into sentences
        List<string> sentences = TokenizeSentences(notes);

        return PreProcessNotesList(sentences);
    }

    public static string PreProcessNotesList(List<string> notes)
    {
        string formattedText = string.Empty;

        for (int i = 0; i < notes.Count; i++)
        {
            formattedText += $"- {CleanText(notes[i])}\n";
        }

        //  Add context and structure for ChatGPT
        string structuredText = AddContext("Notes to process and generate a report", formattedText);

        return structuredText;
    }

}