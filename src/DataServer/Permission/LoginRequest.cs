﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer.Permission
{
    public class LoginRequest
    {
        public string Account { get; set; }
        public string Password { get; set; }
    }
}
