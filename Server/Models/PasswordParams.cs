using System;
using System.ComponentModel.DataAnnotations;


namespace Server.Models;

public class PasswordParams
{
    required public string OldPassword { get; set; }

    required public string NewPassword { get; set; }
}

