
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System.Transactions;
using Interfaces;
using Model.Entity;
using Microsoft.EntityFrameworkCore;
using Model.Dto;

namespace BL
{
    public class WordCounterBO : IWordCounterBO
    {
        private readonly IRepository _repository;

        public WordCounterBO(IRepository repository)
        {
            _repository = repository;
        }


        public async Task<ResponseBase<StatusEnum>> SaveWordCounter(string request)
        {

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var res = new ResponseBase<StatusEnum>();
                    Dictionary<string, int> wordCounterDic = new Dictionary<string, int>();
                    var wordsList = Regex.Replace(request, @"[^0-9a-zA-Z]+", " ").Split(" ");
                    //Prepare the request
                    foreach (string sentence in wordsList)
                    {
                        if (wordCounterDic.TryGetValue(sentence.ToLower(), out int wordcount))
                        {
                            wordCounterDic[sentence.ToLower()] += wordcount;
                        }
                        else // the first add
                        {
                            wordCounterDic.Add(sentence.ToLower(), 1);
                        }
                    }

                    var oldwordList = _repository.GetAll<WordCounter>();
                    var entityToDB = new List<WordCounter>();
                    //var entityToUpdate = new List<WordCounter>();
                    var entityToAdd = new List<WordCounter>();

                    var dbEntities = await (from aa in oldwordList
                                            where wordCounterDic.Keys.Contains(aa.WordName)
                                            select aa).ToListAsync();



                    if (dbEntities.Any())
                    {

                        foreach (var entity in dbEntities)
                        {
                            entity.Counter += wordCounterDic.TryGetValue(entity.WordName.ToLower(), out int wordCounter) ? wordCounter : 0;
                        }

                    }
                  
                    entityToAdd = (from word in wordCounterDic.Keys.Where(p => !dbEntities.Any(p2 => p2.WordName.ToLower() == p))
                                    select new WordCounter
                                    {
                                        WordName = word,
                                        Counter = wordCounterDic.TryGetValue(word.ToLower(), out int wordCounter) ? wordCounter : 0
                                    })?.ToList();

                

                    entityToDB.AddRange(dbEntities);
                    entityToDB.AddRange(entityToAdd);
                    await _repository.UpdateAsync<WordCounter>(entityToDB);

                    scope.Complete();
                 


                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    return new ResponseBase<StatusEnum>()
                    {
                        Response = StatusEnum.Failure
                    };
                     throw;
                }
            }


            //Save To database

            return new ResponseBase<StatusEnum>()
            {
                Response = StatusEnum.Success
            };
        }

        public async Task<int> GetWordStatistic(string wordName)
        {
            return await (from word in _repository.GetAll<WordCounter>()
                          where word.WordName == wordName
                          select word.Counter ?? 0)?.FirstOrDefaultAsync();
        }
    }
}
