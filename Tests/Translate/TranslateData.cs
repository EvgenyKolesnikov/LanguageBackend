using System.Collections;
using Language.Translate;

namespace Tests;

public class TranslateData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            new TranslateRequest
            {
                ClickedWord = "take",
                PreviousWord = "Please",
                NextWord = "off",
                Sentence = "Please take off your clothes and take an apple"
            },
            "take off",
            "take off",
            "снять"
        ];

        yield return
        [
            new TranslateRequest
            {
                ClickedWord = "take",
                PreviousWord = "and",
                NextWord = "an",
                Sentence = "Please take off your clothes and take an apple"
            },
            "take",
            "take",
            "взять"
        ];
        
        yield return
        [
            new TranslateRequest
            {
                ClickedWord = "took",
                PreviousWord = "I",
                NextWord = "it",
                Sentence = "I took it"
            },
            "took",
            "took (take)",
            "взял"
        ];
        
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
