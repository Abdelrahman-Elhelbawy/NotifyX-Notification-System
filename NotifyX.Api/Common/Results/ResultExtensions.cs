using Microsoft.AspNetCore.Mvc;

namespace NotifyX.Api.Common.Results;

public static class ResultExtensions
{
    public static ObjectResult ToProblem(this Result result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsSuccess)
        {
            throw new InvalidOperationException(
                "Cannot convert a successful result to a problem.");
        }

        var problemDetails = new ProblemDetails
        {
            Status = result.Error.StatusCode,
            Title = result.Error.Code,
            Detail = result.Error.Description
        };

        problemDetails.Extensions["errors"] = new[]
        {
            new
            {
                Code = result.Error.Code,
                Description = result.Error.Description
            }
        };

        return new ObjectResult(problemDetails)
        {
            StatusCode = result.Error.StatusCode
        };
    }
}
