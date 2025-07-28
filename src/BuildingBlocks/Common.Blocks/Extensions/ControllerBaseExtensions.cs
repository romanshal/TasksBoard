using Common.Blocks.Models;
using Common.Blocks.Models.ApiResponses;
using Common.Blocks.Models.DomainResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Common.Blocks.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static IActionResult HandleResponse(this ControllerBase controller, Result result, Func<IActionResult> onSuccessHandler = null)
        {
            return result.Match(
               onSuccess: () =>
               {
                   if (onSuccessHandler != null) return onSuccessHandler();

                   return controller.Ok(ApiResponse.Success());
               },
               onFailure: error => controller.MapErrors(error));
        }

        public static IActionResult HandleResponse<T>(this ControllerBase controller, Result<T> result, Func<IActionResult> onSuccessHandler = null)
        {
            return result.Match(
               onSuccess: () =>
               {
                   if (onSuccessHandler != null) return onSuccessHandler();

                   return controller.Ok(ApiResponse.Success(result.Value));
               },
               onFailure: error => controller.MapErrors(error));
        }

        private static IActionResult MapErrors(
            this ControllerBase controller,
            Error error) => error.Code switch
            {
                ErrorCodes.NotFound => controller.NotFound(ApiResponse.Error(error.Description)),
                ErrorCodes.Unauthorized => controller.Unauthorized(),
                ErrorCodes.Forbidden => controller.Forbid(),
                ErrorCodes.AlreadyExist => controller.BadRequest(ApiResponse.Error(error.Description)),
                ErrorCodes.NoEntities => controller.NotFound(ApiResponse.Error(error.Description)),
                ErrorCodes.CantCreate or ErrorCodes.CantUpdate or ErrorCodes.CantDelete or ErrorCodes.CantCancel => controller.Problem(detail: error.Description, statusCode: StatusCodes.Status503ServiceUnavailable),
                _ => controller.Problem(detail: error.Description, statusCode: StatusCodes.Status500InternalServerError)
            };
    }
}
