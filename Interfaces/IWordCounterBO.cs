using Model.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IWordCounterBO
    {
        public Task<ResponseBase<StatusEnum>> SaveWordCounter(string request);
        public Task<int> GetWordStatistic(string wordName);

    }
}
