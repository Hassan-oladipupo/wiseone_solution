using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Utility
{
    class Archives
    {
        /* Codes --- from SignInOutAPIController, SignInOutPL and SignInOutDL ---
               

         [HttpPost]
        public HttpResponseMessage StartBreak([FromBody]SignInOutDto clockIn)
        {
            try
            {
                var signIn = SignInOutPL.StartBreak(clockIn);
                return Request.CreateResponse(HttpStatusCode.OK, signIn);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

        [HttpPost]
        public HttpResponseMessage FinishBreak([FromBody]SignInOutDto clockIn)
        {
            try
            {
                var signIn = SignInOutPL.FinishBreak(clockIn);
                return Request.CreateResponse(HttpStatusCode.OK, signIn);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }

           [HttpPut]
        public HttpResponseMessage ApproveOrDeclineOverTime([FromBody]ShiftSwapDto shiftOverTime)
        {
            try
            {
                ShiftConfigPL.ApproveShiftOverTime(shiftOverTime);
                return Request.CreateResponse(HttpStatusCode.OK, string.Format("Request was {0} successfully.", shiftOverTime.ApprovalStatus.ToLower()));
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return response;
            }
        }


        public static bool ApproveOrDeclineOverTime(SignInOutDto signInOut)
        {
            try
            {
                var clockIn = new SignInOut
                {
                    ID = signInOut.ID,
                    OverTimeApprovalStatus = signInOut.OverTimeApprovalStatus
                };

                var updated = SignInOutDL.ApproveOrDeclineOverTime(clockIn, signInOut.ApprovalID);
                if (updated)
                {
                    var approval = new Approval { ID = signInOut.ApprovalID, Status = signInOut.OverTimeApprovalStatus };
                    ApprovalDL.Update(approval);
                    return updated;
                }
                else
                {
                    throw new Exception("Approval request failed.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SignInOutDto StartBreak(SignInOutDto clockIn)
        {
            try
            {
                var signIn = new SignInOut
                {
                    ID = clockIn.ID,
                    StartedBreak = true,
                    StartedBreakTime = DateTime.Now
                };

                var updatedClockIn = SignInOutDL.StartBreak(signIn);

                if (updatedClockIn)
                {
                    clockIn.StartedBreak = signIn.StartedBreak;
                    clockIn.StartedBreakTime = string.Format("{0:H:mm}", signIn.StartedBreakTime);

                    return clockIn;
                }
                else
                {
                    throw new Exception("Start break request failed. Kindly try again.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SignInOutDto FinishBreak(SignInOutDto clockIn)
        {
            try
            {
                var signIn = new SignInOut
                {
                    ID = clockIn.ID,
                    FinishedBreak = true,
                    FinishedBreakTime = DateTime.Now
                };

                var updatedClockIn = SignInOutDL.FinishBreak(signIn);

                if (updatedClockIn)
                {
                    clockIn.FinishedBreak = signIn.FinishedBreak;
                    clockIn.FinishedBreakTime = string.Format("{0:H:mm}", signIn.FinishedBreakTime);

                    return clockIn;
                }
                else
                {
                    throw new Exception("End break request failed. Kindly try again.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

          public static bool ApproveOrDeclineOverTime(SignInOut signInOut, long approvalID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var clockIn = context.SignInOuts.Where(s => s.ID == signInOut.ID).FirstOrDefault();
                    clockIn.OverTimeApprovalStatus = signInOut.OverTimeApprovalStatus;
                    context.Entry(clockIn).State = EntityState.Modified;
                    context.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool StartBreak(SignInOut clockIn)
        {
            try
            {
                using (var context = new DataContext())
                {
                    using (var db = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var signIn = context.SignInOuts.Where(s => s.ID == clockIn.ID).FirstOrDefault();
                            signIn.StartedBreak = clockIn.StartedBreak;
                            signIn.StartedBreakTime = clockIn.StartedBreakTime;
                            context.Entry(signIn).State = EntityState.Modified;
                            context.SaveChanges();

                            var shiftFeeds = context.StaffShiftFeeds.Where(s => s.Date == signIn.Date && s.StaffID == signIn.StaffID);

                            if (shiftFeeds.Any())
                            {
                                var staffShiftFeed = shiftFeeds.FirstOrDefault();
                                staffShiftFeed.StartedBreak = true;
                                staffShiftFeed.StartedBreakTime = clockIn.StartedBreakTime;

                                context.Entry(staffShiftFeed).State = EntityState.Modified;
                                context.SaveChanges();
                            }

                            db.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            db.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool FinishBreak(SignInOut clockIn)
        {
            try
            {
                using (var context = new DataContext())
                {
                    using (var db = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var signIn = context.SignInOuts.Where(s => s.ID == clockIn.ID).FirstOrDefault();
                            signIn.FinishedBreak = clockIn.FinishedBreak;
                            signIn.FinishedBreakTime = clockIn.FinishedBreakTime;
                            context.Entry(signIn).State = EntityState.Modified;
                            context.SaveChanges();

                            var shiftFeeds = context.StaffShiftFeeds.Where(s => s.Date == signIn.Date && s.StaffID == signIn.StaffID);

                            if (shiftFeeds.Any())
                            {
                                var staffShiftFeed = shiftFeeds.FirstOrDefault();
                                staffShiftFeed.FinishedBreak = true;
                                staffShiftFeed.FinishedBreakTime = clockIn.FinishedBreakTime;

                                context.Entry(staffShiftFeed).State = EntityState.Modified;
                                context.SaveChanges();
                            }

                            db.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            db.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    */
    }
}
