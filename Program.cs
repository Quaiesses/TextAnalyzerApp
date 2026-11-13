using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Serilog;
using Newtonsoft.Json;

namespace TextAnalyzerApp
{
    public interface ITextAnalyzer
    {
        string Analyze(string text);
    }

    public class WordCounter : ITextAnalyzer
    {
        public string Analyze(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "[WordCounter] Текст пуст.";
            int wordCount = text.Split(new[] { ' ', '\n', '\r', '\t', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries).Length;
            return $"[WordCounter] Количество слов в тексте: {wordCount}";
        }
    }

    public class SentimentAnalyzer : ITextAnalyzer
    {
        public string Analyze(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "[SentimentAnalyzer] Текст пуст.";

            string lowerText = text.ToLower();
            int positiveScore = (lowerText.Contains("отличн") || lowerText.Contains("хорош") || lowerText.Contains("рад") || lowerText.Contains("лучш")) ? 1 : 0;
            int negativeScore = (lowerText.Contains("плох") || lowerText.Contains("ужас") || lowerText.Contains("проблем")) ? 1 : 0;

            string sentiment = "Нейтральная";
            if (positiveScore > negativeScore) sentiment = "Позитивная";
            else if (negativeScore > positiveScore) sentiment = "Негативная";

            return $"[SentimentAnalyzer] Тональность текста: {sentiment}";
        }
    }

    public class KeywordExtractor : ITextAnalyzer
    {
        public string Analyze(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "[KeywordExtractor] Текст пуст.";

            var words = Regex.Matches(text, @"\b[а-яёа-яa-z]{6,}\b", RegexOptions.IgnoreCase)
                             .Cast<Match>()
                             .Select(m => m.Value)
                             .Distinct()
                             .Take(5);

            string keywords = string.Join(", ", words);
            return $"[KeywordExtractor] Ключевые слова: {(string.IsNullOrEmpty(keywords) ? "не найдены" : keywords)}";
        }
    }

    public class LanguageDetector : ITextAnalyzer
    {
        public string Analyze(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "[LanguageDetector] Текст пуст.";

            bool hasCyrillic = text.Any(c => c >= 'А' && c <= 'я');
            bool hasLatin = text.Any(c => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'));

            string lang = "Неизвестно";
            if (hasCyrillic && hasLatin) lang = "Смешанный (Русский / Английский)";
            else if (hasCyrillic) lang = "Русский";
            else if (hasLatin) lang = "Английский";

            return $"[LanguageDetector] Предполагаемый язык: {lang}";
        }
    }

    public class TextAnalysisService
    {
        private readonly IEnumerable<ITextAnalyzer> _analyzers;

        public TextAnalysisService(IEnumerable<ITextAnalyzer> analyzers)
        {
            _analyzers = analyzers ?? throw new ArgumentNullException(nameof(analyzers));
        }

        public List<string> ProcessAll(string text)
        {
            var results = new List<string>();
            foreach (var analyzer in _analyzers)
            {
                string res = analyzer.Analyze(text);
                results.Add(res);
                
                Log.Information("Выполнен анализ модулем {AnalyzerType}. Результат: {Result}", analyzer.GetType().Name, res);
            }
            return results;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Конфигурация Serilog (запись в консоль и в файл logs/app.log)
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("=== Запуск системы анализа текста с Serilog и JSON ===");

            try
            {
                string sampleText = "Hello world! Это тестовый текст для проверки работы нашей системы анализа. C# и .NET — отличный выбор!";

                var analyzers = new List<ITextAnalyzer>
                {
                    new WordCounter(),
                    new SentimentAnalyzer(),
                    new KeywordExtractor(),
                    new LanguageDetector()
                };

                var service = new TextAnalysisService(analyzers);
                var results = service.ProcessAll(sampleText);

                // Сериализация результатов в JSON через Newtonsoft.Json
                string jsonOutput = JsonConvert.SerializeObject(results, Formatting.Indented);
                File.WriteAllText("analyzers_report.json", jsonOutput);
                
                Log.Information("Отчет успешно сериализован и сохранен в файл 'analyzers_report.json'.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Произошла критическая ошибка во время выполнения программы.");
            }
            finally
            {
                Log.CloseAndFlush();
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}