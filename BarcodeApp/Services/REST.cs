using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BarcodeApp.Models;
using Newtonsoft.Json;
using System.Reflection;
using RestSharp;
using log4net;

namespace BarcodeApp.Services
{
    class REST
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public LoginToken GetToken(Token t)
        {
            try
            {
                var client = new RestClient("https://services.waterway.com");
                var request = new RestRequest("/api/1.0/account/login");

                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("Accept", "application/json");
                request.AddParameter("grant_type", t.grant_type);
                request.AddParameter("username", t.username);
                request.AddParameter("password", t.password);
                var response = client.Post(request);

                return JsonConvert.DeserializeObject<LoginToken>(response.Content);
            }
            catch (Exception e)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logger.ErrorFormat("Error in {0}.{1}: {2}", m.ReflectedType.Name, m.Name, e.Message);
                return null;
            }
        }

        public Response CheckEmail(Token t, MemberRequest mr)
        {
            try
            {
                var client = new RestClient("https://services.waterway.com");
                var request = new RestRequest("/api/1.0/businessaction/member/byemail");

                request.AddHeader("Accept", "application/json");
                request.AddHeader("Authorization", t.token.token_type + " " + t.token.access_token);
                request.AddParameter("EmailAddress", mr.email);
                var response = client.Post(request);

                return JsonConvert.DeserializeObject<Response>(response.Content);
            }
            catch (Exception e)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logger.ErrorFormat("Error in {0}.{1}: {2}", m.ReflectedType.Name, m.Name, e.Message);
                return null;
            }
        }

        public Response CreateMember(Token t, MemberRequest mr)
        {
            try
            {
                var client = new RestClient("https://services.waterway.com");
                var request = new RestRequest("/api/1.0/businessaction/newmember");

                request.AddHeader("Accept", "application/json");
                request.AddHeader("Authorization", t.token.token_type + " " + t.token.access_token);
                request.AddParameter("FirstName", mr.first);
                request.AddParameter("LastName", mr.last);
                request.AddParameter("EmailAddress", mr.email);
                request.AddParameter("Unit", "99");
                request.AddParameter("MemberLevel", "Blue");
                request.AddParameter("MonthlyBilling", 0);
                request.AddParameter("PaymentRequired", false);
                request.AddParameter("CreateApplication", false);
                request.AddParameter("ChargeCustomerProfile", false);
                request.AddParameter("CustomerProfileId", null);
                request.AddParameter("CustomerPaymentProfileId", null);
                request.AddParameter("Amount", 0);
                request.AddParameter("Cashier", null);
                request.AddParameter("CardNumber", null);
                request.AddParameter("CardExpiration", null);
                request.AddParameter("SendWelcomeEmail", false);

                var response = client.Post(request);

                return JsonConvert.DeserializeObject<Response>(response.Content);
            }
            catch (Exception e)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logger.ErrorFormat("Error in {0}.{1}: {2}", m.ReflectedType.Name, m.Name, e.Message);
                return null;
            }
        }

        public String CreateNote(Token t, MemberNote mn)
        {
            try
            {
                var client = new RestClient("https://services.waterway.com");
                var request = new RestRequest("/api/1.0/businessaction/member/note/add");

                request.AddHeader("Accept", "application/json");
                request.AddHeader("Authorization", t.token.token_type + " " + t.token.access_token);
                request.AddParameter("MemberNumber", mn.MemberNumber);
                request.AddParameter("Note", mn.Note);

                var response = client.Post(request);

                return JsonConvert.DeserializeObject<String>(response.Content);
            }
            catch (Exception e)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logger.ErrorFormat("Error in {0}.{1}: {2}", m.ReflectedType.Name, m.Name, e.Message);
                return null;
            }
        }

        //public ReturnUnusedCodes GetUseCode(Token t, LocationRequest lr)
        //{
        //    try
        //    {
        //        var client = new RestClient("https://services.waterway.com");
        //        var request = new RestRequest("/api/1.0/businessaction/getdrbsingleusedeals/" + lr.UnitNumber);

        //        request.AddHeader("Accept", "application/json");
        //        request.AddHeader("Authorization", t.token.token_type + " " + t.token.access_token);
        //        request.AddParameter("Unit", lr.UnitNumber);

        //        var response = client.Post(request);

        //        return JsonConvert.DeserializeObject<ReturnUnusedCodes>(response.Content);
        //    }
        //    catch (Exception e)
        //    {
        //        MethodBase m = MethodBase.GetCurrentMethod();
        //        Logger.ErrorFormat("Error in {0}.{1}: {2}", m.ReflectedType.Name, m.Name, e.Message);
        //        return null;
        //    }
        //}

        public ReturnSingleUse GetSingleUse(Token t, GetSingleUseCode su)
        {
            try
            {
                var client = new RestClient("https://services.waterway.com");
                var request = new RestRequest("/api/1.0/businessaction/getdrbsingleusecode" + "?unit=" + su.UnitNumber + "&dealName=" + su.DealName + "&memberNumber=" + su.MemberNumber);

                request.AddHeader("Accept", "application/json");
                request.AddHeader("Authorization", t.token.token_type + " " + t.token.access_token);
                request.AddParameter("unit", su.UnitNumber);
                request.AddParameter("dealName", su.DealName);
                request.AddParameter("memberNumber", su.MemberNumber);

                var response = client.Post(request);

                return JsonConvert.DeserializeObject<ReturnSingleUse>(response.Content);
            }
            catch (Exception e)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logger.ErrorFormat("Error in {0}.{1}: {2}", m.ReflectedType.Name, m.Name, e.Message);
                return null;
            }
        }

    }
}
