﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TSQL;
using TSQL.Statements;

namespace SqlToElasticSearchConverter {

    public class SqlToElasticSearchConversion {

        public SqlToElasticSearchConversion(string sqlQuery) {
            ProcessSqlQuery(sqlQuery);
        }

        private void ProcessSqlQuery(string sqlQuery) {
            TSQLSelectStatement statement = TSQLStatementReader.ParseStatements(sqlQuery)[0] as TSQLSelectStatement;

            var table = statement.From.Table().Index;
            var conditions = statement.Where.Conditions();
            var fields = statement.Select.Fields();

            // get table statement
            string tableStatement = $"POST {table}/_search";

            // get the field statement
            string fieldStatement = string.Empty;

            if (fields.Count > 0 && fields[0].Column != "*") {
                // add quotes around each field, plus a starting minus
                fieldStatement = string.Join(", ", fields.Select(x => "\"" + x.Column + "\""));
                fieldStatement = ", \"fields\": [" + fieldStatement + "]";
            }

            // get the conditions statement
            string conditionText = string.Empty;
            var conditionsList = new List<string>();

            foreach (var condition in conditions) {

                switch (condition.Operator) {
                    case WhereCondition.OperatorType.Equal:

                        switch (condition.Type) {

                            case WhereCondition.LiteralType.Numeric:
                            case WhereCondition.LiteralType.String:
                                conditionText = Templates.SingleCondition
                                                    .Replace("(column)", condition.Column)
                                                    .Replace("(value)", condition.SingularValue);
                                break;
                        }
                        break;
                    case WhereCondition.OperatorType.In:

                        // add switch for condition types later
                        conditionText = Templates.InCondition
                                                    .Replace("(column)", condition.Column)
                                                    .Replace("(value)", string.Join(",", condition.InValues.Select(x => "\"" + x + "\"")));

                        break;

                    case WhereCondition.OperatorType.Between:

                        // add switch for condition types later
                        conditionText = Templates.BetweenCondition
                                                    .Replace("(column)", condition.Column)
                                                    .Replace("(lowerValue)", condition.BetweenValues.First())
                                                    .Replace("(upperValue)", condition.BetweenValues.Last());
                        break;

                    case WhereCondition.OperatorType.GreaterThan:
                    case WhereCondition.OperatorType.GreaterThanOrEquals:
                    case WhereCondition.OperatorType.LessThan:
                    case WhereCondition.OperatorType.LessThanOrEquals:
                        // add switch for condition types later
                        conditionText = Templates.ComparisonCondition
                                                    .Replace("(column)", condition.Column)
                                                    .Replace("(operator)", WhereCondition.FromOperatorType(condition.Operator))
                                                    .Replace("(value)", condition.SingularValue);
                        break;

                    case WhereCondition.OperatorType.Unknown:
                        break;
                }

                conditionsList.Add(conditionText);
            }
            string conditionsStatement = Templates.Conditions.Replace("(conditions)", string.Join(",", conditionsList));

            string jsonPortion = $@"{{
                {conditionsStatement}
                {fieldStatement}
            }}";

            // format JSON
            jsonPortion = JToken.Parse(jsonPortion).ToString(Formatting.Indented);

            // set module level variable
            ElasticQuery = $"{tableStatement}{Environment.NewLine}{jsonPortion}";
        }

        public string ElasticQuery { get; set; } = string.Empty;
    }
}
