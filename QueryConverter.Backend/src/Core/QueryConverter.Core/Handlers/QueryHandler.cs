﻿using QueryConverter.Core.ExceptionCodes;
using QueryConverter.Core.Processor;
using QueryConverter.Shared.Types.Exceptions;
using QueryConverter.Shared.Utils.Extensions;
using QueryConverter.Shared.Utils.Extensions.Conditions;
using QueryConverter.Types.Shared.Consts;
using QueryConverter.Types.Shared.Dto;
using TSQL.Statements;

namespace QueryConverter.Core.Handlers
{
    public class QueryHandler : IQueryHandler
    {
        private string elasticQuery;

        public async Task<ResultModel> HandleOrderByStatement(TSQLSelectStatement statement)
        {
            try
            {
                var table = statement.From.Table().Index;
                var conditions = statement.OrderBy.Condition();
                var fields = statement.Select.Fields();

                string tableStatement = $"GET {table}/_search";

                string orderByStatement = string.Empty;
                const string nextAggregationMarker = "(addNextAggregationHere)";

                foreach (var field in fields)
                {
                    string template = Templates.ConditionBy.Replace("(column)", field.Column);

                    if (field == fields.Last())
                        template = template.Replace("(additionalAggregation)", "");
                    else
                        template = template.Replace("(additionalAggregation)", nextAggregationMarker);

                    if (orderByStatement.Contains(nextAggregationMarker))
                        orderByStatement = orderByStatement.Replace(nextAggregationMarker, "," + Environment.NewLine + template);
                    else
                        orderByStatement = template;
                }

                List<string> conditionsList = ConditionStatement.GetConditionStatement(conditions);
                string conditionsStatement = Templates.Conditions.Replace("(conditions)", string.Join(",", conditionsList));

                string sizeStatement = Templates.SizeZero;

                string jsonPortion = $@"{{
                       {sizeStatement},
                       {orderByStatement},
                       {conditionsStatement}
                       }}".PrettyJson();

                elasticQuery = $"{tableStatement}{Environment.NewLine}{jsonPortion}";

                var rows = ExtensionMethods.SplitQuery(ref elasticQuery);

                var result = new ResultModel()
                {
                    ElasticQuery = elasticQuery,
                    Rows = rows
                };

                return result;

            }
            catch (Exception ex)
            {
                throw new QueryConverterException(Codes.InvalidArguments, $"{ex.Message}");
            }
        }
    }
}
