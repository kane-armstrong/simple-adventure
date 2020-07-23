using System;
using System.Collections.Generic;
using System.Text;

namespace PetDoctor.InfrastructureStack
{
    public class MyStackOptions
    {
        public string Prefix { get; set; }
        public string Password { get; set; }
        public int NodeCount { get; set; }
        public string SshKey { get; set; }
    }
}
