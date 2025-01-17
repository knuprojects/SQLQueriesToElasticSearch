﻿namespace QueryConverter.Types.Shared.Consts
{
    public static class Templates
    {
        public const string SingleCondition = @"
                {
                      ""match"": {
                        ""(column)"": ""(value)""
                      }
                }
            ";

        public const string InCondition = @"
                {
                      ""terms"": {
                        ""(column)"": [(value)]
                      }
                }
            ";

        public const string BetweenCondition = @"
                {
                      ""range"": {
                        ""(column)"": {
                            ""gte"": ""(lowerValue)"",
                            ""lte"": ""(upperValue)""
                        }                            
                      }
                }
            ";

        public const string ComparisonCondition = @"
                {
                      ""range"": {
                        ""(column)"": {
                            ""(operator)"": ""(value)""
                        }                            
                      }
                }
            ";

        public const string OrderCondition = @"
                {
                      ""sort"": [
                        {
                          ""(column)"": {
                            ""order"": ""(operator)""
                          }                            
                      }]
                }
            ";

        public const string LikeCondition = @"
                {
                      ""wildcard"": {
                        ""(column)"": ""(value)""
                      }
                }
            ";

        public const string Conditions = @"""query"": {
                ""bool"": {
                    ""must"": [
                        (conditions)
                    ]
                }
            }";

        public const string ConditionBy = @"
              ""aggregations"": {
                ""(column)"": {
                  ""terms"": { ""field"": ""(column).keyword"" }(additionalAggregation)
                }
              }";

        public static string SizeZero => @"""size"": 0";
    }
}
