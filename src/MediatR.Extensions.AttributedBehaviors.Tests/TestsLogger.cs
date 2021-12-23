using System.Collections.Generic;

namespace MediatR.Extensions.AttributedBehaviors.Tests
{
    public class TestsLogger
    {
        public IList<string> Messages { get; } = new List<string>();
    }
}
