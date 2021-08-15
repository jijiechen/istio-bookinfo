using System;

namespace Reviews
{
    public class Review
    {
        public string Reviewer { get; set; }
        public string Text { get; set; }
        public Rating Rating { get; set; }
        
    }

    public class Rating
    {
        public string Error { get; set; }
        public int? StarCounts { get; set; }
        public string StarColor { get; set; }
    }
}