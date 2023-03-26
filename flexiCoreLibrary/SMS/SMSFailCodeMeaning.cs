using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace flexiCoreLibrary
{
    public class SMSFailCodeMeaning
    {
        public static string Get(string failCode)
        {
            var failCodeMeaning = string.Empty;
            switch (failCode)
            {
                case "OUT_OF_RANGE":
                    failCodeMeaning = "A request parameter value can’t be accepted.";
                    break;
                case "IS_EMPTY":
                    failCodeMeaning = "A mandatory request parameter is empty.";
                    break;
                case "TOO_MANY_NUMBERS":
                    failCodeMeaning = "Too many numbers specified.";
                    break;
                case "INVALID_NUMBER":
                    failCodeMeaning = "Number is incorrectly formatted.";
                    break;
                case "TOO_MANY_CHARACTERS":
                    failCodeMeaning = "A request parameter is too long.";
                    break;
                case "INVALID_CHARACTERS":
                    failCodeMeaning = "A request parameter has invalid chars.";
                    break;
                case "DATA_MISSING":
                    failCodeMeaning = "The data was missing.";
                    break;
                case "CALL_BARRED":
                    failCodeMeaning = "The service is restricted by the destination network.";
                    break;
                case "ABSENT_SUBSCRIBER_SM":
                    failCodeMeaning = "The subscriber is absent.";
                    break;
                case "UNEXPECTED_DATA_VALUE":
                    failCodeMeaning = "An unexpected data value in the request.";
                    break;
                case "SYSTEM_FAILURE":
                    failCodeMeaning = "A system failure occurred in the HLR lookup.";
                    break;
                case "FACILITY_NOT_SUPPORTED":
                    failCodeMeaning = "Short message facility is not supported.";
                    break;
                case "TELE_SERVICE_NOT_PROVISIONED":
                    failCodeMeaning = "SMS teleservice is not provisioned.";
                    break;
                case "HLR_REJECT":
                    failCodeMeaning = "The HLR request was rejected.";
                    break;
                case "HLR_ABORT":
                    failCodeMeaning = "The HLR (or some other entity) aborted the request. No response to the request was received.";
                    break;
                case "HLR_LOCAL_CANCEL":
                    failCodeMeaning = "No response for the HLR request was received.";
                    break;
                case "TIMEOUT":
                    failCodeMeaning = "No response to the request was received.";
                    break;
                case "REQUEST_THROTTLED":
                    failCodeMeaning = "Maximum ongoing requests exceeded.";
                    break;
                case "IMSI_LOOKUP_BLOCKED":
                    failCodeMeaning = "Request is blocked.";
                    break;
                default:
                    failCodeMeaning = failCode;
                    break;
            };

            return failCodeMeaning;
        }
    }
}