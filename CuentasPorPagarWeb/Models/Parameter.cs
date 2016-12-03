using Parse;

namespace CuentasPorPagar.Models
{
    internal class Parameter : ParseObject
    {
        public Parameter(string processedYear, string processedMonth, bool executedClosing)
        {
            ProcessedYear = processedYear;
            ProcessedMonth = processedMonth;
            ExecutedClosing = executedClosing;
        }

        public string ProcessedYear { get; set; }
        public string ProcessedMonth { get; set; }
        public bool ExecutedClosing { get; set; }
    }
}