﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Dtos
{
    public enum OtpVerifyStatus
    {
        Success,
        ExpiredOrInvalid,
        TooManyAttempts
    }
}
