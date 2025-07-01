using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Repositories.Model;


namespace Repositories.Services
{
    public class JobSearchServices
    {
        private readonly ElasticsearchClient _client;
        private readonly string _indexName;

        public JobSearchServices(ElasticsearchClient client, string indexName = "jobs")
        {
            _client = client;
            _indexName = indexName;
        }
        public async Task CreateOrUpdateIndex_NIU(List<Job_Post1> jobs)
        {
            // Check if index exists
            var indexExists = await _client.Indices.ExistsAsync(_indexName);

            if (!indexExists.Exists)
            {
                var createIndexResponse = await _client.Indices.CreateAsync(_indexName, c => c
                    .Index(_indexName)
                    .Mappings(m => m
                        .Properties(new Properties
                        {
                    { nameof(Job_Post1.c_job_title), new KeywordProperty() },
                    { nameof(Job_Post1.c_job_desc), new KeywordProperty() },
                    { nameof(Job_Post1.c_job_location), new KeywordProperty() },
                    { nameof(Job_Post1.c_job_type), new KeywordProperty() },
                    { nameof(Job_Post1.c_salary_range), new TextProperty
                      {
                          Fields = new Properties
                          {
                              { "keyword", new KeywordProperty() }
                          }
                      }
                    },
                    { nameof(Job_Post1.c_vacancy), new IntegerNumberProperty() },
                    { nameof(Job_Post1.c_dept_id), new IntegerNumberProperty() },
                    { nameof(Job_Post1.c_qualification_title), new KeywordProperty() },
                    { nameof(Job_Post1.c_skills), new KeywordProperty() },
                    { nameof(Job_Post1.c_company_id), new IntegerNumberProperty() }
                        })
                    )
                );

                if (!createIndexResponse.IsValidResponse)
                {
                    throw new Exception($"Failed to create index: {createIndexResponse.DebugInformation}");
                }
            }

            // Bulk index the documents with a unique identifier
            var bulkDescriptor = new BulkRequest(_indexName);

            foreach (var job in jobs)
            {
                bulkDescriptor.Operations.Add(new BulkIndexOperation<Job_Post1>(job)
                {
                    Id = job.c_job_id.ToString() // Ensure unique ID is used
                });
            }

            var bulkResponse = await _client.BulkAsync(bulkDescriptor);

            if (!bulkResponse.IsValidResponse)
            {
                throw new Exception($"Failed to index documents: {bulkResponse.DebugInformation}");
            }
        }

        public async Task CreateOrUpdateIndex(List<Job_Post1> jobs)
        {
            // Check if index exists
            var indexExists = await _client.Indices.ExistsAsync(_indexName);

            if (!indexExists.Exists)
            {
                var createIndexResponse = await _client.Indices.CreateAsync(_indexName, c => c
                    .Index(_indexName)
                    .Mappings(m => m
                        .Properties(new Properties
                        {
            { nameof(Job_Post1.c_job_title), new KeywordProperty() },
            { nameof(Job_Post1.c_job_desc), new KeywordProperty() },
            { nameof(Job_Post1.c_job_location), new KeywordProperty() },
            { nameof(Job_Post1.c_job_type), new KeywordProperty() },
            { nameof(Job_Post1.c_salary_range), new TextProperty
              {
                  Fields = new Properties
                  {
                      { "keyword", new KeywordProperty() }
                  }
              }
            },
            { nameof(Job_Post1.c_vacancy), new IntegerNumberProperty() },
            { nameof(Job_Post1.c_dept_id), new IntegerNumberProperty() },
            { nameof(Job_Post1.c_qualification_title), new KeywordProperty() },
            { nameof(Job_Post1.c_skills), new KeywordProperty() },
            { nameof(Job_Post1.c_company_id), new IntegerNumberProperty() }
                        })
                    )
                );
                if (!createIndexResponse.IsValidResponse)
                {
                    throw new Exception($"Failed to create index: {createIndexResponse.DebugInformation}");
                }
            }

            // Bulk index the documents
            var bulkResponse = await _client.BulkAsync(b => b
                .Index(_indexName)
                .IndexMany(jobs)
            );

            if (!bulkResponse.IsValidResponse)
            {
                throw new Exception($"Failed to index documents: {bulkResponse.DebugInformation}");
            }
        }

        public async Task<List<Job_Post1>> SearchJobs(
        string? searchText = null,
        string? jobTitle = null,
        string? location = null,
        string? jobType = null,
        string? salaryRange = null,
        decimal? searchSalary = null, //New
        int? vacancy = null,
        int? departmentId = null,
        string? qualification = null,
        string? skills = null,
        int? companyId = null,
        string? companyName = null,
        string? departmentName = null,
        string? companyLogo = null)
        {
            var mustQueries = new List<Query>();

            // if (!string.IsNullOrEmpty(jobTitle))
            //     mustQueries.Add(new MatchPhraseQuery(new Field("c_job_title")) { Query = jobTitle });

            if (!string.IsNullOrEmpty(jobTitle))
            {
                // Replace MatchPhraseQuery with a combination of Match and Wildcard queries
                mustQueries.Add(new BoolQuery
                {
                    Should = new List<Query>
            {
                new MatchQuery(new Field("c_job_title")) { Query = jobTitle },
                new WildcardQuery(new Field("c_job_title"))
                {
                    Value = $"*{jobTitle}*",
                    CaseInsensitive = true
                }
            },
                    MinimumShouldMatch = 1
                });
            }
            if (!string.IsNullOrEmpty(companyName))
            {
                // Replace MatchPhraseQuery with a combination of Match and Wildcard queries
                mustQueries.Add(new BoolQuery
                {
                    Should = new List<Query>
            {
                new MatchQuery(new Field("c_company_name")) { Query = companyName },
                new WildcardQuery(new Field("c_company_name"))
                {
                    Value = $"*{companyName}*",
                    CaseInsensitive = true
                }
            },
                    MinimumShouldMatch = 1
                });
            }
            if (!string.IsNullOrEmpty(departmentName))
            {
                // Replace MatchPhraseQuery with a combination of Match and Wildcard queries
                mustQueries.Add(new BoolQuery
                {
                    Should = new List<Query>
            {
                new MatchQuery(new Field("c_dept_name")) { Query = departmentName },
                new WildcardQuery(new Field("c_dept_name"))
                {
                    Value = $"*{departmentName}*",
                    CaseInsensitive = true
                }
            },
                    MinimumShouldMatch = 1
                });
            }

            if (!string.IsNullOrEmpty(location))
            {
                mustQueries.Add(new BoolQuery
                {
                    Should = new List<Query>
            {
                new MatchQuery(new Field("c_job_location")) { Query = location },
                new WildcardQuery(new Field("c_job_location.keyword")) { Value = $"*{location}*", CaseInsensitive = true }
            },
                    MinimumShouldMatch = 1
                });
            }

            if (!string.IsNullOrEmpty(jobType))
                mustQueries.Add(new MatchPhraseQuery(new Field("c_job_type")) { Query = jobType });

            if (!string.IsNullOrEmpty(salaryRange))
                mustQueries.Add(new MatchPhraseQuery(new Field("c_salary_range")) { Query = salaryRange });

            if (searchSalary.HasValue)
            {
                mustQueries.Add(new ScriptQuery
                {
                    Script = new Script
                    {
                        Source = """
            if (doc['c_salary_range.keyword'].size() > 0) {
                def rangeText = doc['c_salary_range.keyword'].value;
                def rangeParts = rangeText.split("-");
                if (rangeParts.length == 2) {
                    def minSalary = Float.parseFloat(rangeParts[0].trim().replaceAll("[^0-9.]", ""));
                    def maxSalary = Float.parseFloat(rangeParts[1].trim().replaceAll("[^0-9.]", ""));
                    return params.search_salary >= minSalary && params.search_salary <= maxSalary;
                }
            }
            return false;
            """,
                        Lang = "painless",
                        Params = new Dictionary<string, object>
                {
                    { "salary_range", "doc['c_salary_range']" },
                    { "search_salary", searchSalary.Value }
                }
                    }
                });
            }
            else if (!string.IsNullOrEmpty(salaryRange))
            {
                mustQueries.Add(new MatchPhraseQuery(new Field("c_salary_range")) { Query = salaryRange });
            }

            if (!string.IsNullOrEmpty(qualification))
            {
                mustQueries.Add(new BoolQuery
                {
                    Should = new List<Query>
            {
                new MatchQuery(new Field("c_qualification_title")) { Query = qualification },
                new WildcardQuery(new Field("c_qualification_title.keyword")) { Value = $"*{qualification}*", CaseInsensitive = true }
            },
                    MinimumShouldMatch = 1
                });
            }


            if (!string.IsNullOrEmpty(skills))
            {
                // Replace MatchPhraseQuery with a combination of Match and Wildcard queries
                mustQueries.Add(new BoolQuery
                {
                    Should = new List<Query>
            {
                new MatchQuery(new Field("c_skills")) { Query = skills },
                new WildcardQuery(new Field("c_skills"))
                {
                    Value = $"*{skills}*",
                    CaseInsensitive = true
                }
            },
                    MinimumShouldMatch = 1
                });
            }

            var shouldQueries = new List<Query>();

            // if (!string.IsNullOrEmpty(searchText))
            // {
            //     shouldQueries.Add(new MatchPhraseQuery(new Field("c_job_title")) { Query = searchText });
            //     shouldQueries.Add(new MatchQuery(new Field("c_job_desc")) { Query = searchText });
            //     shouldQueries.Add(new MatchQuery(new Field("c_skills")) { Query = searchText });

            //     // Wildcard query for partial matches (case-insensitive)
            //     shouldQueries.Add(new WildcardQuery(new Field("c_job_title"))
            //     {
            //         Value = $"*{searchText}*",
            //         CaseInsensitive = true
            //     });

            //     shouldQueries.Add(new MultiMatchQuery
            //     {
            //         Query = searchText,
            //         Fields = new[]
            //         {
            //     new Field("c_job_title^3"),
            //     new Field("c_job_desc^2"),
            //     new Field("c_skills"),
            //     new Field("c_qualification_title")
            // },
            //         Type = TextQueryType.BestFields
            //     });
            // }
            
            if (!string.IsNullOrEmpty(searchText))
            {
                // Create a more comprehensive multi-match query for full-text search
                shouldQueries.Add(new MultiMatchQuery
                {
                    Query = searchText,
                    Fields = new[]
                    {
                        new Field("c_job_title^3"),        // Higher boost for job title
                        new Field("c_job_desc^2"),          // Medium boost for job description
                        new Field("c_skills^2"),            // Medium boost for skills
                        new Field("c_qualification_title^2"), // Medium boost for qualifications
                        new Field("c_job_location"),        // Include location
                        new Field("c_job_type"),            // Include job type
                        new Field("c_salary_range"),        // Include salary range
                        new Field("c_company_name"),        // Include company name
                        new Field("c_dept_name")            // Include department name
                    },
                    Type = TextQueryType.MostFields,        // Use MostFields for better matching
                    Operator = Operator.Or,                 // Match any of the terms
                    Fuzziness = new Fuzziness(1),          // Allow for slight misspellings
                    PrefixLength = 2                        // Minimum prefix length for fuzzy matching
                });

                // Add wildcard queries for partial matches
                shouldQueries.Add(new WildcardQuery(new Field("c_job_title"))
                {
                    Value = $"*{searchText}*",
                    CaseInsensitive = true
                });
                
                shouldQueries.Add(new WildcardQuery(new Field("c_job_desc"))
                {
                    Value = $"*{searchText}*",
                    CaseInsensitive = true
                });
                
                shouldQueries.Add(new WildcardQuery(new Field("c_skills"))
                {
                    Value = $"*{searchText}*",
                    CaseInsensitive = true
                });
                
                shouldQueries.Add(new WildcardQuery(new Field("c_qualification_title"))
                {
                    Value = $"*{searchText}*",
                    CaseInsensitive = true
                });
                
                shouldQueries.Add(new WildcardQuery(new Field("c_job_location"))
                {
                    Value = $"*{searchText}*",
                    CaseInsensitive = true
                });
                
                shouldQueries.Add(new WildcardQuery(new Field("c_company_name"))
                {
                    Value = $"*{searchText}*",
                    CaseInsensitive = true
                });
                
                shouldQueries.Add(new WildcardQuery(new Field("c_dept_name"))
                {
                    Value = $"*{searchText}*",
                    CaseInsensitive = true
                });
            }



            var searchRequest = new SearchRequest(_indexName)
            {
                Query = new BoolQuery
                {
                    Must = mustQueries,
                    Should = shouldQueries,
                    MinimumShouldMatch = shouldQueries.Any() ? 1 : 0
                }
            };

            var response = await _client.SearchAsync<Job_Post1>(searchRequest);

            if (!response.IsValidResponse)
            {
                throw new Exception($"Search query failed: {response.DebugInformation}");
            }

            // Deduplicate results based on job ID
            var uniqueResults = response.Documents
                .GroupBy(job => job.c_job_id)
                .Select(group => group.First())
                .ToList();

            return uniqueResults;
        }
    }
}