using System;
using System.ComponentModel.DataAnnotations;


namespace Server.Models;

public class PasswordParams
{
    required public string AccountId { get; set; }


    required public string OldPassword { get; set; }

    required public string NewPassword { get; set; }
}

