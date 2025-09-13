using System.Collections;
using Language.Translate;

namespace Tests;

public class TranslateData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            new TranslateRequest
            {
                ClickedWord = "take",
                PreviousWord = "Please",
                NextWord = "off",
                Sentence = "Please take off your clothes and take an apple"
            },
            "take off",
            "снять"
        };

        yield return new object[]
        {
            new TranslateRequest
            {
                ClickedWord = "run",
                PreviousWord = "I",
                NextWord = "fast",
                Sentence = "I run fast every morning"
            },
            "run",
            "бежать"
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
