using System;

namespace MB.Model
{
    public class AuthResult
    {
        public int resultId { get; set; }
        public Nullable<int> authId { get; set; }
        public Nullable<int> targetId { get; set; }
    }
}