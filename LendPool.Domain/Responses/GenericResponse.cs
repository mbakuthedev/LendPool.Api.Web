using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendPool.Domain.Responses
{

        public class GenericResponse<T>
        {
            public T? Data { get; set; }
            public bool Success { get; set; } = true;
            public string Message { get; set; } = "Request completed successfully.";
            public int StatusCode { get; set; } = 200;

            public static GenericResponse<T> SuccessResponse(T data, int statusCode, string message = "Success")
            {
                return new GenericResponse<T>
                {
                    Data = data,
                    Success = true,
                    Message = message,
                    StatusCode = statusCode
                };
            }

            public static GenericResponse<T> FailResponse(string message, int statusCode = 500)
            {
                return new GenericResponse<T>
                {
                    Data = default,
                    Success = false,
                    Message = message,
                    StatusCode = statusCode
                };
            }
        }
    }
