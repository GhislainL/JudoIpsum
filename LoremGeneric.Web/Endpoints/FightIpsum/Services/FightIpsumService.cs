using LoremGeneric;

namespace FightIpsum.Endpoints.FightIpsum.Services;

public interface IFightIpsumService
{
    public IEnumerable<string> GenerateLorem(int paragraphNumber, ParagraphSize paragraphSize, bool specialStart, bool japanese);
}

public class FightIpsumService : IFightIpsumService
{
    Random random = new Random();

    public string GenerateWord(bool japanese)
    {
        if (japanese)
        {
            int idx = random.Next(Constants.DicoJapan.Length);
            return Constants.DicoJapan[idx];
        }
        else
        {
            int idx = random.Next(Constants.Dico.Length);
            return Constants.Dico[idx];
        }
    }

    public string GenerateSentence(bool japanese)
    {
        int numberOfWord = random.Next(10, 15);
        string[] res = new string[numberOfWord];
        for (int i = 0; i < numberOfWord; i++)
        {
            res[i] = GenerateWord(japanese);
        }

        // TODO first letter to uppercase
        return string.Join(" ", res);
    }

    public string GenerateParagraph(ParagraphSize paragraphSize, bool japanese)
    {
        int numberOfSentence = paragraphSize switch
        {
            ParagraphSize.small => random.Next(2, 4),
            ParagraphSize.medium => random.Next(3, 5),
            ParagraphSize.large => random.Next(4, 6),
            _ => throw new NotImplementedException()
        };

        string[] res = new string[numberOfSentence];

        for (int i = 0; i < numberOfSentence; i++)
        {
            res[i] = GenerateSentence(japanese);
        }

        // TODO remove last ' '
        return string.Join(". ", res);
    }

    public IEnumerable<string> GenerateLorem(
        int paragraphNumber,
        ParagraphSize paragraphSize,
        bool specialStart = false,
        bool japanese = false)
    {
        var res = new List<string>();
        for (int i = 0; i < paragraphNumber; i++)
        {
            res.Add(GenerateParagraph(paragraphSize, japanese));
        }

        if (specialStart)
        {
            res[0] = $"{Constants.SpecialStart} {res[0]}";
        }

        return res;
    }
}
