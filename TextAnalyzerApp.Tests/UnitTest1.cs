using System.Collections.Generic;
using TextAnalyzerApp;
using Xunit;

namespace TextAnalyzerApp.Tests
{
    public class MockAnalyzer : ITextAnalyzer
    {
        public bool IsCalled { get; private set; } = false;
        public string LastAnalyzedText { get; private set; } = string.Empty;

        public string Analyze(string text)
        {
            IsCalled = true;
            LastAnalyzedText = text;
            return "[Mock] Success";
        }
    }

    public class TextAnalysisServiceTests
    {
        [Fact]
        public void ProcessAll_CallsAllAnalyzers()
        {
            var mock1 = new MockAnalyzer();
            var mock2 = new MockAnalyzer();
            var service = new TextAnalysisService(new[] { mock1, mock2 });

            var results = service.ProcessAll("Тест");

            Assert.True(mock1.IsCalled);
            Assert.True(mock2.IsCalled);
            Assert.Equal(2, results.Count);
        }

        [Fact]
        public void WordCounter_ReturnsCorrectCount()
        {
            var counter = new WordCounter();
            string result = counter.Analyze("Один два три");
            
            Assert.Contains("3", result);
        }
    }
}