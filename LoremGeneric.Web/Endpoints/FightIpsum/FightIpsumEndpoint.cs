using FightIpsum.Endpoints.FightIpsum.Services;
using Microsoft.AspNetCore.Mvc;

namespace FightIpsum.Endpoints.FightIpsum;

public static class FightIpsumEndpoint
{

    public static RouteGroupBuilder MapFightIpsumEndpoint(this RouteGroupBuilder builder)
    {
        builder.WithTags("FightIpsum");

        builder.MapGet("/{number}/{size}", GetGeneratedLorem)
            .Produces<IEnumerable<string>>()
            .WithOpenApi(options =>
            {
                options.Summary = "summary";

                var param1 = options.Parameters[0];
                param1.Description = "number of paragraphs (max 10)";

                var param2 = options.Parameters[1];
                param2.Description = "Size : 0 (small), 1 (medium), 2 (large)";
                return options;
            })
            .Produces(StatusCodes.Status429TooManyRequests)
            .RequireRateLimiting("API");

        return builder;
    }


    static IResult GetGeneratedLorem([FromServices] IFightIpsumService generatorService, int number, int size, bool jujitsu)
    {
        if (number > 10) number = 10;
        if (size > 2) size = 2;

        ParagraphSize pSize = (ParagraphSize)size;

        IEnumerable<string> res = generatorService.GenerateLorem(number, pSize, false, jujitsu);
        return TypedResults.Ok(res);
    }
}
