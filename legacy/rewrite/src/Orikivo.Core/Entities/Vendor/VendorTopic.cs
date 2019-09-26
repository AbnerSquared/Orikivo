using System.Collections.Generic;

namespace Orikivo
{
    public class VendorTopic
    {
        public VendorTopic(Vendor vendor) // get a random vendor topic based from the vendor.
        {
            
        }
        public string Question {get; private set;}
        public List<VendorTopicResponse> Responses {get; private set;}
    }
}