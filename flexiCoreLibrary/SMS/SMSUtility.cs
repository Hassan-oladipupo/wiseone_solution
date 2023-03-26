using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZenSend;

namespace flexiCoreLibrary
{
    public class SMSUtility
    {
        public static decimal Balance()
        {
            try
            {
                var api_key = System.Configuration.ConfigurationManager.AppSettings.Get("APIKey");
                var originator = System.Configuration.ConfigurationManager.AppSettings.Get("SMSTitle");

                var client = new Client(api_key);

                var balance = client.CheckBalance();
                return balance;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static SMSResponse MobileNumberLookUp(string mobileNumber)
        {
            try
            {
                var api_key = System.Configuration.ConfigurationManager.AppSettings.Get("APIKey");
                var originator = System.Configuration.ConfigurationManager.AppSettings.Get("SMSTitle");

                var client = new Client(api_key);

                var lookup = client.LookupOperator(mobileNumber);

                return new SMSResponse
                {
                    Successful = true,
                    FailCode = string.Empty,
                    ErrorMessage = string.Empty
                };
            }
            catch (System.Net.WebException e)
            {
                return new SMSResponse
                {
                    Successful = false,
                    FailCode = "WEB_EXCEPTION",
                    ErrorMessage = e.Message
                };
            }
            catch (ZenSendException e)
            {
                return new SMSResponse
                {
                    Successful = false,
                    FailCode = e.FailCode,
                    ErrorMessage = SMSFailCodeMeaning.Get(e.FailCode)
                };
            }
            catch (Exception e)
            {
                return new SMSResponse
                {
                    Successful = false,
                    FailCode = "GENERAL_EXCEPTION",
                    ErrorMessage = e.Message
                };
            }
        }

        public static SMSResponse SendMessage(string message, string[] numbers)
        {
            try
            {
                if (numbers.Any())
                {
                    var api_key = System.Configuration.ConfigurationManager.AppSettings.Get("APIKey");
                    var originator = System.Configuration.ConfigurationManager.AppSettings.Get("SMSTitle");                   

                    var balance = SMSUtility.Balance();
                    if (balance <= 5.00m)
                    {
                        return new SMSResponse
                        {
                            Successful = false,
                            FailCode = "INSUFFICIENT_FUND",
                            ErrorMessage = "Insufficient fund to process request"
                        };
                    }
                    else
                    {
                        var client = new Client(api_key);

                        var result = client.SendSms(
                          originator: originator,
                          body: message,
                          numbers: numbers
                        );

                        return new SMSResponse
                        {
                            Successful = true,
                            FailCode = string.Empty,
                            ErrorMessage = string.Empty
                        };
                    }
                }
                else
                {
                    return new SMSResponse
                    {
                        Successful = false,
                        FailCode = "INVALID_SMS_REQUEST",
                        ErrorMessage = "Mobile number and message are required."
                    };
                }
            }
            catch (System.Net.WebException e)
            {
                return new SMSResponse
                {
                    Successful = false,
                    FailCode = "WEB_EXCEPTION",
                    ErrorMessage = e.Message
                };
            }
            catch (ZenSendException e)
            {
                return new SMSResponse
                {
                    Successful = false,
                    FailCode = e.FailCode,
                    ErrorMessage = SMSFailCodeMeaning.Get(e.FailCode)
                };
            }
            catch (Exception e)
            {
                return new SMSResponse
                {
                    Successful = false,
                    FailCode = "GENERAL_EXCEPTION",
                    ErrorMessage = e.Message
                };
            }
        }
    }
}