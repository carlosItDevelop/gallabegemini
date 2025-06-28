using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralLabSolutions.Domain.DomainObjects
{
    public class Result
    {
        public Result()
        {
            Errors = new List<string>();
            Data = new object();
        }

        public object Data { get; set; }
        public List<string> Errors { get; set; }
    }


    /*
     
     ToDo: Sugestão de refatoração;


        public class Result
        {
            public bool IsSuccess { get; }
            public bool IsFailure => !IsSuccess;
            public List<string> Errors { get; }
            protected Result(bool isSuccess, List<string> errors)
            {
                IsSuccess = isSuccess;
                Errors = errors ?? new List<string>();
            }

            public static Result Ok() => new Result(true, null);
            public static Result Fail(string error) => new Result(false, new List<string> { error });
            public static Result Fail(List<string> errors) => new Result(false, errors);
        }

        public class Result<T> : Result
        {
            public T Data { get; }

            protected Result(T data, bool isSuccess, List<string> errors) : base(isSuccess, errors)
            {
                Data = data;
            }

            public static Result<T> Ok(T data) => new Result<T>(data, true, null);
            public new static Result<T> Fail(string error) => new Result<T>(default(T), false, new List<string> { error });
            public new static Result<T> Fail(List<string> errors) => new Result<T>(default(T), false, errors);
        }
     
     */

}
