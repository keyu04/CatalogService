using CatalogService.Common.Constants;
using CatalogService.Common.DTOs;

namespace CatalogService.Common.Helpers;

public static class ResponseHelper
{
    public static ApiResponse<T> Success<T>(T data, string Code)
    {
        return new ApiResponse<T>
        {
            status = true,
            Message = LogConst.Success,
            Data = data,
            Code = Code,
        };
    }

    public static ApiResponse<object> Success(string Code)
    {
        return new ApiResponse<object>
        {
            status = true,
            Message = LogConst.Success,
            Code = Code
        };
    }

    public static ApiResponse<object> Failure(string? message)
    {
        return new ApiResponse<object>
        {
            status = false,
            Code = LogConst.CATALOG_SERVICE + LogConst.SERVER_CODE,
            Message = message ?? LogConst.Failure
        };
    }
}