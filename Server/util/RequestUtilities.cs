using Server.Models;

namespace Server.Util;


/**
This file is mainly for definitions for request types.
*/

public class CheckoutStatusRequest
    {
        public string SessionId { get; set; } = null!;
        public string Address { get; set; } = null!;

        public Item[] Items { get; set; } = null!;
    }