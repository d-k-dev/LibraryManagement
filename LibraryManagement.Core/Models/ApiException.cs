﻿namespace LibraryManagement.Core.Models
{
    public class ApiException : Exception
    {
        public int StatusCode { get; set; }

        public ApiException(string message, int statusCode = 400) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}